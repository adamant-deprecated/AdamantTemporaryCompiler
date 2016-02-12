using System.Collections;
using System.Collections.Generic;

namespace Adamant.CompilerCompiler.Lex.Runtime
{
	public abstract class Lexer<TTokenType> : IEnumerable<Token<TTokenType>>
	{
		public abstract IEnumerator<Token<TTokenType>> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
