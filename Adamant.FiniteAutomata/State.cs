using System;

namespace Adamant.FiniteAutomata
{
	/// <summary>
	/// An opaque reference to a state of a Finite Automaton.  The downside of using a struct is that
	/// the default value is a valid state reference.  Do Not:
	///		* Use the default value
	///		* Use states from one finite automaton in another
	/// This type does not validate that it is being used with the source FA.
	/// </summary>
	public struct State : IComparable<State>
	{
		public readonly int Index;

		internal State(int index)
		{
			Index = index;
		}

		public int CompareTo(State other)
		{
			return Index.CompareTo(other.Index);
		}

		public override string ToString()
		{
			return $"State {Index}";
		}
	}
}
