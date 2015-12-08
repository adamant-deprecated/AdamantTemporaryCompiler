using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes.Functions
{
	public class DecodeFunctionSpec : FunctionSpec
	{
		public readonly int Base;

		public DecodeFunctionSpec(RegexSpec expression, int @base)
			: base(expression)
		{
			Base = @base;
		}

		protected override RegexSpec Simplify(LexerSpec lexerSpec, RegexSpec exp, bool changed)
		{
			return changed ? new DecodeFunctionSpec(exp, Base) : this;
		}

		public override States AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			// TODO how to actually handle decode
			return Expression.AddTo(nfa, equivalenceClasses);
		}

		public override string ToString()
		{
			return $"@decode({Expression}, {Base})";
		}
	}
}
