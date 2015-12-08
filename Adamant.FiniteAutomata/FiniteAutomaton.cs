using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Adamant.FiniteAutomata
{
	/// <summary>
	/// A Finite Automaton operating over inputs
	/// </summary>
	/// <typeparam name="TData">Data to associate with each state</typeparam>
	public abstract class FiniteAutomaton<TData>
	{
		protected const int DefaultStateCapacity = 128;

		private readonly SortedSet<State> startStates = new SortedSet<State>();
		private readonly BitArray stateIsFinal;
		private TData[] stateData;

		protected FiniteAutomaton(int inputValueCount, int stateCapacity)
		{
			InputValueCount = inputValueCount;
			stateIsFinal = new BitArray(stateCapacity);
			stateData = new TData[stateCapacity];
		}

		public int InputValueCount { get; }
		public int StateCount { get; private set; }
		public int StartStateCount => startStates.Count;

		public IEnumerable<Input> Inputs => Enumerable.Range(0, InputValueCount).Select(i => new Input(i));
		public IEnumerable<State> States => Enumerable.Range(0, StateCount).Select(i => new State(i));
		public IEnumerable<State> StartStates => startStates;

		public virtual State AddState(bool startState = false)
		{
			var state = new State(StateCount++);
			if(stateIsFinal.Length < StateCount)
				stateIsFinal.Length = stateIsFinal.Length * 2;
			if(startState)
				SetStart(state);
			return state;
		}
		public State AddState(TData data, bool startState = false)
		{
			var state = AddState(startState);
			SetData(state, data);
			return state;
		}

		public void SetStart(State state, bool isStart = true)
		{
			if(isStart)
				startStates.Add(state);
			else
				startStates.Remove(state);
		}
		public bool IsStart(State state)
		{
			return startStates.Contains(state);
		}

		public void SetFinal(State state, bool isFinal = true)
		{
			stateIsFinal[state.Index] = isFinal;
		}
		public bool IsFinal(State state)
		{
			return stateIsFinal[state.Index];
		}

		public void SetData(State state, TData data)
		{
			if(stateData.Length <= state.Index)
				Array.Resize(ref stateData, stateData.Length * 2);

			stateData[state.Index] = data;
		}
		public TData GetData(State state)
		{
			return stateData.Length <= state.Index ? default(TData) : stateData[state.Index];
		}

		protected void ValidateInput(Input onInput)
		{
			if(onInput.Value < 0 || onInput.Value >= InputValueCount)
				throw new ArgumentOutOfRangeException(nameof(onInput), onInput.Value, $"must be between 0 and InputValueCount={InputValueCount}");
		}
	}
}
