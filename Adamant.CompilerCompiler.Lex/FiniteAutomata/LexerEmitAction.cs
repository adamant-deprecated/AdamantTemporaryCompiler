using Adamant.CompilerCompiler.Lex.FiniteAutomata.EmitActions;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public abstract class LexerEmitAction
	{
		public static LexerEmitAction Nothing { get; } = EmitNothing.Instance;
		public static LexerEmitAction Skip { get; } = SkipToken.Instance;


		public static LexerEmitAction Token(int channel, string tokenType, bool isError)
		{
			return new EmitToken(channel, tokenType, isError);
		}
	}
}
