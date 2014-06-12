using Netco.Logging;

namespace MagentoAccess.Misc
{
	internal class MagentoLogger
	{
		public static ILogger Log()
		{
			return NetcoLogger.GetLogger( "MagentoLogger" );
		}
	}
}