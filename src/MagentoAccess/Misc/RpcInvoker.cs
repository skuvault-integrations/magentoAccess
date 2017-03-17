using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MagentoAccess.Misc
{
	public static class RpcInvoker
	{
		public static async Task< T > NullOnIncorrectEnvelop< T >( Func< Task< T > > func ) where T : class
		{
			try
			{
				return await func().ConfigureAwait( false );
			}
			catch( CommunicationException ex ) //crutch for magento 2.1
			{
				var exceptionMessge = ex.Message.ToLower();
				if( exceptionMessge.Contains( @"(soap12 (http://www.w3.org/2003/05/soap-envelope))" ) &&
				    exceptionMessge.Contains( @"(soap11 (http://schemas.xmlsoap.org/soap/envelope/))" ) )
				{
					return null;
				}
				throw;
			}
		}
	}
}