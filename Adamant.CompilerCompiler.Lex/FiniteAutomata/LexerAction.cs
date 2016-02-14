using System;
using System.Collections.Generic;
using System.Linq;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public class LexerAction
	{
		/// <summary>
		/// Lower numbers are higher priority
		/// </summary>
		public readonly int Priority;
		public readonly LexerInputAction InputAction;
		public readonly IReadOnlyList<LexerModeAction> ModeActions;
		public readonly LexerEmitAction EmitAction;
		public readonly string Code;

		public LexerAction(
			int priority,
			LexerInputAction inputAction,
			IEnumerable<LexerModeAction> modeActions,
			LexerEmitAction emitAction,
			string code)
		{
			if(inputAction == null) throw new ArgumentNullException(nameof(inputAction));

			Priority = priority;
			InputAction = inputAction;
			ModeActions = modeActions.ToList();
			EmitAction = emitAction;
			Code = code;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != this.GetType()) return false;
			return Equals((LexerAction) obj);
		}

		protected bool Equals(LexerAction other)
		{
			return Priority == other.Priority && Equals(InputAction, other.InputAction) && Equals(ModeActions, other.ModeActions) && Equals(EmitAction, other.EmitAction) && string.Equals(Code, other.Code);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Priority;
				hashCode = (hashCode * 397) ^ InputAction.GetHashCode();
				hashCode = (hashCode * 397) ^ ModeActions.GetHashCode();
				hashCode = (hashCode * 397) ^ EmitAction.GetHashCode();
				hashCode = (hashCode*397) ^ (Code?.GetHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}
