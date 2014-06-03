using Netco.Logging;

namespace MagentoAccess.Misc
{
	public class MagentoLogger
	{
		public static ILogger Log()
		{
			return NetcoLogger.GetLogger( "MagentoLogger" );
		}
	}
}