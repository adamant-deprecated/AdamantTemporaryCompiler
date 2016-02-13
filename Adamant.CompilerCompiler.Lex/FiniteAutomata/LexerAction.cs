using System.Collections.Generic;
using System.Linq;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public class LexerAction
	{
		public readonly int Priority;
		public readonly int? TokenType;
		public readonly bool IsMore;
		public readonly bool IsError;
		public readonly IReadOnlyList<LexerModeAction> ModeActions;
		public readonly string Code;

		public LexerAction(int priority, int? tokenType, bool isMore, bool isError, IEnumerable<LexerModeAction> modeActions, string code)
		{
			Priority = priority;
			TokenType = tokenType;
			IsError = isError;
			ModeActions = modeActions.ToList();
			Code = code;
			IsMore = isMore;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != GetType()) return false;
			return Equals((LexerAction)obj);
		}

		protected bool Equals(LexerAction other)
		{
			return TokenType == other.TokenType
				&& IsMore == other.IsMore
				&& IsError == other.IsError
				&& ModeActions.SequenceEqual(other.ModeActions)
				&& Code == other.Code;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = TokenType.GetHashCode();
				hashCode = (hashCode * 397) ^ IsMore.GetHashCode();
				hashCode = (hashCode * 397) ^ IsError.GetHashCode();
				foreach(var modeAction in ModeActions)
					hashCode = (hashCode * 397) ^ modeAction.GetHashCode();

				hashCode = (hashCode * 397) ^ (Code?.GetHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}
