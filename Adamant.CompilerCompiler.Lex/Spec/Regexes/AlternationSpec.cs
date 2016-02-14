using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;
using Sys.Text;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class AlternationSpec : BinaryRegexSpec
	{
		public AlternationSpec(IEnumerable<RegexSpec> expressions)
			: base(expressions)
		{
		}

		public sealed override RegexSpec Simplify(LexerSpec lexerSpec)
		{
			var codePoints = InversionListCodePointSet.Empty;
			var simplified = new List<RegexSpec>();
			foreach(var exp in Expressions.Select(e => e.Simplify(lexerSpec)))
			{
				var charClass = exp.AsCharClass();
				if(charClass != null)
				{
					// [x]|[y] -> [xy]
					codePoints = codePoints.Union(charClass.CodePoints);
				}
				else
					simplified.Add(exp);
			}
			if(!codePoints.IsEmpty())
				simplified.Add(new CharClassSpec(codePoints));

			//  All expressions reduced
			if(simplified.Count == 1)
				return simplified[0];

			return Expressions.SequenceEqual(simplified) ? this : new AlternationSpec(simplified);
		}

		public override string Operator => "|";

		public override StateRange AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			var startState = nfa.AddState();
			var endState = nfa.AddState();
			foreach(var states in Expressions.Select(exp => exp.AddTo(nfa, equivalenceClasses)))
			{
				nfa.AddEpsilonTransition(startState, states.Start);
				nfa.AddEpsilonTransition(states.End, endState);
			}
			return new StateRange(startState, endState);
		}
	}
}
