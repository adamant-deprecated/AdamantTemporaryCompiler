using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class DifferenceSpec : BinaryRegexSpec
	{
		public DifferenceSpec(IEnumerable<RegexSpec> expressions)
			: base(expressions)
		{
		}

		public override RegexSpec Simplify(LexerSpec lexerSpec)
		{
			// a-b -> !(!a|b)
			var expanded = Expressions.Aggregate((exp1, exp2) => new ComplementSpec(new AlternationSpec(new[] { new ComplementSpec(exp1), exp2 })));
			return expanded.Simplify(lexerSpec);
		}

		public override string Operator => "-";

		public override States AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			throw new NotSupportedException("Adding Difference to NFA not supported, should be simplified away");
		}
	}
}
