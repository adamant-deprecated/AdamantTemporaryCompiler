using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.FiniteAutomata.Minimize;

namespace Adamant.FiniteAutomata
{
	public class DFA<TData> : FiniteAutomaton<TData>
	{
		private State?[][] transitions;

		public DFA(int inputValueCount, int stateCapacity = DefaultStateCapacity)
			: base(inputValueCount, stateCapacity)
		{
			transitions = new State?[stateCapacity][];
		}

		public override State AddState(bool startState = false)
		{
			var state = base.AddState(startState);
			if(transitions.Length < StateCount)
				Array.Resize(ref transitions, transitions.Length * 2);
			transitions[state.Index] = new State?[InputValueCount];
			return state;
		}

		public void SetTransition(State fromState, Input onInput, State toState)
		{
			ValidateInput(onInput);
			transitions[fromState.Index][onInput.Value] = toState;
		}

		public State? GetTransition(State state, Input onInput)
		{
			ValidateInput(onInput);
			return transitions[state.Index][onInput.Value];
		}

		public void MakeComplete()
		{
			var error = AddErrorState();
			for(var state = 0; state < StateCount; state++)
				for(var input = 0; input < InputValueCount; input++)
					if(transitions[state][input] == null)
						transitions[state][input] = error;
		}

		public State AddErrorState()
		{
			var state = AddState();
			var stateTransitions = transitions[state.Index];
			for(var input = 0; input < stateTransitions.Length; input++)
				stateTransitions[input] = state;

			return state;
		}

		public void Minimize()
		{
			var transitionList = MakeTransitionsList();
			var blocks = new Partition(StateCount);

			// Reachable from start
			foreach(var startState in StartStates)
				blocks.Mark(startState.Index);

			DiscardNotReachable(blocks, transitionList, t => t.From, t => t.To);

			// Reachable from final
			foreach(var finalState in States.Where(IsFinal))
				blocks.Mark(finalState.Index);

			DiscardNotReachable(blocks, transitionList, t => t.To, t => t.From);

			// Split final states
			foreach(var finalState in States.Where(IsFinal))
				blocks.Mark(finalState.Index);

			blocks.SplitSets();

			// TODO split final states based on action

			// Cords partition to manage transitions
			var cords = new Partition(transitionList.Count);

			// Sort transitions by input
			cords.PartitionBy(transition => transitionList[transition].OnInput);

			//Split blocks and cords
			var adjacentTransitions = new AdjacentTransitions(StateCount, transitionList, t => t.To);
			var blockSet = 1;
			for(var cordSet = 0; cordSet < cords.SetCount; cordSet++)
			{
				foreach(var transition in cords.Set(cordSet))
					blocks.Mark(transitionList[transition].From);

				blocks.SplitSets();

				for(; blockSet < blocks.SetCount; blockSet++)
				{
					foreach(var state in blocks.Set(blockSet))
						foreach(var transition in adjacentTransitions[state])
							cords.Mark(transition);

					cords.SplitSets();
				}
			}

			// TODO minimize input classes?

			// TODO output minimized DFA or modify current DFA
		}

		private List<Transition> MakeTransitionsList()
		{
			var transitionList = new List<Transition>();
			for(var from = 0; @from < StateCount; @from++)
				for(var input = 0; input < InputValueCount; input++)
				{
					var to = transitions[@from][input];
					if(to != null)
						transitionList.Add(new Transition(@from, input, to.Value.Index));
				}
			return transitionList;
		}

		private void DiscardNotReachable(Partition blocks, List<Transition> transitionList, Func<Transition, int> getFrom, Func<Transition, int> getTo)
		{
			var adjacentTransitions = new AdjacentTransitions(StateCount, transitionList, getFrom);

			foreach(var state in blocks.Marked(0))
				foreach(var transition in adjacentTransitions[state])
					blocks.Mark(getTo(transitionList[transition]));

			blocks.DiscardUnmarked();

			transitionList.RemoveAll(transition => blocks.SetOf(getFrom(transition)) == -1);
		}
	}
}
