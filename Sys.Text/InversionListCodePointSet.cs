using System;
using Adamant.Core;

namespace Sys.Text
{
	// TODO this would implement ISet<CodePoint> in Adamant
	public struct InversionListCodePointSet
	{
		public static readonly InversionListCodePointSet Empty = new InversionListCodePointSet(new CodePoint[0]);
		public static readonly InversionListCodePointSet All = new InversionListCodePointSet(new[] { CodePoint.MinValue });

		// Inversion List Implementation based on "Unicode Demystified" by Richard Gillam, "Useful Data Structures"
		// https://en.wikipedia.org/wiki/Inversion_list
		private readonly CodePoint[] ranges;

		private InversionListCodePointSet(CodePoint[] ranges)
		{
			this.ranges = ranges;
		}

		public InversionListCodePointSet(CodePoint codePoint)
		{
			if(codePoint == CodePoint.MaxValue)
				ranges = new[] { codePoint };
			else
				ranges = new[] { codePoint, (CodePoint)(codePoint + 1) };
		}

		public InversionListCodePointSet(CodePoint from, CodePoint to)
		{
			if(from > to)
				throw new ArgumentException("from can't be greater than to");

			if(to == CodePoint.MaxValue)
				ranges = new[] { from };
			else
				ranges = new[] { from, (CodePoint)(to + 1) };
		}


		public bool Contains(CodePoint codePoint)
		{
			// binary search for it
			var low = 0;
			var high = ranges.Length;
			while(low < high)
			{
				var mid = (low + high) / 2;
				if(codePoint >= ranges[mid])
					low = mid + 1;
				else if(codePoint < ranges[mid])
					high = mid;
			}
			var pos = high - 1;
			return (pos & 1) == 0;
		}

		public InversionListCodePointSet Intersect(InversionListCodePointSet value)
		{
			var a = ranges;
			var b = value.ranges;
			var tempResult = new CodePoint[a.Length + b.Length];
			var posA = 0;
			var posB = 0;
			var posResult = 0;
			var count = 0;
			// go through the two sets as though you're merging them
			while(posA < a.Length && posB < b.Length)
			{
				CodePoint c;
				int pos;

				// if the lower entry is in the first array, or if the
				// entries are equal and this one's the end of a range,
				// consider the entry from the first array next
				if(a[posA] < b[posB] || (a[posA] == b[posB] && (posA & 1) == 1))
				{
					pos = posA;
					c = a[posA++];
				}
				// otherwise, consider the entry from the second array next
				else
				{
					pos = posB;
					c = b[posB++];
				}
				// if the entry is the start of a range (i.e., an even-numbered
				// entry), increment the running count. If the count goes to two
				// after incrementing then write the entry to the result set,
				// because both sets contain this.
				if((pos & 1) == 0)
				{
					++count;
					if(count == 2)
						tempResult[posResult++] = c;

				}
				// if the entry is the end of a range (i.e., an odd-numbered
				// entry), decrement the running count. If the count is two
				// before decrementing, also write the entry to the result set,
				// because only one set contains this range now
				else
				{
					if(count == 2)
						tempResult[posResult++] = c;
					--count;
				}
			}

			// figure out how big the result should really be
			var length = posResult;
			// if we stopped in the middle of a range, increment the count
			// before figuring out whether there are extra entries to write
			if((posA != a.Length && (posA & 1) == 0)
			|| (posB != b.Length && (posB & 1) == 0))
				++count;
			// if, after the adjustment, the count is 0, then all
			// entries from the set we haven't exhausted also go into
			// the result
			if(count == 2)
				length += (a.Length - posA) + (b.Length - posB);
			// copy the results into the actual result array (they may

			// include the excess from the array we hadn't finished
			// examining)
			var result = new CodePoint[length];
			Array.Copy(tempResult, 0, result, 0, posResult);
			if(count == 2)
			{
				// only one of these two calls will do anything
				Array.Copy(a, posA, result, posResult, a.Length - posA);
				Array.Copy(b, posB, result, posResult, b.Length - posB);
			}
			return new InversionListCodePointSet(result);
		}

