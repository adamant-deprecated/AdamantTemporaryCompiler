namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.EmitActions
{
	public class EmitToken : LexerEmitAction
	{
		public readonly int Channel;
		public readonly int TokenType;
		public readonly bool IsError;

		public EmitToken(int channel, int tokenType, bool isError)
		{
			Channel = channel;
			TokenType = tokenType;
			IsError = isError;
		}

		protected bool Equals(EmitToken other)
		{
			return Channel == other.Channel && TokenType == other.TokenType && IsError == other.IsError;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != GetType()) return false;
			return Equals((EmitToken)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Channel;
				hashCode = (hashCode * 397) ^ TokenType;
				hashCode = (hashCode * 397) ^ IsError.GetHashCode();
				return hashCode;
			}
		}
	}
}
