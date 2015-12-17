using System;
using System.Collections.Generic;

namespace Adamant.CompilerCompiler.Lex.CodeGen
{
	public abstract class Skeleton
	{
		public abstract string Template { get; }
		public abstract IEnumerable<DataType> DataTypes { get; }

		public abstract string Extension { get; }
	}
}
