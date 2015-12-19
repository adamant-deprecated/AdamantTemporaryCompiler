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

		public State MakeComplete()
		{
			var error = AddErrorState();
			for(var state = 0; state < StateCount; state++)
				for(var input = 0; input < InputValueCount; input++)
					if(transitions[state][input] == null)
						transitions[state][input] = error;

			return error;
		}

		public State AddErrorState()
		{
			var state = AddState();
			var stateTransitions = transitions[state.Index];
			for(var input = 0; input < stateTransitions.Length; input++)
				stateTransitions[input] = state;

			return state;
		}

		public Tuple<IReadOnlyDictionary<State, State>, DFA<TData>> Minimize()
		{
			return Minimize((x, y) => true, (x, y) => x);
		}

		public Tuple<IReadOnlyDictionary<State, State>, DFA<TData>> Minimize(Func<State, State, bool> areEquivalent, Func<TData, TData, TData> combine)
		{
			var transitionList = MakeTransitionsList();
			var blocks = new Partition(StateCount);

			// Reachable from start
			foreach(var startState in StartStates)
				blocks.Mark(startState.Index);

			DiscardNotReachable(blocks, transitionList, t => t.From, t => t.To);

			// Reachable from final
			var finalStates = States.Where(IsFinal).ToList();
			foreach(var finalState in finalStates)
				blocks.Mark(finalState.Index);

			DiscardNotReachable(blocks, transitionList, t => t.To, t => t.From);

			// Split final states from non-final
			foreach(var finalState in finalStates)
				blocks.Mark(finalState.Index);

			blocks.SplitSets();

			// Split final states from other non-equivalent final states
			var possibleEquivalentStates = finalStates;
			while(possibleEquivalentStates.Any())
			{
				var state = possibleEquivalentStates.First();
				var equivalentStatesLookup = possibleEquivalentStates.Skip(1).ToLookup(s => areEquivalent(state, s));

				// Mark states
				blocks.Mark(state.Index);
				foreach(var finalState in equivalentStatesLookup[true])
					blocks.Mark(finalState.Index);

				blocks.SplitSets();

				possibleEquivalentStates = equivalentStatesLookup[false].ToList();
			}

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

			// Generate minimized DFA
			var minDFA = new DFA<TData>(InputValueCount, blocks.SetCount);

			// Create states
			for(var set = 0; set < blocks.SetCount; set++)
			{
				var data = blocks.Set(set).Select(s => GetData(new State(s))).Aggregate(combine);
				var state = minDFA.AddState(data);
				// Sets are either all final or non-final states
				if(IsFinal(new State(blocks.SomeElementOf(set))))
					minDFA.SetFinal(state);
			}

			// Create Start State Map
			var startStateMap = (IReadOnlyDictionary<State, State>)StartStates.ToDictionary(s => s, s => new State(blocks.SetOf(s.Index)));

			// Mark Start States
			foreach(var startState in startStateMap.Values)
				minDFA.SetStart(startState);

			// Create transitions
			for(var set = 0; set < cords.SetCount; set++)
			{
				var transition = transitionList[cords.SomeElementOf(set)];
				var from = new State(blocks.SetOf(transition.From));
				var to = new State(blocks.SetOf(transition.To));
				minDFA.SetTransition(from, new Input(transition.OnInput), to);
			}

			return Tuple.Create(startStateMap, minDFA);
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
