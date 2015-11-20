using System;
using System.Diagnostics;
using System.Reflection;
using Netco.Logging;

namespace MagentoAccess.Misc
{
	internal class MagentoLogger
	{
		private static readonly FileVersionInfo _fvi;

		static MagentoLogger()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			_fvi = FileVersionInfo.GetVersionInfo( assembly.Location );
		}

		public static ILogger Log()
		{
			return NetcoLogger.GetLogger( "MagentoLogger" );
		}

		public static void LogTraceException( Exception exception )
		{
			Log().Trace( exception, "[magento] An exception occured. {0}.", _fvi );
		}

		public static void LogTraceStarted( string info )
		{
			Log().Trace( "[magento] Start call:{0}. {1}.", info, _fvi );
		}

		public static void LogTraceEnded( string info )
		{
			Log().Trace( "[magento] End call:{0}. {1}.", info, _fvi );
		}

		public static void LogTrace( string info )
		{
			Log().Trace( "[magento] Trace info:{0}. {1}", info, _fvi );
		}
	}
}