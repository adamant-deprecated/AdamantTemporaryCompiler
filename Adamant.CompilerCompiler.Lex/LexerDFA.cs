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
			Dfa.Minimize(StatesEquivalent, Combine);
		}

		private bool StatesEquivalent(State a, State b)
		{
			return Equals(Dfa.GetData(a), Dfa.GetData(b));
		}

		private LexerAction Combine(LexerAction arg1, LexerAction arg2)
		{
			throw new System.NotImplementedException();
		}
	}
}
