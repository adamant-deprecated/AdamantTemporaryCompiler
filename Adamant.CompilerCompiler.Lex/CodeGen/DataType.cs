namespace Adamant.CompilerCompiler.Lex.CodeGen
{
	public class DataType
	{
		public readonly int Bits;
		public readonly string Name;

		public DataType(int bits, string name)
		{
			Bits = bits;
			Name = name;
		}
	}
}
