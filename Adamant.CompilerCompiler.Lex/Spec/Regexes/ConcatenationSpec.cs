using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class ConcatenationSpec : BinaryRegexSpec
	{
		public ConcatenationSpec(IEnumerable<RegexSpec> expressions)
			: base(expressions)
		{
		}

		public override RegexSpec Simplify(LexerSpec lexerSpec)
		{
			var simplified = Expressions.Select(e => e.Simplify(lexerSpec)).ToList();
			return Expressions.SequenceEqual(simplified) ? this : new ConcatenationSpec(simplified);
		}

		public override string Operator => " ";

		public override StateRange AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			return Expressions.Select(exp => exp.AddTo(nfa, equivalenceClasses)).Aggregate((states1, states2) =>
			{
				nfa.AddEpsilonTransition(states1.End, states2.Start);
				return new StateRange(states1.End, states2.Start);
			});
		}
	}
}
