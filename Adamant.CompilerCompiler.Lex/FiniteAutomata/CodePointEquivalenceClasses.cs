using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Core;
using Adamant.FiniteAutomata;
using Sys.Text;

namespace Adamant.CompilerCompiler.Lex.FiniteAutomata
{
	public class CodePointEquivalenceClasses : IEnumerable<Input>
	{
		private readonly IList<InversionListCodePointSet> classes = new List<InversionListCodePointSet>();

		public CodePointEquivalenceClasses()
		{
			classes.Add(InversionListCodePointSet.All);
		}

		public int Count => classes.Count;

		public Input GetClass(CodePoint value)
		{
			for(var i = 0; i < classes.Count; i++)
				if(classes[i].Contains(value))
					return new Input(i);

			throw new Exception("Every character should belong to an equivalence class.");
		}

		/**
		 * The effect of this method is to ensure that all characters in the
		 * set are not in the same equivalece class with any characters not
		 * in the set.
		 */
		public void MakeClasses(InversionListCodePointSet set)
		{
			var oldCount = Count;
			for(var i = 0; i < oldCount; i++)
			{
				if(classes[i].Equals(set))
					return; // A class for exactly this set already exists

				var intersection = classes[i].Intersect(set);
				var intersectionComplement = intersection.Complement(); // used for difference later

				if(intersection.IsEmpty()) continue;

				if(intersection.Equals(classes[i])) // i.e. classes[i] is subset of set
				{
					set = set.Intersect(intersectionComplement); // set -= intersection
					continue;
				}

				if(intersection.Equals(set)) // i.e. set is subset of classes[i]
				{
					classes[i] = classes[i].Intersect(intersectionComplement); // classes[i] -= intersection
					classes.Add(intersection);
					return;
				}

				set = set.Intersect(intersectionComplement); // set -= intersection
				classes[i] = classes[i].Intersect(intersectionComplement); // classes[i] -= intersection
				classes.Add(intersection);
			}
		}

		public void MakeClasses(CodePoint codePoint)
		{
			MakeClasses(new InversionListCodePointSet(codePoint));
		}

		public void MakeClasses(string value)
		{
			foreach(var codePoint in value.GetCodePoints())
				MakeClasses(codePoint);
		}

		public IEnumerable<Input> GetClasses(InversionListCodePointSet codePoints)
		{
			var result = new List<Input>();
			for(var i = 0; i < classes.Count; i++)
				if(!classes[i].Intersect(codePoints).IsEmpty()) // TODO an overlaps method would optimize this
					result.Add(new Input(i));

			return result;
		}

		public IEnumerator<Input> GetEnumerator()
		{
			return Enumerable.Range(0, classes.Count).Select(i => new Input(i)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
