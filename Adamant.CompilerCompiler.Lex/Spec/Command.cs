using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.FiniteAutomata;
using Adamant.CompilerCompiler.Lex.Spec.Commands;

namespace Adamant.CompilerCompiler.Lex.Spec
{
	public abstract class Command
	{
		#region Command Factory Methods
		public static readonly Command Skip = SkipCommand.Instance;
		public static readonly Command More = MoreCommand.Instance;
		public static readonly Command FlagError = FlagErrorCommand.Instance;
		public static readonly Command PopMode = PopModeCommand.Instance;

		public static Command PushMode(Mode mode)
		{
			return new PushModeCommand(mode);
		}
		public static Command SetMode(Mode mode)
		{
			return new SetModeCommand(mode);
		}
		public static Command SetType(string tokenType)
		{
			return new SetTypeCommand(tokenType);
		}
		public static Command CodeAction(string code)
		{
			return new CodeActionCommand(code);
		}
		#endregion

		public virtual Mode ModeEntered()
		{
			return null;
		}

		public virtual void Validate(RuleSpec rule, LexerSpec lexer)
		{
		}

		public virtual string GetTokenType(string tokenType)
		{
			return tokenType;
		}

		public virtual IEnumerable<LexerModeAction> ModeActions()
		{
			return Enumerable.Empty<LexerModeAction>();
		}
	}
}
