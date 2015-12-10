namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class CodeActionCommand : Command
	{
		public readonly string Code;

		public CodeActionCommand(string code)
		{
			Code = code;
		}
	}
}
