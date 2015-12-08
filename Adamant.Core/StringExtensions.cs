using System.Collections.Generic;

namespace Adamant.Core
{
	public static class StringExtensions
	{
		public static IEnumerable<CodePoint> GetCodePoints(this string value)
		{
			for(var i = 0; i < value.Length; i++)
			{
				yield return (CodePoint)char.ConvertToUtf32(value, i);
				if(char.IsHighSurrogate(value, i))
					i++;
			}
		}
	}
}
