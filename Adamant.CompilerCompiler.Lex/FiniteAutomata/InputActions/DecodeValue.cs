namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions
{
	public class DecodeValue : LexerValueAction
	{
		public readonly int Base;

		public DecodeValue(int @base)
		{
			Base = @base;
		}

		public override int GetHashCode()
		{
			return Base;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != GetType()) return false;
			return Equals((DecodeValue) obj);
		}

		protected bool Equals(DecodeValue other)
		{
			return Base == other.Base;
		}
	}
}
