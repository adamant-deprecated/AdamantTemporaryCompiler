using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.Spec.Regexes.Functions;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public abstract class RegexSpec
	{
		public virtual void Validate(LexerSpec lexerSpec)
		{
		}

		public virtual RegexSpec Simplify(LexerSpec lexerSpec)
		{
			return this;
		}

		public virtual CharClassSpec AsCharClass()
		{
			return null;
		}

		public abstract void MakeEquivalenceClasses(CodePointEquivalenceClasses equivalenceClasses);

		public abstract States AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses);

		#region Operators // TODO this should really be some other builder type or something
		public static implicit operator RegexSpec(string literal)
		{
			return new LiteralSpec(literal);
		}

		public static RegexSpec operator +(RegexSpec left, RegexSpec right)
		{
			return new ConcatenationSpec(new[] { left, right });
		}

		public static RegexSpec operator |(RegexSpec left, RegexSpec right)
		{
			return new AlternationSpec(new[] { left, right });
		}

		public static RegexSpec operator ~(RegexSpec regex)
		{
			return new UptoSpec(regex);
		}

		public static RegexSpec operator !(RegexSpec regex)
		{
			return new ComplementSpec(regex);
		}

		public RegexSpec Optional()
		{
			return new RepetitionSpec(this, 0, 1);
		}

		public RegexSpec RepeatAtLeast(int min)
		{
			return new RepetitionSpec(this, min, null);
		}

		public RegexSpec Repeat()
		{
			return new RepetitionSpec(this, 0, null);
		}

		public RegexSpec Repeat(int n)
		{
			return new RepetitionSpec(this, n, n);
		}

		public RegexSpec Repeat(int min, int max)
		{
			return new RepetitionSpec(this, min, max);
		}

		public RegexSpec Capture()
		{
			return new CaptureFunctionSpec(this);
		}

		public RegexSpec Decode(int @base)
		{
			return new DecodeFunctionSpec(this, @base);
		}

		public RegexSpec Sub(string value)
		{
			return new SubstituteFunctionSpec(this, value);
		}
		#endregion
	}
}
