using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.Spec;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex
{
	public class LexerDFA
	{
		public LexerDFA(LexerSpec lexerSpec, IDictionary<Mode, State> modeMap, CodePointEquivalenceClasses equivalenceClasses, DFA<LexerAction> dfa)
		{
			LexerSpec = lexerSpec;
			ModeMap = new Dictionary<Mode, State>(modeMap);
			EquivalenceClasses = equivalenceClasses;
			Dfa = dfa;
		}

		public LexerSpec LexerSpec { get; }
		public IReadOnlyDictionary<Mode, State> ModeMap { get; }
		public CodePointEquivalenceClasses EquivalenceClasses { get; }
		public DFA<LexerAction> Dfa { get; set; }

		public LexerDFA Minimize()
		{
			var minResult = Dfa.Minimize(StatesEquivalent, Combine);

			var modeMap = ModeMap.ToDictionary(e => e.Key, e => minResult.Item1[e.Value]);
			return new LexerDFA(LexerSpec, modeMap, EquivalenceClasses, minResult.Item2);
		}

		private bool StatesEquivalent(State a, State b)
		{
			return Equals(Dfa.GetData(a), Dfa.GetData(b));
		}

		private static LexerAction Combine(LexerAction a, LexerAction b)
		{
			if(a == null) return b;
			if(b == null) return a;

			if(!Equals(a, b))
				throw new NotSupportedException("Can't combine two lexer actions that are not equivalent");

			return a.Priority <= b.Priority ? a : b;
		}
	}
}
