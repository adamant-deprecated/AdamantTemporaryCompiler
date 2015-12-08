using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class ComplementSpec : UnaryRegexSpec
	{
		public ComplementSpec(RegexSpec expression)
			: base(expression)
		{
		}

		protected override RegexSpec Simplify(LexerSpec lexerSpec, RegexSpec exp, bool changed)
		{
			// ![x] -> [^x]
			var charClass = Expression.Simplify(lexerSpec).AsCharClass();
			if(charClass != null)
				return new CharClassSpec(charClass.CodePoints.Complement());

			// !!x -> x
			var complement = exp as ComplementSpec;
			if(complement != null)
				return complement.Expression;

			return changed ? new ComplementSpec(exp) : this;
		}

		public override States AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			// Create a separate NFA
			var tempNFA = new NFA<State?>(equivalenceClasses.Count);
			var states = Expression.AddTo(tempNFA, equivalenceClasses);
			tempNFA.SetStart(states.Start);
			tempNFA.SetFinal(states.End);

			// Convert to a DFA
			var dfa = tempNFA.ToDFA(stateData => null);
			// TODO minimize DFA
			dfa.MakeComplete(); // Can only do complement on complete DFA

			// Now add the complement to this nfa
			var startState = nfa.AddState();
			var endState = nfa.AddState();
			foreach(var dfaState in dfa.States)
			{
				var nfaState = GetMatchingState(nfa, dfa, dfaState);
				if(dfa.IsStart(dfaState))
					nfa.AddEpsilonTransition(startState, nfaState);
				if(!dfa.IsFinal(dfaState))
					nfa.AddEpsilonTransition(nfaState, endState);
				// Copy transitions
				foreach(var input in nfa.Inputs)
				{
					var toDFAState = dfa.GetTransition(dfaState, input);
					if(toDFAState != null)
					{
						var toNFAState = GetMatchingState(nfa, dfa, toDFAState.Value);
						nfa.AddTransition(nfaState, input, toNFAState);
					}
				}
			}

			return new States(startState, endState);
		}

		private static State GetMatchingState<T>(NFA<T> nfa, DFA<State?> dfa, State dfaState)
		{
			var nfaState = dfa.GetData(dfaState);
			if(nfaState == null)
			{
				nfaState = nfa.AddState();
				dfa.SetData(dfaState, nfaState);
			}
			return nfaState.Value;
		}

		public override string ToString()
		{
			return $"!{Expression}";
		}
	}
}
