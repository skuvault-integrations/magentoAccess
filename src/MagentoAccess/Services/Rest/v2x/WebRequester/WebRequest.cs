using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x;
using Netco.Logging;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public sealed class WebRequest
	{
		public MagentoUrl Url { get; private set; } = MagentoUrl.SandBox;
		public MagentoWebRequestMethod MagentoWebRequestMethod { get; private set; } = MagentoWebRequestMethod.Get;
		public MagentoServicePath MagentoServicePath { get; private set; } = MagentoServicePath.CreateProductsServicePath();
		public MagentoWebRequestBody Body { get; private set; } = MagentoWebRequestBody.Empty;
		public SearchCriteria Parameters { get; private set; } // TODO: create special class
		public AuthorizationToken AuthorizationToken { get; set; } = AuthorizationToken.Empty;
		public int? Timeout { get; set; }

		public static WebRequestBuilder Create()
		{
			return new WebRequestBuilder( new WebRequest() );
		}

		public Task< Stream > RunAsync( CancellationToken cancellationToken, Mark mark = null )
		{
			var webRequestServices = new WebRequestServices();
			var serviceUrl = this.Url.ToString().TrimEnd('/') + "/" + this.MagentoServicePath;
			if ( this.Parameters != null )
			{
				serviceUrl = $"{serviceUrl.TrimEnd( '/', '?' )}/?{this.Parameters}";
			}

			var body = this.Body.ToString();
			var method = this.MagentoWebRequestMethod.ToString();

			var logClientHeadersBeforeRequest = new Action< string >( clientHeaders =>
			{
				MagentoLogger.LogTraceRequestMessage( $"method:'{method}',url:'{serviceUrl}',parameters:'{this.Parameters}',headers:{clientHeaders},body:'{body}', timeout:'{this.Timeout} ms'", mark );
			} );

			return webRequestServices.GetResponseStreamAsync( method, serviceUrl, this.AuthorizationToken.Token, cancellationToken, body, this.Timeout, logClientHeadersBeforeRequest );
		}

		public class WebRequestBuilder
		{
			private readonly WebRequest WebRequest;

			public WebRequestBuilder( WebRequest webRequest )
			{
				this.WebRequest = webRequest;
			}

			public WebRequestBuilder Url( MagentoUrl url )
			{
				this.WebRequest.Url = url;
				return this;
			}

			public WebRequestBuilder Path( MagentoServicePath magentoServicePath )
			{
				this.WebRequest.MagentoServicePath = magentoServicePath;
				return this;
			}

			public WebRequestBuilder AuthToken( AuthorizationToken authorizationToken )
			{
				this.WebRequest.AuthorizationToken = authorizationToken;
				return this;
			}

			public WebRequestBuilder Method( MagentoWebRequestMethod magentoWebRequestMethod )
			{
				this.WebRequest.MagentoWebRequestMethod = magentoWebRequestMethod;
				return this;
			}

			public WebRequestBuilder Body( string body )
			{
				this.WebRequest.Body = MagentoWebRequestBody.Create( body );
				return this;
			}

			public WebRequestBuilder Parameters( SearchCriteria parameters )
			{
				this.WebRequest.Parameters = parameters;
				return this;
			}

			public WebRequestBuilder Timeout( int timeout )
			{
				this.WebRequest.Timeout = timeout;
				return this;
			}

			public static implicit operator WebRequest( WebRequestBuilder pb )
			{
				return pb.WebRequest;
			}
		}
	}
}