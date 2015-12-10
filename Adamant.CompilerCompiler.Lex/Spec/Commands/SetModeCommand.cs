using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class SetModeCommand : Command
	{
		public readonly Mode Mode;

		public SetModeCommand(Mode mode)
		{
			if(mode == null)
				throw new ArgumentNullException();
			Mode = mode;
		}

		public override Mode ModeEntered()
		{
			return Mode;
		}

		public override void Validate(RuleSpec rule, LexerSpec lexer)
		{
			if(!lexer.Modes.Contains(Mode))
				throw new Exception($"Rule '{rule.Name}' references mode not in lexer spec: '{Mode}'");
		}

		public override IEnumerable<LexerModeAction> ModeActions()
		{
			return Enumerable.Repeat(LexerModeAction.Set(Mode), 1);
		}
	}
}
