namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.EmitActions
{
	public class MoreToken : LexerEmitAction
	{
		#region Singleton
		public static LexerEmitAction Instance { get; } = new MoreToken();

		private MoreToken()
		{
		}
		#endregion
	}
}
