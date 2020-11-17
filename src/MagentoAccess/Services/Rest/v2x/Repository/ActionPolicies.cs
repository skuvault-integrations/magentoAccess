using System;
using System.Net;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using Netco.ActionPolicyServices;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	internal static class ActionPolicies
	{
		public static ActionPolicyAsync RepeatOnChannelProblemAsync { get; } = ActionPolicyAsync.From( ( exception =>
		{
			var webException = ( exception as MagentoWebException )?.InnerException as WebException;
			if( webException == null )
				return false;

			switch( webException.Status )
			{
				case WebExceptionStatus.ProtocolError:
					var response = webException.Response as HttpWebResponse;
					if( response == null )
						return false;
					switch( response.StatusCode )
					{
						case HttpStatusCode.NotFound:
						case HttpStatusCode.BadRequest:
						case HttpStatusCode.ServiceUnavailable:
						case HttpStatusCode.BadGateway:
							return true;
						default:
							return false;
					}
				case WebExceptionStatus.ConnectionClosed:
				case WebExceptionStatus.ConnectFailure:
				case WebExceptionStatus.Timeout:
				case WebExceptionStatus.SecureChannelFailure:
				case WebExceptionStatus.RequestCanceled:
					return true;
				default:
					return false;
			}
		} ) )
			.RetryAsync( 7, async ( ex, i ) =>
			{
				MagentoLogger.Log().Trace( ex, "Retrying Magento API call due to channel problem for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );
	}
}