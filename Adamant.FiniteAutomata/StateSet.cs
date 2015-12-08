using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Adamant.FiniteAutomata
{
	internal class StateSet : IEnumerable<State>
	{
		private readonly BitArray states;

		public StateSet(int capacity)
		{
			states = new BitArray(capacity);
		}

		private StateSet(BitArray states)
		{
			this.states = states;
		}

		private void EnsureLength(StateSet other)
		{
			var length = Math.Max(states.Length, other.states.Length);
			states.Length = length;
			other.states.Length = length;
		}

		private void EnsureLength(State state)
		{
			if(states.Length <= state.Index) states.Length = state.Index + 1;
		}

		public void AddState(State state)
		{
			EnsureLength(state);
			states.Set(state.Index, true);
		}

		public bool Contains(State state)
		{
			EnsureLength(state);
			return states[state.Index];
		}

		public bool TryRemove(out State state)
		{
			for(var i = 0; i < states.Count; i++)
				if(states[i])
				{
					state = new State(i);
					states.Set(i, false);
					return true;
				}

			state = new State(-1);
			return false;
		}

		public void UnionWith(StateSet other)
		{
			EnsureLength(other);
			states.Or(other.states);
		}

		[Pure]
		public StateSet Subtract(StateSet other)
		{
			EnsureLength(other);
			// BitArray provides no clone, and no non-side effecting methods
			var bits = new BitArray(states.Length);
            return new StateSet(bits.Or(other.states).Not().And(states));
		}

		public IEnumerator<State> GetEnumerator()
		{
			for(var i = 0; i < states.Count; i++)
				if(states[i])
					yield return new State(i);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return states.GetEnumerator();
		}

		public bool IsEmpty()
		{
			return !this.Any();
		}

		public StateSet Clone()
		{
			// BitArray provides no clone, and no non-side effecting methods
			return new StateSet(new BitArray(states.Length).Or(states));
		}

		public override int GetHashCode()
		{
			// It burns us precious
			var hc = 0;
			foreach(var state in this)
				hc = unchecked(hc * 31 + state.Index);

			return hc;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(this, obj)) return true;
			var other = obj as StateSet;
			if(other == null) return false;

			// It burns us precious
			EnsureLength(other);
			for(var i = 0; i < states.Length; i++)
				if(states[i] != other.states[i])
					return false;

			return true;
		}

		public override string ToString()
		{
			return string.Join(", ", this);
		}
	}
}
