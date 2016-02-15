using System;

namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class SetChannelCommand : Command
	{
		public SetChannelCommand(Channel channel)
		{
			if(channel == null) throw new ArgumentNullException(nameof(channel));
			Channel = channel;
		}

		public override Channel Channel { get; }

		public override void Validate(RuleSpec rule, LexerSpec lexer)
		{
			if(!lexer.Channels.Contains(Channel))
				throw new Exception($"Rule '{rule.Name}' references channel not in lexer spec: '{Channel}'");
		}
	}
}
