using Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public abstract class LexerInputAction
	{
		public static LexerInputAction Ignore { get; } = IgnoreInput.Instance;
		public static LexerInputAction Capture { get; } = CaptureInput.Instance;
		public static LexerInputAction CaptureForDecode { get; } = CaptureInputForDecode.Instance;

		public static LexerInputAction DecodeValue(int @base)
		{
			return new DecodeValue(@base);
		}

		public static LexerInputAction CaptureValue(string value)
		{
			return new CaptureValue(value);
		}
	}
}
