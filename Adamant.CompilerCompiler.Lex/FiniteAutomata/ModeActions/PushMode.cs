namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.ModeActions
{
	public class PushMode : LexerModeAction
	{
		#region Singleton
		public static LexerModeAction Instance { get; } = new PushMode();

		private PushMode()
		{
		}
		#endregion
	}
}
