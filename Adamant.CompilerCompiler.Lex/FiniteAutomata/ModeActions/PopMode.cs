namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.ModeActions
{
	public class PopMode : LexerModeAction
	{
		#region Singleton
		public static LexerModeAction Instance { get; } = new PopMode();

		private PopMode()
		{
		}
		#endregion
	}
}
