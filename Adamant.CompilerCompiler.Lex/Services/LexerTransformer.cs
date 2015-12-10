using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.FiniteAutomata.ModeActions;
using Adamant.CompilerCompiler.Lex.Spec;
using Adamant.CompilerCompiler.Lex.Spec.Commands;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Services
{
	public class LexerTransformer
	{
		#region Singleton
		public static LexerTransformer Instance { get; } = new LexerTransformer();

		private LexerTransformer()
		{
		}
		#endregion

		public LexerNFA ConvertToNFA(LexerSpec spec)
		{
			spec = spec.HasBeenSimplified ? spec : spec.Simplify();
			var equivalenceClasses = MakeEquivalenceClasses(spec);
			var nfa = new NFA<LexerAction>(equivalenceClasses.Count);
			var modeMap = spec.Modes.ToDictionary(m => m, m => nfa.AddState(true));

			// TODO what we actually want here is something more like a didirectional dictionary?
			var tokenTypes = spec.Rules.OrderBy(r => r.Name).Select(r => r.Name).ToList();
			var tokenLookup = new Dictionary<string, int>();
			for(var i = 0; i < tokenTypes.Count; i++)
				tokenLookup.Add(tokenTypes[i], i);

			for(var i = 0; i < spec.Rules.Count; i++)
			{
				var rule = spec.Rules[i];
				var endState = AddStates(rule, modeMap, equivalenceClasses, nfa);
				var tokenType = GetTokenType(rule, tokenLookup);
				var isMore = rule.Commands.Contains(Command.More);
				var isError = rule.Commands.Contains(Command.FlagError);
				var modeActions = GetModeActions(rule.Commands);
				var code = rule.Commands.OfType<CodeActionCommand>().SingleOrDefault()?.Code;
				nfa.SetData(endState, new LexerAction(i, tokenType, isMore, isError, modeActions, code));
			}

			return new LexerNFA(modeMap, equivalenceClasses, nfa);
		}

		private static CodePointEquivalenceClasses MakeEquivalenceClasses(LexerSpec spec)
		{
			var equivalenceClasses = new CodePointEquivalenceClasses();
			foreach(var rule in spec.Rules)
				rule.Expression.MakeEquivalenceClasses(equivalenceClasses);

			return equivalenceClasses;
		}

		// TODO move to RuleSpec?
		private static State AddStates(RuleSpec rule, IDictionary<Mode, State> modeMap, CodePointEquivalenceClasses equivalenceClasses, NFA<LexerAction> nfa)
		{
			var states = rule.Expression.AddTo(nfa, equivalenceClasses);

			foreach(var mode in rule.Modes)
				nfa.AddEpsilonTransition(modeMap[mode], states.Start);

			nfa.SetFinal(states.End);
			return states.End;
			// TODO nfa.SetData(rule.Commands)
		}

		private static int? GetTokenType(RuleSpec rule, IDictionary<string, int> tokenLookup)
		{
			var tokenTypeName = rule.Commands.Aggregate(rule.Name, (current, command) => command.GetTokenType(current));
			return tokenTypeName == null ? (int?)null : tokenLookup[tokenTypeName];
		}

		private IEnumerable<LexerModeAction> GetModeActions(IReadOnlyList<Command> commands)
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

		public LexerDFA ConvertToDFA(LexerNFA lexerNFA)
		{
			var dfaResult = lexerNFA.Nfa.ToDFA(lexerActions =>
											lexerActions.Select(x => x.Item2)
											.Where(a => a != null)
											.OrderBy(a => a.Priority)
											.FirstOrDefault());
			var modeMap = lexerNFA.ModeMap.ToDictionary(e => e.Key, e => dfaResult.Item1[e.Value]);
			return new LexerDFA(modeMap, lexerNFA.EquivalenceClasses, dfaResult.Item2);
		}
	}
}
