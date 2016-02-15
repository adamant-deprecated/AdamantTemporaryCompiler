using System;

namespace Adamant.CompilerCompiler.Lex.Spec
{
	public class Channel
	{
		public readonly string Name;

		public Channel(string name)
		{
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
			Name = name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != GetType()) return false;
			return Equals((Channel)obj);
		}

		protected bool Equals(Channel other)
		{
			return string.Equals(Name, other.Name);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
