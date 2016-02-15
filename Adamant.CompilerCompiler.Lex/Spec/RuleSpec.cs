using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.FiniteAutomata.ModeActions;
using Adamant.CompilerCompiler.Lex.Spec.Commands;
using Adamant.CompilerCompiler.Lex.Spec.Regexes;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec
{
	public class RuleSpec
	{
		public readonly ISet<Mode> Modes;
		public readonly string Name;
		public readonly bool IsFragment;
		public readonly RegexSpec Expression;
		public readonly IReadOnlyList<Command> Commands;

		public RuleSpec(IEnumerable<Mode> modes, string name, RegexSpec expression, IEnumerable<Command> commands)
		{
			Modes = new HashSet<Mode>(modes);
			Name = name;
			IsFragment = char.IsLower(name, 0);
			Expression = expression;
			Commands = commands.ToList();
		}

		public RuleSpec(Mode mode, string name, RegexSpec expression, params Command[] commands)
			: this(new[] { mode }, name, expression, commands)
		{
		}

		public void Validate(LexerSpec lexer)
		{
			var modeReferenceErrors = Modes.Except(lexer.Modes).ToList();
			if(modeReferenceErrors.Any())
				throw new Exception($"Rule '{Name}' references mode(s) not in lexer spec: '{string.Join("', '", modeReferenceErrors)}'");
			ValidateCommands(lexer);
			Expression.Validate(lexer);
		}

		private void ValidateCommands(LexerSpec lexer)
		{
			if(IsFragment && Commands.Any())
				throw new Exception($"Rule '{Name}' is a fragment, but has commands.  This is not allowed");

			if(Commands.Count(c => c == Command.Skip || c == Command.More || c is SetTypeCommand) > 1)
				throw new Exception($"Rule '{Name}', only one of @skip, @more or @type command is allowed per rule");

			if(Commands.Count(c => c == Command.Capture || c is DecodeCommand || c is TextCommand) > 1)
				throw new Exception($"Rule '{Name}', only one of @capture, @decode or @text command is allowed per rule");

			if(Commands.Contains(Command.Skip) && Commands.Contains(Command.FlagError))
				throw new Exception($"Rule '{Name}', skipped rules can't be marked @error");

			if(Commands.Contains(Command.Skip) && Commands.Contains(Command.Capture))
				throw new Exception($"Rule '{Name}', skipped rules can't be marked @capture");

			if(Commands.Contains(Command.Skip) && Commands.Any(c => c is DecodeCommand))
				throw new Exception($"Rule '{Name}', skipped rules can't be marked @decode");

			if(Commands.Contains(Command.Skip) && Commands.Any(c => c is DecodeCommand))
				throw new Exception($"Rule '{Name}', skipped rules can't be marked @text");

			if(Commands.Contains(Command.Skip) && Commands.Any(c => c is SetChannelCommand))
				throw new Exception($"Rule '{Name}', skipped rules can't be marked with a @channel");

			if(Commands.Contains(Command.More) && Commands.Contains(Command.FlagError))
				throw new Exception($"Rule '{Name}', more rules can't be marked @error");

			if(Commands.Contains(Command.More) && Commands.Any(c => c is SetChannelCommand))
				throw new Exception($"Rule '{Name}', more rules can't be marked with a @channel");

			if(Commands.OfType<SetChannelCommand>().Count() > 1)
				throw new Exception($"Rule '{Name}', only one @channel command is allowed per rule");

			if(Commands.Count(c => c == Command.FlagError) > 1)
				throw new Exception($"Rule '{Name}', only one @error command is allowed per rule");

			if(Commands.Reverse().Skip(1).Any(c => c is CodeActionCommand))
				throw new Exception($"Rule '{Name}', there can only be one code action per rule, and it must be the last command");

			foreach(var command in Commands)
				command.Validate(this, lexer);
		}

		public ISet<Mode> ModesEntered()
		{
			return new HashSet<Mode>(Commands.Select(command => command.ModeEntered()).Where(mode => mode != null));
		}

		public RuleSpec Simplify(ISet<Mode> modesEntered, LexerSpec lexer)
		{
			var reducedModes = new HashSet<Mode>(Modes.Intersect(modesEntered));
			var simplfiedExpression = Expression.Simplify(lexer);
			if(reducedModes.Count >= Modes.Count && Expression == simplfiedExpression) return this;

			return new RuleSpec(reducedModes, Name, simplfiedExpression, Commands);
		}

		public void AddStates(IDictionary<Mode, State> modeMap, CodePointEquivalenceClasses equivalenceClasses, NFA<LexerAction> nfa, int priority, Channel defaultChannel)
		{
			var states = Expression.AddTo(nfa, equivalenceClasses);

			foreach(var mode in Modes)
				nfa.AddEpsilonTransition(modeMap[mode], states.Start);

			nfa.SetFinal(states.End);

			// Set Action
			nfa.SetData(states.End, new LexerAction(priority, GetValueAction(), GetModeActions(), GetEmitAction(defaultChannel), GetCodeAction()));
		}

		public Channel Channel => Commands.Select(c => c.Channel).SingleOrDefault(c => c != null);

		public string TokenType => Commands.Select(c => c.TokenType).SingleOrDefault(t => t != null) ?? Name;

		private LexerValueAction GetValueAction()
		{
			if(Commands.Contains(Command.Capture))
				return LexerValueAction.Capture;

			var decode = Commands.OfType<DecodeCommand>().SingleOrDefault();
			if(decode != null)
				return LexerValueAction.Decode(decode.Base);

			var text = Commands.OfType<TextCommand>().SingleOrDefault();
			if(text != null)
				return LexerValueAction.Text(text.Value);

			return LexerValueAction.Ignore;
		}

		private IEnumerable<LexerModeAction> GetModeActions()
		{
			var actions = Commands.SelectMany(c => c.ModeActions()).ToList();

			// We do not check for the additional possible simplification of
			// push, set(x), push, set(x) -> push, set(x), push

			// TODO change this to something more object oriented
			for(var i = actions.Count - 2; i >= 0; i--)
			{
				var action = actions[i];
				var nextAction = actions[i + 1];
				if(action is SetMode && (nextAction is PopMode || nextAction is SetMode))
				{
					//	set(x), pop -> pop
					//	set(x), set(y) -> set(y)
					actions.RemoveAt(i);
				}
				else if(action is PushMode && nextAction is PopMode)
				{
					//	push, pop -> nothing
					actions.RemoveAt(i);
					actions.RemoveAt(i);
				}
			}

			return actions;
		}

		private LexerEmitAction GetEmitAction(Channel defaultChannel)
		{
			if(Commands.Contains(Command.More))
				return LexerEmitAction.More;

			if(Commands.Contains(Command.Skip))
				return LexerEmitAction.Skip;

			return LexerEmitAction.Token(Channel ?? defaultChannel, TokenType, Commands.Contains(Command.FlagError));
		}

		private string GetCodeAction()
		{
			return Commands.OfType<CodeActionCommand>().SingleOrDefault()?.Code;
		}
	}
}
