using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.FiniteAutomata;
using Sys.Text;

namespace Adamant.CompilerCompiler.Lex.Spec.Regexes
{
	public class CharClassSpec : RegexSpec
	{
		public readonly InversionListCodePointSet CodePoints;

		public CharClassSpec(InversionListCodePointSet codePoints)
		{
			CodePoints = codePoints;
		}

		public override CharClassSpec AsCharClass()
		{
			return this;
		}

		public override void MakeEquivalenceClasses(CodePointEquivalenceClasses equivalenceClasses)
		{
			equivalenceClasses.MakeClasses(CodePoints);
		}

		public override States AddTo<T>(NFA<T> nfa, CodePointEquivalenceClasses equivalenceClasses)
		{
			var startState = nfa.AddState();
			var endState = nfa.AddState();

			foreach(var equivalenceClass in equivalenceClasses.GetClasses(CodePoints))
				nfa.AddTransition(startState, equivalenceClass, endState);

			return new States(startState, endState);
		}
	}
}
