using System.Collections.Generic;
using Adamant.CompilerCompiler.Lex.Properties;

namespace Adamant.CompilerCompiler.Lex.CodeGen
{
	public class CSharpSkeleton : Skeleton
	{
		private readonly static DataType[] dataTypes =
		{
			new DataType(8,"byte"),
			new DataType(16, "ushort"),
			new DataType(32, "uint"),
			new DataType(64, "ulong"),   
		};
		public override string Template => Resources.CSharpSkeleton;

		public override IEnumerable<DataType> DataTypes => dataTypes;

		public override string Extension => "cs";
	}
}
