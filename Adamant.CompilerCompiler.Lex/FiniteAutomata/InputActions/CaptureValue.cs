namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions
{
	public class CaptureValue : LexerInputAction
	{
		public readonly string Value;

		public CaptureValue(string value)
		{
			Value = value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var other = obj as CaptureValue;
			return other != null && Value == other.Value;
		}
	}
}
