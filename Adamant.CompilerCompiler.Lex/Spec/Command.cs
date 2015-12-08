using Adamant.CompilerCompiler.Lex.Spec.Commands;

namespace Adamant.CompilerCompiler.Lex.Spec
{
	public class Command
	{
		public static readonly Command Skip = new Command();
		public static readonly Command More = new Command();
		public static readonly Command FlagError = new Command();
		public static Command PushMode(Mode mode)
		{
			return new PushModeCommand(mode);
		}
		public static Command PopMode(Mode mode)
		{
			return new PopModeCommand(mode);
		}

		public virtual Mode ModeEntered()
		{
			return null;
		}
	}
}
