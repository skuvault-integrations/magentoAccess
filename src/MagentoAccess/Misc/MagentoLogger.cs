using System;
using Netco.Logging;

namespace MagentoAccess.Misc
{
	internal class MagentoLogger
	{
		public static ILogger Log()
		{
			return NetcoLogger.GetLogger( "MagentoLogger" );
		}

		public static void LogTraceException( Exception exception )
		{
			Log().Trace( exception, "[magento] An exception occured." );
		}

		public static void LogTraceStarted( string info )
		{
			Log().Trace( "[magento] Start call:{0}.", info );
		}

		public static void LogTraceEnded( string info )
		{
			Log().Trace( "[magento] End call:{0}.", info );
		}

		public static void LogTrace( string info )
		{
			Log().Trace( "[magento] Trace info:{0}.", info );
		}
	}
}