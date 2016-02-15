using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.FiniteAutomata.ModeActions;
using Adamant.CompilerCompiler.Lex.Spec;
using Adamant.Core;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex
{
	public static class Functions
	{
		public static LexerNFA ConvertToNFA(LexerSpec spec)
		{
			spec = spec.HasBeenSimplified ? spec : spec.Simplify();
			var equivalenceClasses = MakeEquivalenceClasses(spec);
			var nfa = new NFA<LexerAction>(equivalenceClasses.Count);
			var modeMap = spec.Modes.ToDictionary(m => m, m => nfa.AddState(true));

			for(var i = 0; i < spec.Rules.Count; i++)
				spec.Rules[i].AddStates(modeMap, equivalenceClasses, nfa, i, spec.DefaultChannel);

			return new LexerNFA(spec, modeMap, equivalenceClasses, nfa);
		}

		private static CodePointEquivalenceClasses MakeEquivalenceClasses(LexerSpec spec)
		{
			var equivalenceClasses = new CodePointEquivalenceClasses();
			foreach(var rule in spec.Rules)
				rule.Expression.MakeEquivalenceClasses(equivalenceClasses);

			return equivalenceClasses;
		}

		public static IEnumerable<LexerModeAction> GetModeActions(IReadOnlyList<Command> commands)
		{
			var actions = commands.SelectMany(c => c.ModeActions()).Where(a => a != null).ToList();

			// We do not check for the additional possible simplification of
			// push, set(x), push, set(x) -> push, set(x), push

			// TODO change this to something more object oriented
			for(var i = actions.Count - 2; i >= 0; i--)
			{
				var action = actions[i];
				var nextAction = actions[i + 1];
				if(action is SetMode && (nextAction is PopMode || nextAction is SetMode))
				{
					//	set(x), pop -> pop
					//	set(x), set(y) -> set(y)
					actions.RemoveAt(i);
				}
				else if(action is PushMode && nextAction is PopMode)
				{
					//	push, pop -> nothing
					actions.RemoveAt(i);
					actions.RemoveAt(i);
				}
			}

			return actions;
		}

		public static LexerDFA ConvertToDFA(LexerNFA lexerNFA)
		{
			var dfaResult = lexerNFA.Nfa.ToDFA(MergeData);
			var modeMap = lexerNFA.ModeMap.ToDictionary(e => e.Key, e => dfaResult.Item1[e.Value]);
			return new LexerDFA(lexerNFA.LexerSpec, modeMap, lexerNFA.EquivalenceClasses, dfaResult.Item2);
		}

		private static LexerAction MergeData(IEnumerable<Tuple<State, LexerAction>> pairs)
		{
			var actions = pairs.Select(x => x.Item2).Where(a => a != null).ToList();
			var highestPriorty = actions.Min(a => (int?)a.Priority);
			return actions.SingleOrDefault(a => a.Priority == highestPriorty);
		}

		public static LexerCodeGenerator ConvertToCodeGenerator(LexerDFA lexerDfa)
		{
			var errorState = lexerDfa.Dfa.MakeComplete();

			// TODO Reduce Columns (covers equivalent inputs)
			// TODO Reduce Rows (is this needed? shouldn't this be gotten by DFA min? Perhaps if we match the same token in two diff modes?)
			// TODO Use column map when building equivalence table
			var optimizedEquivalenceTable = Optimize(GenEquivalenceTable(lexerDfa.EquivalenceClasses));
			// TODO It would be great if we had an algo for real table compression like described in Dragon book, but can't find out what that is yet

			var dfa = lexerDfa.Dfa;

			var rowMap = new int[dfa.StateCount];
			var transitions = new int[dfa.StateCount * dfa.InputValueCount];
			var actionMap = new int[dfa.StateCount];
			var actionIndexes = new Dictionary<LexerAction, int>();
			var nextActionIndex = 1;
			var row = 0;
			foreach(var state in dfa.States)
			{
				// Since we aren't reducing rows yet, each row is just 1-to-1 with the input length
				var rowOffset = row * dfa.InputValueCount;
				rowMap[row] = rowOffset;

				var column = 0;
				foreach(var input in dfa.Inputs)
				{
					transitions[rowOffset + column] = dfa.GetTransition(state, input)?.Index ?? -1; // Since we made it complete, -1 should never be output
					column++;
				}

				var action = dfa.GetData(state);
				if(action != null)
				{
					if(!actionIndexes.ContainsKey(action))
					{
						actionIndexes.Add(action, nextActionIndex);
						nextActionIndex++;
					}
					actionMap[row] = actionIndexes[action];
				}
				else
					actionMap[row] = 0;

				row++;
			}

			var actions = new[] { default(LexerAction) }.Concat(actionIndexes.OrderBy(x => x.Value).Select(x => x.Key)).ToArray();

			return new LexerCodeGenerator(lexerDfa.LexerSpec, lexerDfa.ModeMap, errorState.Index, optimizedEquivalenceTable.Item1, optimizedEquivalenceTable.Item2, rowMap, transitions, actionMap, actions);
		}

		/// <summary>
		/// These are the unicode planes that contain assigned code points as of Unicode 8.0
		/// </summary>
		private static readonly int[] UnicodePlanes = { 0, 1, 2, 14, 15, 16 };

		private static IDictionary<int, Input[]> GenEquivalenceTable(CodePointEquivalenceClasses equivalenceClasses)
		{
			var equivalenceTable = new Dictionary<int, Input[]>();
			foreach(var plane in UnicodePlanes)
			{
				var planeEquivalenceClasses = new Input[0x10000];
				equivalenceTable.Add(plane, planeEquivalenceClasses);
				var planeBits = plane << 16;
				for(var planePlace = 0; planePlace < 0x10000; planePlace++)
					planeEquivalenceClasses[planePlace] = equivalenceClasses.GetClass((CodePoint)(planeBits + planePlace));
			}
			return equivalenceTable;
		}

		/// <summary>
		/// This is a fairly simple optimization strategy. We're just going to lay the blocks out in order.
		/// For each block we'll try to find the first place it could fit, which may just be the end.
		/// </summary>
		private static Tuple<IDictionary<int, int[]>, IList<Input>> Optimize(IDictionary<int, Input[]> fullEquivalenceTable)
		{
			var equivalenceTable = new List<Input>();
			var planeOffsets = new Dictionary<int, int[]>();
			foreach(var plane in UnicodePlanes)
			{
				var planeEquivalenceClasses = fullEquivalenceTable[plane];
				if(AllEqual(planeEquivalenceClasses))
				{
					// Optimize the case where the whole plane is the same equivalence class,
					// This is represented as a single element array in place of the offsets
					planeOffsets.Add(plane, new[] { planeEquivalenceClasses[0].Value });
				}
				else
				{
					var blockOffsets = new int[256];
					planeOffsets.Add(plane, blockOffsets);
					for(var block = 0; block < 256; block++)
						for(var place = 0; place < equivalenceTable.Count + 1; place++)
							if(CanPlaceBlockAt(planeEquivalenceClasses, block, place, equivalenceTable))
							{
								blockOffsets[block] = place;
								// We may need to copy the end of the block into the equivalenceTable
								var copyCount = (place + 256) - equivalenceTable.Count;
								if(copyCount > 0)
									equivalenceTable.AddRange(new ArraySegment<Input>(planeEquivalenceClasses, (block << 8) + 256 - copyCount, copyCount));
								break; // We may have just made equivalenceTable larger and it will want to continue the loop
							}

				}
			}
			return Tuple.Create<IDictionary<int, int[]>, IList<Input>>(planeOffsets, equivalenceTable);
		}

		private static bool CanPlaceBlockAt(Input[] planeEquivalenceClasses, int block, int place, IList<Input> equivalenceTable)
		{
			var blockStart = block << 8;
			// Since the block place could take us off the end of the equivalenceTable, we may only need to check a portion of the block
			var blockPortion = Math.Min(0xFF, equivalenceTable.Count - place - 1);
			var blockEnd = blockStart + blockPortion;
			for(var blockChar = blockStart; blockChar <= blockEnd; blockChar++, place++)
				if(equivalenceTable[place] != planeEquivalenceClasses[blockChar])
					return false;

			return true;
		}

		private static bool AllEqual(Input[] planeEquivalenceClasses)
		{
			var value = planeEquivalenceClasses[0];
			for(var i = 1; i < planeEquivalenceClasses.Length; i++)
				if(value != planeEquivalenceClasses[i])
					return false;

			return true;
		}
	}
}
