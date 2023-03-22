using System;
using System.Net;

namespace MagentoAccess
{
	internal static class ExceptionExtensions
	{
		public static bool IsMagentoPermanentRedirectException( this Exception exception )
		{
			return exception is PermanentRedirectException || exception?.InnerException is PermanentRedirectException;
		}

		public static bool IsHttp308PermanentRedirectException( this Exception exception )
		{
			return exception?.InnerException is WebException
				&& ( int )( ( HttpWebResponse )( ( WebException )exception.InnerException ).Response ).StatusCode == 308;
		}
	}
}
