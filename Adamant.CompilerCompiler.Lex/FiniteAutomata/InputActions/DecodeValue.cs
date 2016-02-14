namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions
{
	public class DecodeValue : LexerInputAction
	{
		public readonly int Base;

		public DecodeValue(int @base)
		{
			Base = @base;
		}

		public override int GetHashCode()
		{
			return Base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var other = obj as DecodeValue;
			return other != null && Base == other.Base;
		}
	}
}
