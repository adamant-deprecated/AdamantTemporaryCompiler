using System;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;
using Sys.Text;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class UptoSpec : UnaryRegexSpec
	{
		public UptoSpec(RegexSpec expression)
			: base(expression)
		{
		}

		protected override RegexSpec Simplify(LexerSpec lexerSpec, RegexSpec exp, bool changed)
		{
			// ~a -> !([^]* a [^]*)
			var anyChar = new CharClassSpec(InversionListCodePointSet.All);
			var repetition = new RepetitionSpec(anyChar, 0, null);
			var concatenation = new ConcatenationSpec(new[] { repetition, exp, repetition });
			var expanded = new ComplementSpec(concatenation);
			return expanded.Simplify(lexerSpec);
		}

		public override States AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			throw new NotSupportedException("Adding Upto to NFA not supported, should be simplified away");
		}

		public override string ToString()
		{
			return $"~{Expression}";
		}
	}
}