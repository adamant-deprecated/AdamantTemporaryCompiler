namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions
{
	public class IgnoreValue : LexerValueAction
	{
		#region Singleton
		public static LexerValueAction Instance { get; } = new IgnoreValue();

		private IgnoreValue()
		{
		}
		#endregion
	}
}
