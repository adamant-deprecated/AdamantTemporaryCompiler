namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class CaptureCommand : Command
	{
		#region Singleton
		public static Command Instance { get; } = new CaptureCommand();

		private CaptureCommand()
		{
		}
		#endregion
	}
}
