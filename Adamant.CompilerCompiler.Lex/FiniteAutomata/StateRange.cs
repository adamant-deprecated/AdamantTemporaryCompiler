using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public struct StateRange
	{
		public readonly State Start;
		public readonly State End;

		public StateRange(State start, State end)
		{
			Start = start;
			End = end;
		}
	}
}
