using System;

namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class SetTypeCommand : Command
	{
		public readonly string TokenType;

		public SetTypeCommand(string tokenType)
		{
			TokenType = tokenType;
		}

		public override void Validate(RuleSpec rule, LexerSpec lexer)
		{
			if(!lexer.Rules.Contains(TokenType))
				throw new Exception($"Rule '{rule.Name}' references type not in lexer spec: '{TokenType}'");

			if(lexer.Rules[TokenType].IsFragment)
				throw new Exception($"Rule '{rule.Name}' references fragment for token type: '{TokenType}'");
		}

		public override string GetTokenType(string tokenType)
		{
			return TokenType;
		}
	}
}
