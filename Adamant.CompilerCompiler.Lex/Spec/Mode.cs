﻿using System;

namespace Adamant.CompilerCompiler.Lex.Spec
{
	public class Mode
	{
		public readonly string Name;

		public Mode(string name)
		{
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
			Name = name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		protected bool Equals(Mode other)
		{
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != this.GetType()) return false;
			return Equals((Mode) obj);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
