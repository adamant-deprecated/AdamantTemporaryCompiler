namespace Adamant.CompilerCompiler.Lex.Runtime
{
	/// <summary>
	/// Represents a position in a source file. All values are zero based.  Newline chars should be
	/// reported as on the line they terminate.
	/// </summary>
	public struct FilePosition
	{
		public readonly long Offset;
		public readonly int Line;
		public readonly int Column;

		public FilePosition(long offset, int line, int column)
		{
			Offset = offset;
			Line = line;
			Column = column;
		}
	}
}
