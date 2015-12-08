using Adamant.CompilerCompiler.Lex.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public abstract class UnaryRegexSpec : RegexSpec
	{
		public readonly RegexSpec Expression;

		protected UnaryRegexSpec(RegexSpec expression)
		{
			Expression = expression;
		}

		public override void Validate(LexerSpec lexerSpec)
		{
			Expression.Validate(lexerSpec);
		}

		public override RegexSpec Simplify(LexerSpec lexerSpec)
		{
			var simplifiedExpression = Expression.Simplify(lexerSpec);
			return Simplify(lexerSpec, simplifiedExpression, simplifiedExpression != Expression);
		}

		public override void MakeEquivalenceClasses(CodePointEquivalenceClasses equivalenceClasses)
		{
			Expression.MakeEquivalenceClasses(equivalenceClasses);
		}

		protected abstract RegexSpec Simplify(LexerSpec lexerSpec, RegexSpec exp, bool changed);
	}
}
