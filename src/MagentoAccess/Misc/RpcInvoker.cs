using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MagentoAccess.Misc
{
	public static class RpcInvoker
	{
		public static async Task< RpcResponse< T > > SuppressExceptions< T >( Func< Task< T > > func ) where T : class
		{
			try
			{
				var result = await func().ConfigureAwait( false );
				return new RpcResponse< T >( SoapErrorCode.Success, result, null );
			}
			catch( CommunicationException ex ) //crutch for magento 2.1
			{
				var exceptionMessge = ex.Message.ToLower();
				if( exceptionMessge.Contains( @"(soap12 (http://www.w3.org/2003/05/soap-envelope))" ) &&
				    exceptionMessge.Contains( @"(soap11 (http://schemas.xmlsoap.org/soap/envelope/))" ) )
				{
					return new RpcResponse< T >( SoapErrorCode.WaitingAnotherEnvelopVersion, null, ex );
				}
				throw;
			}
		}

		public enum SoapErrorCode
		{
			Success = 0,
			WaitingAnotherEnvelopVersion = 1,
			Unknown = 2,
		}

		public interface IRpcResponse< out T >
		{
			SoapErrorCode ErrorCode{ get; }
			T Result{ get; }
			Exception Exception{ get; }
		}

		public class RpcResponse< T >: IRpcResponse< T >
		{
			public SoapErrorCode ErrorCode{ get; private set; }
			public T Result{ get; private set; }
			public Exception Exception{ get; private set; }

			public RpcResponse( SoapErrorCode errorCode, T result, Exception exception )
			{
				this.ErrorCode = errorCode;
				this.Result = result;
				this.Exception = exception;
			}
		}

		public class RpcRequestResponse< T1, T2 >
		{
			public T1 Request{ get; private set; }
			public IRpcResponse< T2 > Response{ get; private set; }

			public RpcRequestResponse( T1 request, IRpcResponse< T2 > response )
			{
				this.Request = request;
				this.Response = response;
			}
		}
	}
}