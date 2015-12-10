namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class SkipCommand : Command
	{
		#region Singleton
		public static Command Instance { get; } = new SkipCommand();

		private SkipCommand()
		{
		}
		#endregion

		public override string GetTokenType(string tokenType)
		{
			return null;
		}
	}
}
