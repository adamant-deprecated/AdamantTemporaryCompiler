using System;

namespace Adamant.Core
{
	/// <summary>
	/// Since C# char is only 2 bytes instead of the 4 needed for a full unicode code point, we use this struct for Adamant code points.
	/// </summary>
	public struct CodePoint
	{
		public static CodePoint MinValue = new CodePoint(0);
		public static CodePoint MaxValue = new CodePoint(0x10FFFF);

		private readonly uint value;

		private CodePoint(uint value)
		{
			this.value = value;
		}

		public static CodePoint Get(string value, int index)
		{
			return new CodePoint((uint)char.ConvertToUtf32(value, index));
		}

		#region Equality
		public static bool operator ==(CodePoint a, CodePoint b)
		{
			return a.value == b.value;
		}

		public static bool operator !=(CodePoint a, CodePoint b)
		{
			return a.value != b.value;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var other = obj as CodePoint?;
			return other != null && this == other.Value;
		}
		#endregion

		#region Conversion
		public static implicit operator uint (CodePoint value)
		{
			return value.value;
		}

		public static explicit operator int (CodePoint value)
		{
			return (int)value.value;
		}

		public static implicit operator CodePoint(char value)
		{
			return new CodePoint(value);
		}

		public static explicit operator CodePoint(uint value)
		{
			if(value > MaxValue.value)
				throw new ArgumentOutOfRangeException(nameof(value), value, "Not a valid code point");
			return new CodePoint(value);
		}

		public static explicit operator CodePoint(int value)
		{
			if(value < 0 || value > MaxValue.value)
				throw new ArgumentOutOfRangeException(nameof(value), value, "Not a valid code point");

			return new CodePoint((uint)value);
		}
		#endregion

		public override string ToString()
		{
			return $"U+{value:X}";
		}
	}
}
