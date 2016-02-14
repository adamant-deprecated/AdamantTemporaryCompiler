using System.Collections.Generic;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.Spec;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex
{
	public class LexerNFA
	{
		public LexerNFA(LexerSpec lexerSpec, IDictionary<Mode, State> modeMap, CodePointEquivalenceClasses equivalenceClasses, NFA<LexerAction> nfa)
		{
			this.LexerSpec = lexerSpec;
			ModeMap = new Dictionary<Mode, State>(modeMap);
			EquivalenceClasses = equivalenceClasses;
			Nfa = nfa;
		}

		public LexerSpec LexerSpec { get; }
		public IReadOnlyDictionary<Mode, State> ModeMap { get; }
		public CodePointEquivalenceClasses EquivalenceClasses { get; }
		public NFA<LexerAction> Nfa { get; }
	}
}
