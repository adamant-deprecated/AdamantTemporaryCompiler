using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex
{
	public class LexerDFA
	{
		public LexerDFA(CodePointEquivalenceClasses equivalenceClasses, DFA<LexerAction> dfa)
		{
			EquivalenceClasses = equivalenceClasses;
			Dfa = dfa;
		}

		public CodePointEquivalenceClasses EquivalenceClasses { get; }
		public DFA<LexerAction> Dfa { get; set; }

		public void Minimize()
		{
			Dfa.Minimize();
		}
	}
}
