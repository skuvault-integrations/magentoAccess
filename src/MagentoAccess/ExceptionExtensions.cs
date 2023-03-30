using System;
using System.Net;

namespace MagentoAccess
{
	/// <summary>
	/// Contains extensions provided methods for definitions of specific types of errors
	/// </summary>
	internal static class ExceptionExtensions
	{
		/// <summary>
		/// Checks whether an exception is a Magento redirect-specific exception. 
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static bool IsMagentoPermanentRedirectException( this Exception exception )
		{
			return exception is PermanentRedirectException || exception?.InnerException is PermanentRedirectException;
		}

		/// <summary>
		/// Checks whether an exception is a http 308 redirect-specific exception.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static bool IsHttp308PermanentRedirectException( this Exception exception )
		{
			return exception?.InnerException is WebException
				&& ( int )( ( HttpWebResponse )( ( WebException )exception.InnerException ).Response ).StatusCode == 308;
		}
	}
}
