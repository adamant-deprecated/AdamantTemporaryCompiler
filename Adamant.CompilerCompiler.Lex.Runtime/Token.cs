namespace Adamant.CompilerCompiler.Lex.Runtime
{
	public class Token<TType>
	{
		public Token(TType type)
		{
			Type = type;
		}

		public TType Type { get; }
	}
}
