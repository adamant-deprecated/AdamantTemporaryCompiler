using System;
using Adamant.CompilerCompiler.Lex.Spec;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata.EmitActions
{
	public class EmitToken : LexerEmitAction
	{
		public readonly Channel Channel;
		public readonly string TokenType;
		public readonly bool IsError;

		public EmitToken(Channel channel, string tokenType, bool isError)
		{
			if(channel == null) throw new ArgumentNullException(nameof(channel));
			if(string.IsNullOrWhiteSpace(tokenType)) throw new ArgumentNullException(nameof(tokenType));
			Channel = channel;
			TokenType = tokenType;
			IsError = isError;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != GetType()) return false;
			return Equals((EmitToken)obj);
		}

		protected bool Equals(EmitToken other)
		{
			return Equals(Channel, other.Channel) && TokenType == other.TokenType && IsError == other.IsError;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Channel.GetHashCode();
				hashCode = (hashCode * 397) ^ TokenType.GetHashCode();
				hashCode = (hashCode * 397) ^ IsError.GetHashCode();
				return hashCode;
			}
		}
	}
}
