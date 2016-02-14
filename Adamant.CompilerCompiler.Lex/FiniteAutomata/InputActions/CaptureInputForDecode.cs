namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions
{
	public class CaptureInputForDecode : LexerInputAction
	{
		#region Singleton
		public static LexerInputAction Instance { get; } = new CaptureInputForDecode();

		private CaptureInputForDecode()
		{
		}
		#endregion
	}
}
