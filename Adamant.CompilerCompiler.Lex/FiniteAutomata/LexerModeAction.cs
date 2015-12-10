using Adamant.CompilerCompiler.Lex.FiniteAutomata.ModeActions;
using Adamant.CompilerCompiler.Lex.Spec;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public abstract class LexerModeAction
	{
		public static LexerModeAction Pop { get; } = PopMode.Instance;
		public static LexerModeAction Push { get; } = PushMode.Instance;
		public static LexerModeAction Set(Mode mode)
		{
			return new SetMode(mode);
		}
	}
}
