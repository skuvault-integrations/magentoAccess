using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Exceptions;
using MagentoAccess.Misc;
using Netco.Logging;
using Newtonsoft.Json;

namespace MagentoAccess.Services
{
	internal class WebRequestServices : IWebRequestServices
	{
		private HttpClient _httpClient = new HttpClient();
		private const int _maxHttpTimeoutInMinutes = 30;

		#region BaseRequests
		public HttpClient GetConfiguredHttpClient( string authorizationToken )
		{
			this._httpClient.Timeout = new TimeSpan( 0, _maxHttpTimeoutInMinutes, 0 );
			
			this._httpClient.DefaultRequestHeaders.Remove( "Authorization" );
			this._httpClient.DefaultRequestHeaders.Add( "Authorization", $"Bearer { authorizationToken }" );

			this._httpClient.DefaultRequestHeaders.Add( "User-Agent", MagentoService.UserAgentHeader );

			// Keep-Alive: true
			this._httpClient.DefaultRequestHeaders.ConnectionClose = false;

			return this._httpClient;
		}
		#endregion

		#region ResponseHanding
		private string GetCurrentHttpClientHeadersRaw()
		{
			return JsonConvert.SerializeObject( this._httpClient.DefaultRequestHeaders.Select( kv => new { Header = kv.Key, Value = kv.Value.FirstOrDefault() } ) );
		}

		public Task< Stream > GetResponseStreamAsync( string method, string url, string authorizationToken, CancellationToken cancellationToken, string body = null, int? operationTimeout = null, Action< string > logHeaders = null )
		{
			var httpClient = GetConfiguredHttpClient( authorizationToken );

			if ( logHeaders != null )
				logHeaders( GetCurrentHttpClientHeadersRaw() );

			if ( method == WebRequestMethods.Http.Get )
				return GetRawResponseStreamAsync( httpClient, url, cancellationToken, operationTimeout );
			else if ( method == WebRequestMethods.Http.Post )
				return PostAndGetRawResponseStreamAsync( httpClient, url, body, cancellationToken, operationTimeout );
			else if ( method == WebRequestMethods.Http.Put )
				return PutAndGetRawResponseStreamAsync( httpClient, url, body, cancellationToken, operationTimeout );

			throw new MagentoWebException( $"Http method {method} isn't supported", null );
		}

		private async Task< Stream > GetRawResponseStreamAsync( HttpClient client, string url, CancellationToken token, int? operationTimeout = null )
		{
			using( var cts = CancellationTokenSource.CreateLinkedTokenSource( token ) )
			{
				if ( operationTimeout != null )
					cts.CancelAfter( operationTimeout.Value );

				var httpResponse = await client.GetAsync( url, cts.Token ).ConfigureAwait( false );
				return await HandleResponseAsync( httpResponse, url );
			}
		}

		private async Task< Stream > PostAndGetRawResponseStreamAsync( HttpClient client, string url, string body, CancellationToken token, int? operationTimeout = null )
		{
			using( var cts = CancellationTokenSource.CreateLinkedTokenSource( token ) )
			{
				if ( operationTimeout != null )
					cts.CancelAfter( operationTimeout.Value );

				var content = new StringContent( body, Encoding.UTF8, "application/json" );
				var httpResponse = await client.PostAsync( url, content, cts.Token ).ConfigureAwait( false );
				return await HandleResponseAsync( httpResponse, url );
			}
		}

		private async Task< Stream > PutAndGetRawResponseStreamAsync( HttpClient client, string url, string body, CancellationToken token, int? operationTimeout = null )
		{
			using( var cts = CancellationTokenSource.CreateLinkedTokenSource( token ) )
			{
				if ( operationTimeout != null )
					cts.CancelAfter( operationTimeout.Value );

				var content = new StringContent( body, Encoding.UTF8, "application/json" );
				var httpResponse = await client.PutAsync( url, content, cts.Token ).ConfigureAwait( false );
				return await HandleResponseAsync( httpResponse, url );
			}
		}

		private async Task< Stream > HandleResponseAsync( HttpResponseMessage responseMessage, string url, Mark mark = null )
		{
			try
			{
				responseMessage.EnsureSuccessStatusCode();

				using( var dataStream = await new TaskFactory< Stream >().StartNew( () => responseMessage.Content.ReadAsStreamAsync().GetAwaiter().GetResult() ).ConfigureAwait( false ) )
				{
					var memoryStream = new MemoryStream();
					if( dataStream != null )
					{
						await dataStream.CopyToAsync( memoryStream, 0x100 ).ConfigureAwait( false );
						memoryStream.Position = 0;
						MagentoLogger.LogTraceResponseMessage( new StreamReader( memoryStream ).ReadToEnd(), mark );
					}

					memoryStream.Position = 0;
					return memoryStream;
				}
			}
			catch( Exception ex )
			{
				if ( ( int )responseMessage.StatusCode == 308 )
					throw new PermanentRedirectException();
				
				throw new MagentoWebException( $"Exception occured on GetResponseStreamAsync( webRequest:{url})", ex, responseMessage.StatusCode );
			}
		}
		#endregion
	}
}