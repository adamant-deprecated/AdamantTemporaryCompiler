using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.Spec.Regexes;

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
			Commands = new List<Command>(commands);
			if(IsFragment && Commands.Any())
				throw new ArgumentException("A fragment rule can't have commands");
		}

		public RuleSpec(Mode mode, string name, RegexSpec expression, params Command[] commands)
			: this(new[] { mode }, name, expression, commands)
		{
		}

		public void Validate(LexerSpec lexerSpec)
		{
			var modeReferenceErrors = Modes.Except(lexerSpec.Modes).ToList();
			if(modeReferenceErrors.Any())
				throw new Exception($"Rule '{Name}' references mode(s) not in lexer spec: '{string.Join("', '", modeReferenceErrors)}'");
			Modes.IsSubsetOf(lexerSpec.Modes);
			Expression.Validate(lexerSpec);
		}

		public ISet<Mode> ModesEntered()
		{
			return new HashSet<Mode>(Commands.Select(command => command.ModeEntered()).Where(mode => mode != null));
		}

		public RuleSpec Simplify(ISet<Mode> modesEntered, LexerSpec lexerSpec)
		{
			var reducedModes = new HashSet<Mode>(Modes.Intersect(modesEntered));
			var simplfiedExpression = Expression.Simplify(lexerSpec);
			if(reducedModes.Count >= Modes.Count && Expression == simplfiedExpression) return this;

			return new RuleSpec(reducedModes, Name, simplfiedExpression, Commands);
		}
	}
}
