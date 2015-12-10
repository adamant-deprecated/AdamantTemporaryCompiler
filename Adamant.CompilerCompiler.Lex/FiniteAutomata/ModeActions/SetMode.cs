using Adamant.CompilerCompiler.Lex.Spec;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.ModeActions
{
	public class SetMode : LexerModeAction
	{
		public readonly Mode Mode;

		public SetMode(Mode mode)
		{
			Mode = mode;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(this, obj)) return true;
			var other = obj as SetMode;
			return other != null && Mode.Equals(other.Mode);
		}

		public override int GetHashCode()
		{
			return Mode.GetHashCode();
		}
	}
}
