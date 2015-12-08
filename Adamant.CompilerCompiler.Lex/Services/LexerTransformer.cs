using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.Spec;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Services
{
	public class LexerTransformer
	{
		#region Singleton
		public static LexerTransformer Instance { get; } = new LexerTransformer();

		private LexerTransformer()
		{
		}
		#endregion

		public LexerNFA ConvertToNFA(LexerSpec spec)
		{
			spec = spec.HasBeenSimplified ? spec : spec.Simplify();
			var equivalenceClasses = MakeEquivalenceClasses(spec);
			var nfa = new NFA<LexerAction>(equivalenceClasses.Count);
			var modeMap = spec.Modes.ToDictionary(m => m, m => nfa.AddState(true));

			foreach(var rule in spec.Rules)
				AddStates(rule, modeMap, equivalenceClasses, nfa);

			return new LexerNFA(modeMap, equivalenceClasses, nfa);
		}

		private static CodePointEquivalenceClasses MakeEquivalenceClasses(LexerSpec spec)
		{
			var equivalenceClasses = new CodePointEquivalenceClasses();
			foreach(var rule in spec.Rules)
				rule.Expression.MakeEquivalenceClasses(equivalenceClasses);

			return equivalenceClasses;
		}

		// TODO move to RuleSpec?
		private static void AddStates(RuleSpec rule, IDictionary<Mode, State> modeMap, CodePointEquivalenceClasses equivalenceClasses, NFA<LexerAction> nfa)
		{
			var states = rule.Expression.AddTo(nfa, equivalenceClasses);

			foreach(var mode in rule.Modes)
				nfa.AddEpsilonTransition(modeMap[mode], states.Start);

			nfa.SetFinal(states.End);
			// TODO nfa.SetData(rule.Commands)
		}

		public LexerDFA ConvertToDFA(LexerNFA lexerNFA)
		{
			var dfa = lexerNFA.Nfa.ToDFA(stateActions => stateActions.First().Item2);
			return new LexerDFA(lexerNFA.EquivalenceClasses, dfa);
		}
	}
}
