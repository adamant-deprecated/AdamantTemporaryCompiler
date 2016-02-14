using System;
using System.Collections.Generic;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	/// <summary>
	/// Push the current mode onto a stack and then set the mode to the given mode
	/// </summary>
	public class PushModeCommand : Command
	{
		public readonly Mode Mode;

		public PushModeCommand(Mode mode)
		{
			if(mode == null) throw new ArgumentNullException(nameof(mode));
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
			yield return LexerModeAction.Push;
			yield return LexerModeAction.Set(Mode);
		}
	}
}
