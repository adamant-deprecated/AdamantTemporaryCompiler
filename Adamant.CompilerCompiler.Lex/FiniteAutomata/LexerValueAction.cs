using Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public abstract class LexerValueAction
	{
		public static LexerValueAction Ignore { get; } = IgnoreValue.Instance;
		public static LexerValueAction Capture { get; } = CaptureValue.Instance;
		public static LexerValueAction Decode(int @base)
		{
			return new DecodeValue(@base);
		}
		public static LexerValueAction Text(string value)
		{
			return new TextValue(value);
		}
	}
}
