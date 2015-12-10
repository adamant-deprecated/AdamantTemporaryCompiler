using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class PopModeCommand : Command
	{
		#region Singleton
		public static Command Instance { get; } = new PopModeCommand();

		private PopModeCommand()
		{
		}
		#endregion

		public override IEnumerable<LexerModeAction> ModeActions()
		{
			return Enumerable.Repeat(LexerModeAction.Pop, 1);
		}
	}
}
