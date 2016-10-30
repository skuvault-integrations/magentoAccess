namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class WebRequest
	{
		public MagentoWebRequestMethod MagentoWebRequestMethod { get; private set; }
		public MagentoServicePath MagentoServicePath { get; private set; }
		public string Body { get; private set; }
		public string Parameters { get; private set; }

		public static WebRequestBuilder Create()
		{
			return new WebRequestBuilder( new WebRequest() );
		}

		public class WebRequestBuilder
		{
			private readonly WebRequest WebRequest;

			public WebRequestBuilder( WebRequest webRequest )
			{
				this.WebRequest = webRequest;
			}

			public WebRequestBuilder Path( MagentoServicePath magentoServicePath )
			{
				this.WebRequest.MagentoServicePath = magentoServicePath;
				return this;
			}

			public WebRequestBuilder Method( MagentoWebRequestMethod magentoWebRequestMethod )
			{
				this.WebRequest.MagentoWebRequestMethod = magentoWebRequestMethod;
				return this;
			}

			public WebRequestBuilder Body( string body )
			{
				this.WebRequest.Body = body;
				return this;
			}

			public WebRequestBuilder Parameters( string parameters )
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