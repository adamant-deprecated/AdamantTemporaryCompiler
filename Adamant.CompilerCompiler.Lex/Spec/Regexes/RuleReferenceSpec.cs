using System;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.Services;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class RuleReferenceSpec : RegexSpec
	{
		public readonly string RuleName;

		public RuleReferenceSpec(string ruleName)
		{
			RuleName = ruleName;
		}

		public override void Validate(LexerSpec lexerSpec)
		{
			if(!lexerSpec.Rules.Contains(RuleName))
				throw new Exception($"Reference to rule '{RuleName}' that isn't in lexer spec.");
		}

		public override RegexSpec Simplify(LexerSpec lexerSpec)
		{
			return lexerSpec.Rules[RuleName].Expression.Simplify(lexerSpec);
		}

		public override void MakeEquivalenceClasses(CodePointEquivalenceClasses equivalenceClasses)
		{
			throw new NotSupportedException("RuleReference should be simplified away before making equivalence classes");
		}

		public override States AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			throw new NotSupportedException("Adding RuleReference to NFA not supported, should be simplified away");
		}

		public override string ToString()
		{
			return RuleName;
		}
	}
}
