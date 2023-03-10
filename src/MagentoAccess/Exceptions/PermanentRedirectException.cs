using System;

namespace MagentoAccess.Exceptions
{
	/// <summary>
	/// Permanent Redirect exception indicates that the resource requested has been moved
	/// </summary>
	public class PermanentRedirectException : Exception
	{
		public PermanentRedirectException() { }
	}
}