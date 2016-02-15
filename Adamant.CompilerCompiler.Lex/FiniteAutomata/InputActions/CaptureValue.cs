namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions
{
	public class CaptureValue : LexerValueAction
	{
		#region Singleton
		public static LexerValueAction Instance { get; } = new CaptureValue();

		private CaptureValue()
		{
		}
		#endregion
	}
}
