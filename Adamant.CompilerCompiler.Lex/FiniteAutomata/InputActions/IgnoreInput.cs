namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions
{
	public class IgnoreInput : LexerInputAction
	{
		#region Singleton
		public static LexerInputAction Instance { get; } = new IgnoreInput();

		private IgnoreInput()
		{
		}
		#endregion
	}
}
