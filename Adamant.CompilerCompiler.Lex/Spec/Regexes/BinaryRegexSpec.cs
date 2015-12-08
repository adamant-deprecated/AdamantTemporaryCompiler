using System.Collections.Generic;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public abstract class BinaryRegexSpec : RegexSpec
	{
		public readonly IReadOnlyList<RegexSpec> Expressions;

		protected BinaryRegexSpec(IEnumerable<RegexSpec> expressions)
		{
			Expressions = new List<RegexSpec>(expressions);
		}

		public override void Validate(LexerSpec lexerSpec)
		{
			foreach(var expression in Expressions)
				expression.Validate(lexerSpec);
		}

		public override void MakeEquivalenceClasses(CodePointEquivalenceClasses equivalenceClasses)
		{
			foreach(var expression in Expressions)
				expression.MakeEquivalenceClasses(equivalenceClasses);
		}

		public override string ToString()
		{
			return $"({string.Join(Operator, Expressions)})";
		}

		public abstract string Operator { get; }
	}
}
