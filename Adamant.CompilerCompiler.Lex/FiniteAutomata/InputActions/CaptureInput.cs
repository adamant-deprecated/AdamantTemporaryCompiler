namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions
{
	public class CaptureInput : LexerInputAction
	{
		#region Singleton
		public static LexerInputAction Instance { get; } = new CaptureInput();

		private CaptureInput()
		{
		}
		#endregion
	}
}
