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
			Log().Trace( exception, "[magento] [{0}] [Exception]", _fvi.FileVersion );
		}

		public static void LogTraceStarted( string info )
		{
			Log().Trace( "[magento] [{1}] [Start]		[payload:{0}]", info, _fvi.FileVersion );
		}

		public static void LogTraceEnded( string info )
		{
			Log().Trace( "[magento] [{1}] [End]			[payload:{0}]", info, _fvi.FileVersion );
		}

		public static void LogTrace( string info )
		{
			Log().Trace( "[magento] [{1}] [Trace]		[payload:{0}]", info, _fvi.FileVersion );
		}

		public static void LogTraceRequestMessage( string info, Mark mark = null )
		{
			Log().Trace( "[magento] [{1}] [Request]		[mark:{2}] [payload:{0}]", info, _fvi.FileVersion, mark.ToStringSafe() );
		}

		public static void LogTraceResponseMessage( string info, Mark mark = null )
		{
			Log().Trace( "[magento] [{1}] [Response]	[mark:{2}] [payload:{0}]", info, _fvi.FileVersion, mark.ToStringSafe() );
		}
	}
}