using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using Netco.Extensions;
using Netco.Logging;

namespace MagentoAccess.Services
{
	internal class WebRequestServices : IWebRequestServices
	{
		private const int requestTimeoutMs = 5 * 60 * 1000;

		#region BaseRequests
		[ Obsolete ]
		public WebRequest CreateServiceGetRequest( string serviceUrl, Dictionary< string, string > rawUrlParameters )
		{
			var parametrizedServiceUrl = serviceUrl;

			if( rawUrlParameters.Any() )
			{
				parametrizedServiceUrl += "?" + rawUrlParameters.Keys.Aggregate( string.Empty,
					( accum, item ) => accum + "&" + string.Format( "{0}={1}", item, rawUrlParameters[ item ] ) );
			}

			var serviceRequest = ( HttpWebRequest )WebRequest.Create( parametrizedServiceUrl );
			serviceRequest.Method = WebRequestMethods.Http.Get;
			//
			serviceRequest.ContentType = "text/html";
			serviceRequest.KeepAlive = true;
			serviceRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			serviceRequest.CookieContainer = new CookieContainer();
			serviceRequest.CookieContainer.Add( new Uri( "http://192.168.0.104" ), new Cookie( "PHPSESSID", "mfl1c4qsrjs647chj2ummgo886" ) );
			serviceRequest.CookieContainer.Add( new Uri( "http://192.168.0.104" ), new Cookie( "adminhtml", "mk8rlurr9c4kaecnneakg55rv7" ) );
			serviceRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
			//
			return serviceRequest;
		}

		public async Task< WebRequest > CreateServiceGetRequestAsync( string serviceUrl, string body, Dictionary< string, string > rawHeaders )
		{
			return await this.CreateCustomRequestAsync( serviceUrl, body, rawHeaders ).ConfigureAwait( false );
		}

		public async Task< WebRequest > CreateCustomRequestAsync( string serviceUrl, string body, Dictionary< string, string > rawHeaders, string method = WebRequestMethods.Http.Get, string parameters = null )
		{
			try
			{
				var compositeUrl = string.IsNullOrEmpty( parameters ) ? serviceUrl : $"{serviceUrl.TrimEnd( '/', '?' )}/?{parameters}";

				var serviceRequest = ( HttpWebRequest )WebRequest.Create( compositeUrl );
				serviceRequest.Method = method;
				serviceRequest.ContentType = "application/json";
				serviceRequest.KeepAlive = true;
				serviceRequest.Timeout = requestTimeoutMs;

				rawHeaders?.ForEach( k => serviceRequest.Headers.Add( k.Key, k.Value ) );
				serviceRequest.UserAgent = MagentoService.UserAgentHeader;

				if( ( serviceRequest.Method == WebRequestMethods.Http.Post || serviceRequest.Method == WebRequestMethods.Http.Put ) && !string.IsNullOrWhiteSpace( body ) )
				{
					var encodedBody = new UTF8Encoding().GetBytes( body );
					serviceRequest.ContentLength = encodedBody.Length;
					using( var newStream = await serviceRequest.GetRequestStreamAsync().ConfigureAwait( false ) )
						newStream.Write( encodedBody, 0, encodedBody.Length );
				}
				return serviceRequest;
			}
			catch( Exception exc )
			{
				var methodParameters = $@"{{Url:'{serviceUrl}', Body:'{body}', Headers:{rawHeaders.ToJson()}}}";
				throw new MagentoWebException( $"Exception occured. {this.CreateMethodCallInfo( methodParameters )}", exc );
			}
		}

		public void PopulateRequestByBody( string body, HttpWebRequest webRequest )
		{
			try
			{
				if( !string.IsNullOrWhiteSpace( body ) )
				{
					var encodedBody = new UTF8Encoding().GetBytes( body );

					webRequest.ContentLength = encodedBody.Length;
					webRequest.ContentType = "text/xml";
					var getRequestStremTask = webRequest.GetRequestStreamAsync();
					getRequestStremTask.Wait();
					using( var newStream = getRequestStremTask.Result )
						newStream.Write( encodedBody, 0, encodedBody.Length );
				}
			}
			catch( Exception exc )
			{
				var webrequestUrl = "null";

				if( webRequest != null )
				{
					if( webRequest.RequestUri != null )
					{
						if( webRequest.RequestUri.AbsoluteUri != null )
							webrequestUrl = webRequest.RequestUri.AbsoluteUri;
					}
				}

				throw new MagentoWebException( $"Exception occured on PopulateRequestByBody(body:{body ?? "null"}, webRequest:{webrequestUrl})", exc );
			}
		}
		#endregion

		#region ResponseHanding
		public Stream GetResponseStream( WebRequest webRequest )
		{
			try
			{
				using( var response = ( HttpWebResponse )webRequest.GetResponse() )
				using( var dataStream = response.GetResponseStream() )
				{
					var memoryStream = new MemoryStream();
					if( dataStream != null )
						dataStream.CopyTo( memoryStream, 0x100 );
					memoryStream.Position = 0;
					return memoryStream;
				}
			}
			catch( Exception ex )
			{
				var webrequestUrl = "null";

				if( webRequest != null )
				{
					if( webRequest.RequestUri != null )
					{
						if( webRequest.RequestUri.AbsoluteUri != null )
							webrequestUrl = webRequest.RequestUri.AbsoluteUri;
					}
				}

				throw new MagentoWebException( $"Exception occured on GetResponseStream( webRequest:{webrequestUrl})", ex );
			}
		}

		public async Task< Stream > GetResponseStreamAsync( WebRequest webRequest, Mark mark = null )
		{
			try
			{
				using( var response = ( HttpWebResponse )await webRequest.GetResponseAsync().ConfigureAwait( false ) )
				using( var dataStream = await new TaskFactory< Stream >().StartNew( () => response != null ? response.GetResponseStream() : null ).ConfigureAwait( false ) )
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
				var webrequestUrl = PredefinedValues.NotAvailable;

				if( webRequest?.RequestUri?.AbsoluteUri != null )
					webrequestUrl = webRequest.RequestUri.AbsoluteUri;

				throw new MagentoWebException( $"Exception occured on GetResponseStreamAsync( webRequest:{webrequestUrl})", ex );
			}
		}
		#endregion
	}
}