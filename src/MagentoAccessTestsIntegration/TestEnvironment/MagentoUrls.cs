namespace MagentoAccessTestsIntegration.TestEnvironment
{
	public class MagentoUrls
	{
		public MagentoUrls( string magentoBaseUrl, string requestTokenUrl, string authorizeUrl, string accessTokenUrl )
		{
			this.MagentoBaseUrl = magentoBaseUrl;
			this.RequestTokenUrl = requestTokenUrl;
			this.AuthorizeUrl = authorizeUrl;
			this.AccessTokenUrl = accessTokenUrl;
		}

		public string RequestTokenUrl { get; private set; }

		public string MagentoBaseUrl { get; private set; }

		public string AuthorizeUrl { get; private set; }

		public string AccessTokenUrl { get; private set; }
	}
}