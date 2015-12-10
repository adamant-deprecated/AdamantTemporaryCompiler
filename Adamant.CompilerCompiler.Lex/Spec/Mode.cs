using System;

namespace Adamant.CompilerCompiler.Lex.Spec
{
	public class Mode
	{
		public readonly string Name;

		public Mode(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));
			Name = name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var other = obj as Mode;
			return other != null && Name == other.Name;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
