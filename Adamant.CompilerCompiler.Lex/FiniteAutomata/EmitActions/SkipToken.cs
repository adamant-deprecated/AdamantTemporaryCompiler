namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.EmitActions
{
	public class SkipToken : LexerEmitAction
	{
		#region Singleton
		public static LexerEmitAction Instance { get; } = new SkipToken();

		private SkipToken()
		{
		}
		#endregion
	}
}
