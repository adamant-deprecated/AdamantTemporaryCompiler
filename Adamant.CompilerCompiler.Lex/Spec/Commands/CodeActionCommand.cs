using System;

namespace Adamant.CompilerCompiler.Lex.Spec.Commands
{
	public class CodeActionCommand : Command
	{
		public readonly string Code;

		public CodeActionCommand(string code)
		{
			if(code == null) throw new ArgumentNullException(nameof(code));
			Code = code;
		}
	}
}
