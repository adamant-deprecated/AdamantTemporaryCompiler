namespace Adamant.CompilerCompiler.Lex.Runtime
{
	public class Token<TChannel, TType>
	{
		public Token(TChannel channel, TType type, bool isError, FilePosition beginsAt, FilePosition endsAt, string value)
		{
			Channel = channel;
			Type = type;
			IsError = isError;
			BeginsAt = beginsAt;
			EndsAt = endsAt;
			Value = value;
		}

		public TChannel Channel { get; }
		public TType Type { get; }
		public bool IsError { get; }
		public FilePosition BeginsAt { get; }
		public FilePosition EndsAt { get; }
		public string Value { get; }
	}
}
