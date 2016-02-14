using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class RepetitionSpec : UnaryRegexSpec
	{
		public readonly int MinRepititions;
		public readonly int? MaxRepititions;

		public RepetitionSpec(RegexSpec exp, int minRepititions, int? maxRepititions)
			: base(exp)
		{
			MinRepititions = minRepititions;
			MaxRepititions = maxRepititions;
		}

		protected override RegexSpec Simplify(LexerSpec lexerSpec, RegexSpec exp, bool changed)
		{
			// x{1,1} -> x
			if(MinRepititions == 1 && MaxRepititions == 1)
				return exp;

			return changed ? new RepetitionSpec(exp, MinRepititions, MaxRepititions) : this;
		}

		public override States AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			var reps = AddReps(nfa, equivalenceClasses).ToList();

			if(MaxRepititions == null)
			{
				// Repeat last rep as many times as needed
				var lastRep = reps.Last();
				nfa.AddEpsilonTransition(lastRep.End, lastRep.Start);
			}

			return reps.Aggregate((states1, states2) =>
			{
				nfa.AddEpsilonTransition(states1.End, states2.Start);
				return new States(states1.Start, states2.End);
			});
		}

		private IEnumerable<States> AddReps<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			// Add required reps
			for(var i = 0; i < MinRepititions; i++)
				yield return Expression.AddTo(nfa, equivalenceClasses);

			// Add optional reps
			// If min=0 and max=*, then we want to make one optional rep to use for the * hence `MaxRepititions ?? 1`
			var optionalReps = (MaxRepititions ?? 1) - MinRepititions;
			for(var i = 0; i < optionalReps; i++)
			{
				var states = Expression.AddTo(nfa, equivalenceClasses);
				// Can skip these reps
				nfa.AddEpsilonTransition(states.Start, states.End);
				yield return states;
			}
		}

		public override string ToString()
		{
			return $"{Expression}{{{MinRepititions},{MaxRepititions}}}";
		}
	}
}
