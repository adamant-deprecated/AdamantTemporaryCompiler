namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class MoreCommand : Command
	{
		#region Singleton
		public static Command Instance { get; } = new MoreCommand();

		private MoreCommand()
		{
		}
		#endregion
	}
}
