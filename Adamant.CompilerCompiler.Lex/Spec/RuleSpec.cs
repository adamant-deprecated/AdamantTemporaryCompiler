using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
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
			IsFragment = Char.IsLower(name, 0);
			Expression = expression;
			Commands = new List<Command>(commands);
			if(IsFragment && Commands.Any())
				throw new ArgumentException("A fragment rule can't have commands");
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

			if(Commands.Contains(Command.Skip) && Commands.Contains(Command.FlagError))
				throw new Exception($"Rule '{Name}', skipped rules can't be marked @error");

			if(Commands.Reverse().Skip(1).Any(c => c is CodeActionCommand))
				throw new Exception($"Rule '{Name}', there can only be one code action per rule, and it must be the last command");

			foreach(var command in Commands)
				command.Validate(this, lexer);
		}

		public ISet<Channel> ChannelsUsed()
		{
			return new HashSet<Channel>(Commands.Select(command => command.ChannelUsed()).Where(channel => channel != null));
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

		public void AddStates(IDictionary<Mode, State> modeMap, CodePointEquivalenceClasses equivalenceClasses, NFA<LexerAction> nfa, int priority)
		{
			var states = Expression.AddTo(nfa, equivalenceClasses);

			foreach(var mode in Modes)
				nfa.AddEpsilonTransition(modeMap[mode], states.Start);

			nfa.SetFinal(states.End);

			// Input Action
			var inputAction = LexerInputAction.Ignore; // TODO put the right thing here

			// Mode Actions
			var modeActions = Functions.GetModeActions(Commands);

			// Emit Actions
			LexerEmitAction emitAction;
			if(Commands.Contains(Command.More))
				emitAction = LexerEmitAction.Nothing;
			else if(Commands.Contains(Command.Skip))
				emitAction = LexerEmitAction.Skip;
			else
				// TODO handle channel correctly
				emitAction = LexerEmitAction.Token(0, GetTokenType(), Commands.Contains(Command.FlagError));

			var code = Commands.OfType<CodeActionCommand>().SingleOrDefault()?.Code;
			nfa.SetData(states.End, new LexerAction(priority, inputAction, modeActions, emitAction, code));
		}

		private string GetTokenType()
		{
			return Commands.Aggregate(Name, (current, command) => command.GetTokenType(current));
		}
	}
}
