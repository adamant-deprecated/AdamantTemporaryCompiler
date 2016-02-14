namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.EmitActions
{
	public class EmitNothing : LexerEmitAction
	{
		#region Singleton
		public static LexerEmitAction Instance { get; } = new EmitNothing();

		private EmitNothing()
		{
		}
		#endregion
	}
}
