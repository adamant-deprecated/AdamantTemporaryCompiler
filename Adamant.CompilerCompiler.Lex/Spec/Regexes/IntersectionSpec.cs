using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class IntersectionSpec : BinaryRegexSpec
	{
		public IntersectionSpec(IEnumerable<RegexSpec> expressions)
			: base(expressions)
		{
		}

		public override RegexSpec Simplify(LexerSpec lexerSpec)
		{
			// a&b&c -> !(!a|!b|!c)
			var expanded = new ComplementSpec(new AlternationSpec(Expressions.Select(exp => new ComplementSpec(exp))));
			return expanded.Simplify(lexerSpec);
		}

		public override string Operator => "&";

		public override StateRange AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			throw new NotSupportedException("Adding Intersection to NFA not supported, should be simplified away");
		}
	}
}
