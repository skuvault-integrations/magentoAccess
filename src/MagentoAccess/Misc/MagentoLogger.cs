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
			Log().Trace( exception, "[magento]\t[{0}]\t[Exception]\t[mark:]", _fvi.FileVersion );
		}

		public static void LogTraceStarted( string info )
		{
			Log().Trace( "[magento]\t[{1}]\t[Start]\t[mark:]\t[payload:{0}]", info, _fvi.FileVersion );
		}

		public static void LogTraceEnded( string info )
		{
			Log().Trace( "[magento]\t[{1}]\t[End]\t[mark:]\t[payload:{0}]", info, _fvi.FileVersion );
		}

		public static void LogTrace( string info )
		{
			Log().Trace( "[magento]\t[{1}]\t[Trace]\t[mark:]\t[payload:{0}]", info, _fvi.FileVersion );
		}

		public static void LogTraceRequestMessage( string info, Mark mark = null )
		{
			Log().Trace( "[magento]\t[{1}]\t[Request]\t[mark:{2}]\t[payload:{0}]", info, _fvi.FileVersion, mark.ToStringSafe() );
		}

		public static void LogTraceResponseMessage( string info, Mark mark = null )
		{
			Log().Trace( "[magento]\t[{1}]\t[Response]\t[mark:{2}]\t[payload:{0}]", info, _fvi.FileVersion, mark.ToStringSafe() );
		}
	}
}