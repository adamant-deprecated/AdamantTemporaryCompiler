using Adamant.CompilerCompiler.Lex.FiniteAutomata.EmitActions;
using Adamant.CompilerCompiler.Lex.Spec;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public abstract class LexerEmitAction
	{
		public static LexerEmitAction More { get; } = MoreToken.Instance;
		public static LexerEmitAction Skip { get; } = SkipToken.Instance;
		public static LexerEmitAction Token(Channel channel, string tokenType, bool isError)
		{
			return new EmitToken(channel, tokenType, isError);
		}
	}
}
