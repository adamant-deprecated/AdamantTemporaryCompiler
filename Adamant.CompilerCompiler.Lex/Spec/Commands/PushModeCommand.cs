using System;

namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class PushModeCommand : Command
	{
		public readonly Mode Mode;

		public PushModeCommand(Mode mode)
		{
			if(mode == null)
				throw new ArgumentNullException();
			Mode = mode;
		}

		public override Mode ModeEntered()
		{
			return Mode;
		}
	}
}
