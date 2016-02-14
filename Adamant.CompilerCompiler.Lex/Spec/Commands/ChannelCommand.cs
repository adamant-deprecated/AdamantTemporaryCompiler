using System;

namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class ChannelCommand : Command
	{
		public readonly Channel Channel;

		public ChannelCommand(Channel channel)
		{
			if(channel == null) throw new ArgumentNullException(nameof(channel));
			Channel = channel;
		}

		public override Channel ChannelUsed()
		{
			return Channel;
		}

		public override void Validate(RuleSpec rule, LexerSpec lexer)
		{
			if(!lexer.Channels.Contains(Channel))
				throw new Exception($"Rule '{rule.Name}' references channel not in lexer spec: '{Channel}'");
		}
	}
}
