using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x;
using Netco.Logging;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class WebRequest
	{
		private const string MagentoRestPath = "/index.php/rest/V1/";// TODO: remove from here, this class unresponsible for this
		public MagentoUrl Url { get; private set; } = MagentoUrl.SandBox;
		public MagentoWebRequestMethod MagentoWebRequestMethod { get; private set; } = MagentoWebRequestMethod.Get;
		public MagentoServicePath MagentoServicePath { get; private set; } = MagentoServicePath.CreateProductsServicePath();
		public MagentoWebRequestBody Body { get; private set; } = MagentoWebRequestBody.Empty;
		public SearchCriteria Parameters { get; private set; } // TODO: create special class
		public AuthorizationToken AuthorizationToken { get; set; } = AuthorizationToken.Empty;
		private readonly Dictionary< string, string > CommonHeaders = new Dictionary< string, string > { { "user-agent", MagentoService.UserAgentHeader } };

		public static WebRequestBuilder Create()
		{
			return new WebRequestBuilder( new WebRequest() );
		}

		public async Task< Stream > RunAsync( Mark mark = null )
		{
			var webRequestServices = new WebRequestServices();
			var serviceUrl = this.Url.ToString().TrimEnd( '/' ) + MagentoRestPath + this.MagentoServicePath.ToString();
			var body = this.Body.ToString();
			var method = this.MagentoWebRequestMethod.ToString();
			var parameters = this.Parameters?.ToString();
			var rawHeaders = this.CommonHeaders;
			rawHeaders.Add( "Authorization", $"Bearer {this.AuthorizationToken}" );
			var requestAsync = await webRequestServices.CreateCustomRequestAsync(
				serviceUrl,
				body,
				rawHeaders, //TODO:create VO with builder!!!
				method,
				parameters ).ConfigureAwait( false );

			MagentoLogger.LogTraceRequestMessage( $"method:'{method}',url:'{serviceUrl}',parameters:'{parameters}',headers:{rawHeaders.ToJson()},body:'{body}'", mark );
			return await webRequestServices.GetResponseStreamAsync( requestAsync, mark ).ConfigureAwait( false );
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

			public static implicit operator WebRequest( WebRequestBuilder pb )
			{
				return pb.WebRequest;
			}
		}
	}
}