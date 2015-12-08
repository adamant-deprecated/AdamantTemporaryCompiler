namespace Adamant.Core
{
	/// <summary>
	/// This struct is used when void is passed as a type parameter.  When passing or returning a value of this type,
	/// use Void.Value
	/// </summary>
	public struct Void
	{
		public static readonly Void Value = default(Void);
	}
}
