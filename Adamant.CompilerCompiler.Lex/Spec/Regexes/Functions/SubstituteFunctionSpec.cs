using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes.Functions
{
	public class SubstituteFunctionSpec:FunctionSpec
	{
		private readonly string value;

		public SubstituteFunctionSpec(RegexSpec expression, string value)
			: base(expression)
		{
			this.value = value;
		}

		protected override RegexSpec Simplify(LexerSpec lexerSpec, RegexSpec exp, bool changed)
		{
			return changed ? new SubstituteFunctionSpec(exp, value) : this;
		}

		public override StateRange AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			// TODO how to actually handle substitute
			return Expression.AddTo(nfa, equivalenceClasses);
		}

		public override string ToString()
		{
			return $"@sub({Expression}, {value})";
		}
	}
}
