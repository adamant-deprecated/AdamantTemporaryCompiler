namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class FlagErrorCommand : Command
	{
		#region Singleton
		public static Command Instance { get; } = new FlagErrorCommand();

		private FlagErrorCommand()
		{
		}
		#endregion
	}
}
