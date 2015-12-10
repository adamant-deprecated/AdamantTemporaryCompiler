using System;
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
			if(ReferenceEquals(this, obj)) return true;
			var other = obj as LexerAction;
			if(other == null) return false;
			return TokenType == other.TokenType
				   && IsMore == other.IsMore
				   && IsError == other.IsError
				   && ModeActions.SequenceEqual(other.ModeActions)
				   && Code == other.Code;
		}

		public override int GetHashCode()
		{
			throw new NotSupportedException();
		}
	}
}
