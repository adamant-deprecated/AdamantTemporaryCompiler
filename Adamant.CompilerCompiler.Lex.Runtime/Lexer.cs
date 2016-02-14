using System.Collections;
using System.Collections.Generic;

namespace Adamant.CompilerCompiler.Lex.Runtime
{
	public abstract class Lexer<TChannel, TTokenType> : IEnumerable<Token<TChannel, TTokenType>>
	{
		public abstract IEnumerator<Token<TChannel, TTokenType>> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
