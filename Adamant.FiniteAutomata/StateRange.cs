using System;
using System.Collections;
using System.Collections.Generic;

namespace Adamant.FiniteAutomata
{
	public struct StateRange : IEnumerable<State>
	{
		public readonly State Start;
		public readonly State End;

		public StateRange(State start, State end)
		{
			if(start.Index > end.Index) throw new ArgumentException("Start must be before end");
			Start = start;
			End = end;
		}

		public IEnumerator<State> GetEnumerator()
		{
			for(var i = Start.Index; i <= End.Index; i++)
			{
				yield return new State(i);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
