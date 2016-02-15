namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class DecodeCommand : Command
	{
		public readonly int Base;

		public DecodeCommand(int @base)
		{
			Base = @base;
		}
	}
}
