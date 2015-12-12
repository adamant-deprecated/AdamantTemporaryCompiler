namespace Adamant.FiniteAutomata
{
	/// <summary>
	/// The type of input values for the FA.  Input must be bounded from 0 to N-1 where N is the total number of input elements
	/// </summary>
	public struct Input
	{
		public readonly int Value;

		public Input(int value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return $"Input {Value}";
		}

		public static bool operator !=(Input a, Input b)
		{
			return a.Value != b.Value;
		}

		public static bool operator ==(Input a, Input b)
		{
			return a.Value == b.Value;
		}
	}
}
