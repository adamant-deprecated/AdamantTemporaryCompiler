using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public struct States
	{
		public readonly State Start;
		public readonly State End;

		public States(State start, State end)
		{
			Start = start;
			End = end;
		}
	}
}