		public InversionListCodePointSet Union(InversionListCodePointSet value)
		{
			var a = ranges;
			var b = value.ranges;
			var tempResult = new CodePoint[a.Length + b.Length];
			var posA = 0;
			var posB = 0;
			var posResult = 0;
			var count = 0;
			// go through the two sets as though you're merging them
			while(posA < a.Length && posB < b.Length)
			{
				CodePoint c;
				int pos;

				// if the lower entry is in the first array, or if the
				// entries are equal and this one's the start of a range,
				// consider the entry from the first array next
				if(a[posA] < b[posB] || (a[posA] == b[posB] && (posA & 1) == 0))
				{
					pos = posA;
					c = a[posA++];
				}
				// otherwise, consider the entry from the second array next
				else
				{
					pos = posB;
					c = b[posB++];
				}
				// if the entry is the start of a range (i.e., an even-numbered
				// entry), increment the running count. If the count was zero
				// before incrementing, also write the entry to the result
				// set becuase we are starting an included range.
				if((pos & 1) == 0)
				{
					if(count == 0)
						tempResult[posResult++] = c;
					++count;
				}
				// if the entry is the end of a range (i.e., an odd-numbered
				// entry), decrement the running count. If this makes the
				// count zero, also write the entry to the result set becuase we
				// are ending the range
				else
				{
					--count;
					if(count == 0)
						tempResult[posResult++] = c;
				}
			}

			// figure out how big the result should really be
			var length = posResult;
			// if we stopped in the middle of a range, decrement the count
			// before figuring out whether there are extra entries to write
			if((posA != a.Length && (posA & 1) == 1)
			|| (posB != b.Length && (posB & 1) == 1))
				--count;
			// if, after the adjustment, the count is 0, then all
			// entries from the set we haven't exhausted also go into
			// the result
			if(count == 0)
				length += (a.Length - posA) + (b.Length - posB);
			// copy the results into the actual result array (they may

			// include the excess from the array we hadn't finished
			// examining)
			var result = new CodePoint[length];
			Array.Copy(tempResult, 0, result, 0, posResult);
			if(count == 0)
			{
				// only one of these two calls will do anything
				Array.Copy(a, posA, result, posResult, a.Length - posA);
				Array.Copy(b, posB, result, posResult, b.Length - posB);
			}
			return new InversionListCodePointSet(result);
		}

		public InversionListCodePointSet Complement()
		{
			if(ranges.Length == 0 || ranges[0] != CodePoint.MinValue)
			{
				var newRanges = new CodePoint[ranges.Length + 1];
				newRanges[0] = CodePoint.MinValue;
				ranges.CopyTo(newRanges, 1);
				return new InversionListCodePointSet(newRanges);
			}
			else
			{
				var length = ranges.Length - 1;
				var newRanges = new CodePoint[length];
				Array.Copy(ranges, 1, newRanges, 0, length);
				return new InversionListCodePointSet(newRanges);
			}
		}

		public bool IsEmpty()
		{
			return ranges.Length == 0;
		}

		public override bool Equals(object obj)
		{
			var other = obj as InversionListCodePointSet?;
			return other != null && Equals(other.Value);
		}

		public bool Equals(InversionListCodePointSet other)
		{
			if(ranges.Length != other.ranges.Length) return false;
			for(var i = 0; i < ranges.Length; i++)
				if(ranges[i] != other.ranges[i])
					return false;

			return true;
		}

		public override int GetHashCode()
		{
			var hc = ranges.Length;
			for(var i = 0; i < ranges.Length; ++i)
				hc = unchecked(hc*31 + (int) ranges[i]);

			return hc;
		}

		public override string ToString()
		{
			return string.Join(", ", ranges);
		}
	}
}
