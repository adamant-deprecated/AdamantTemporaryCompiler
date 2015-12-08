using Adamant.Core;
using NUnit.Framework;
using Sys.Text;

namespace Adamant.TemporaryCompiler.Tests
{
	[TestFixture]
	public class InversionListCodePointSetTests
	{
		[Test]
		public void EmptyContainsNothing()
		{
			Assert.IsFalse(InversionListCodePointSet.Empty.Contains(CodePoint.MinValue), "min value");
			Assert.IsFalse(InversionListCodePointSet.Empty.Contains((CodePoint)0x65), "random value");
			Assert.IsFalse(InversionListCodePointSet.Empty.Contains(CodePoint.MaxValue), "max value");
		}

		[Test]
		public void Complement()
		{
			Assert.AreEqual(InversionListCodePointSet.Empty, InversionListCodePointSet.All.Complement());
			Assert.AreEqual(InversionListCodePointSet.All, InversionListCodePointSet.Empty.Complement());
		}

		[Test]
		public void Union()
		{
			var eg = new InversionListCodePointSet('e','g');
			var xz = new InversionListCodePointSet('x','z');
			var fhxz = new InversionListCodePointSet('f','h').Union(xz);
			var ehxz = new InversionListCodePointSet('e','h').Union(xz);
			Assert.AreEqual(ehxz, eg.Union(fhxz));
		}

		[Test]
		public void Interesection()
		{
			var mo = new InversionListCodePointSet('m','o');
			var ao = new InversionListCodePointSet('a','o');
			var mx = new InversionListCodePointSet('m','z');
			Assert.AreEqual(mo, ao.Intersect(mx));
		}

		[Test]
		public void Identities()
		{
			var a = new InversionListCodePointSet('a');
			var b = new InversionListCodePointSet('b');
			var x = new InversionListCodePointSet('x');
			Assert.AreEqual(a.Union(b).Complement(), a.Complement().Intersect(b.Complement()), "!(a|b) == !a & !b");
			Assert.AreEqual(a.Union(x).Complement(), a.Complement().Intersect(x.Complement()), "!(a|x) == !a & !x");
		}
	}
}
