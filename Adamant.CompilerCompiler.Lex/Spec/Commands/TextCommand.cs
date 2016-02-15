namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class TextCommand : Command
	{
		public readonly string Value;

		public TextCommand(string value)
		{
			Value = value;
		}
	}
}
