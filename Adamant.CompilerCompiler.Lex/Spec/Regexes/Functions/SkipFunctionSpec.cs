using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes.Functions
{
	public class SkipFunctionSpec : FunctionSpec
	{
		public SkipFunctionSpec(RegexSpec exp)
			: base(exp)
		{
		}

		protected override RegexSpec Simplify(LexerSpec lexerSpec, RegexSpec exp, bool changed)
		{
			return changed ? new SkipFunctionSpec(exp) : this;
		}

		public override States AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			// TODO how to actually handle skip
			return Expression.AddTo(nfa, equivalenceClasses);
		}

		public override string ToString()
		{
			return $"@skip({Expression})";
		}
	}
}
