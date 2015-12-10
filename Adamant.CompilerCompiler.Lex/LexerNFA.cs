using System.Collections.Generic;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.Services;
using Adamant.CompilerCompiler.Lex.Spec;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex
{
	public class LexerNFA
	{
		public LexerNFA(IDictionary<Mode, State> modeMap, CodePointEquivalenceClasses equivalenceClasses, NFA<LexerAction> nfa)
		{
			ModeMap = new Dictionary<Mode, State>(modeMap);
			EquivalenceClasses = equivalenceClasses;
			Nfa = nfa;
		}

		public IReadOnlyDictionary<Mode, State> ModeMap { get; }
		public CodePointEquivalenceClasses EquivalenceClasses { get; }
		public NFA<LexerAction> Nfa { get; }

		public LexerDFA ToDFA()
		{
			return LexerTransformer.Instance.ConvertToDFA(this);
		}
	}
}
