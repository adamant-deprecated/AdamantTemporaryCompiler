using System;
using System.Collections.Generic;
using System.Linq;

namespace Adamant.FiniteAutomata
{
	public class NFA<TData> : FiniteAutomaton<TData>
	{
		private StateSet[][] transitions;
		private StateSet[] epsilonTransitions;

		public NFA(int inputValueCount, int stateCapacity = DefaultStateCapacity)
			: base(inputValueCount, stateCapacity)
		{
			transitions = new StateSet[stateCapacity][];
			epsilonTransitions = new StateSet[stateCapacity];
		}

		public override State AddState(bool startState = false)
		{
			var state = base.AddState(startState);
			if(transitions.Length < StateCount)
				Array.Resize(ref transitions, transitions.Length * 2);
			transitions[state.Index] = new StateSet[InputValueCount];
			if(epsilonTransitions.Length < StateCount)
				Array.Resize(ref epsilonTransitions, epsilonTransitions.Length * 2);
			return state;
		}

		public void AddEpsilonTransition(State fromState, State toState)
		{
			var stateSet = EpsilonTransitions(fromState);
			stateSet.AddState(toState);
		}

		private StateSet EpsilonTransitions(State fromState)
		{
			var stateSet = epsilonTransitions[fromState.Index];
			if(stateSet == null)
				stateSet = epsilonTransitions[fromState.Index] = new StateSet(StateCount);
			return stateSet;
		}

		public void AddTransition(State fromState, Input onInput, State toState)
		{
			ValidateInput(onInput);
			Transitions(fromState, onInput).AddState(toState);
		}

		private StateSet Transitions(State fromState, Input onInput)
		{
			var stateSet = transitions[fromState.Index][onInput.Value];
			if(stateSet == null)
				stateSet = transitions[fromState.Index][onInput.Value] = new StateSet(StateCount);
			return stateSet;
		}

		#region ToDFA
		public Tuple<IReadOnlyDictionary<State, State>, DFA<TData>> ToDFA(Func<IEnumerable<Tuple<State, TData>>, TData> mergeData)
		{
			// Fill in all the epsilon transitions, so we don't have to constantly take the closure
			EpsilonFill();

			// From now on, we won't modify the nfa at all

			var dfa = new DFA<TData>(InputValueCount);

			var dfaStates = new Dictionary<StateSet, State>();
			var nfaStateSets = new List<StateSet>();

			var startStateMap = new Dictionary<State, State>();

			// Add states for our start states
			foreach(var startState in StartStates)
			{
				// TODO could two start states be equivalent?
				var stateSet = EpsilonTransitions(startState); // A state is always in its own epsilon closure
				var dfaState = dfa.AddState(GetData(stateSet, mergeData), true);
				if(stateSet.Any(IsFinal))
					dfa.SetFinal(dfaState);

				nfaStateSets.Add(stateSet);
				dfaStates.Add(stateSet, dfaState);

				startStateMap.Add(startState, dfaState);
			}

			// Follow the DFA states to their conclusions
			// Note that we are adding states as we go and then need to look at those too
			for(var i = 0; i < dfa.StateCount; i++)
			{
				var dfaState = new State(i);
				var nfaStateSet = nfaStateSets[dfaState.Index];
				foreach(var input in Inputs)
				{
					var toNFAStateSet = DFAEdge(nfaStateSet, input);
					if(toNFAStateSet.IsEmpty()) continue;
					State toDfaState;
					if(!dfaStates.TryGetValue(toNFAStateSet, out toDfaState))
					{
						toDfaState = dfa.AddState(GetData(toNFAStateSet, mergeData));
						if(toNFAStateSet.Any(IsFinal))
							dfa.SetFinal(toDfaState);

						nfaStateSets.Add(toNFAStateSet);
						dfaStates.Add(toNFAStateSet, toDfaState);
					}
					dfa.SetTransition(dfaState, input, toDfaState);
				}
			}

			return Tuple.Create((IReadOnlyDictionary<State, State>)startStateMap, dfa);
		}

		/// <summary>
		/// Fill in all implied epsilon edges so we don't have to recompute closure later (i.e. a -> b -> c implies a -> c)
		/// </summary>
		private void EpsilonFill()
		{
			foreach(var state in States)
				EpsilonTransitions(state).UnionWith(EpsilonClosure(state));
		}

		/// <summary>
		/// The closure of states reachable from the given state by epsilon transitions
		/// </summary>
		private StateSet EpsilonClosure(State state)
		{
			var closure = new StateSet(StateCount);
			closure.AddState(state); // A state is always in its own closure
			var unvisited = new StateSet(StateCount);
			unvisited.AddState(state);
			State unvisitedState;
			while(unvisited.TryRemove(out unvisitedState))
			{
				var reachableStates = EpsilonTransitions(unvisitedState);
				unvisited.UnionWith(reachableStates.Subtract(closure));
				closure.UnionWith(reachableStates);
			}
			return closure;
		}

		private StateSet DFAEdge(StateSet currentState, Input input)
		{
			var directStates = new StateSet(StateCount);

			foreach(var state in currentState)
				directStates.UnionWith(Transitions(state, input));

			var closure = directStates.Clone();
			foreach(var state in directStates)
				closure.UnionWith(EpsilonTransitions(state));

			return closure;
		}

		private TData GetData(StateSet currentState, Func<IEnumerable<Tuple<State, TData>>, TData> mergeData)
		{
			return mergeData(currentState.Select(state => Tuple.Create(state, GetData(state))));
		}
	}
	#endregion
}
