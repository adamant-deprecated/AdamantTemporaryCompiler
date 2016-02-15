using System;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.InputActions
{
	public class TextValue : LexerValueAction
	{
		public readonly string Value;

		public TextValue(string value)
		{
			if(string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
			Value = value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != GetType()) return false;
			return Equals((TextValue) obj);
		}

		protected bool Equals(TextValue other)
		{
			return string.Equals(Value, other.Value);
		}
	}
}
