using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.Core;
using Adamant.FiniteAutomata;
using Sys.Text;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class LiteralSpec : RegexSpec
	{
		public readonly string Value;

		public LiteralSpec(string value)
		{
			Value = value;
		}

		public override CharClassSpec AsCharClass()
		{
			if(Value.Length != 1 || !(Value.Length == 2 && char.IsHighSurrogate(Value[0]))) return null;

			return new CharClassSpec(new InversionListCodePointSet(CodePoint.Get(Value, 0)));
		}

		public override void MakeEquivalenceClasses(CodePointEquivalenceClasses equivalenceClasses)
		{
			equivalenceClasses.MakeClasses(Value);
		}

		public override StateRange AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			var startState = nfa.AddState();
			var endState = startState;
			foreach(var codePoint in Value.GetCodePoints())
			{
				var lastState = endState;
				endState = nfa.AddState();
				nfa.AddTransition(lastState, equivalenceClasses.GetClass(codePoint), endState);
			}
			return new StateRange(startState, endState);
		}

		public override string ToString()
		{
			return $"\"{Value}\"";
		}
	}
}
