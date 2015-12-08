namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class PopModeCommand : Command
	{
		public readonly Mode Mode;

		public PopModeCommand()
		{
		}

		public PopModeCommand(Mode mode)
		{
			Mode = mode;
		}
	}
}
