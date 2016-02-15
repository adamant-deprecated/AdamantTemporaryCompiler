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
		public readonly LexerValueAction ValueAction;
		public readonly IReadOnlyList<LexerModeAction> ModeActions;
		public readonly LexerEmitAction EmitAction;
		public readonly string Code;

		public LexerAction(
			int priority,
			LexerValueAction valueAction,
			IEnumerable<LexerModeAction> modeActions,
			LexerEmitAction emitAction,
			string code)
		{
			if(valueAction == null) throw new ArgumentNullException(nameof(valueAction));

			Priority = priority;
			ValueAction = valueAction;
			ModeActions = modeActions.ToList();
			EmitAction = emitAction;
			Code = code;
		}

		public static LexerAction Default(int priority)
		{
			return new LexerAction(priority, LexerValueAction.Ignore, Enumerable.Empty<LexerModeAction>(), LexerEmitAction.More, null);
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
			return Equals(ValueAction, other.ValueAction)
				&& ModeActions.SequenceEqual(other.ModeActions)
				&& Equals(EmitAction, other.EmitAction)
				&& string.Equals(Code, other.Code);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ValueAction.GetHashCode();
				foreach(var modeAction in ModeActions)
					hashCode = (hashCode * 397) ^ modeAction.GetHashCode();
				hashCode = (hashCode * 397) ^ EmitAction.GetHashCode();
				hashCode = (hashCode * 397) ^ (Code?.GetHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}
