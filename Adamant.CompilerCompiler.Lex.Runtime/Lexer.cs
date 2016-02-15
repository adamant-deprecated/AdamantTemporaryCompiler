using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Adamant.CompilerCompiler.Lex.Runtime
{
	public abstract class Lexer<TChannel, TTokenType> : IEnumerable<Token<TChannel, TTokenType>>
	{
		public abstract IEnumerator<Token<TChannel, TTokenType>> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected void AppendCodePoint(StringBuilder builder, int codePoint)
		{
			if(codePoint <= 0xFFFF)
				builder.Append((char) codePoint);
			else
				builder.Append(char.ConvertFromUtf32(codePoint));
		}
	}
}
