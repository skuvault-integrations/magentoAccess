namespace MagentoAccess.Models.Credentials
{
	public class MagentoAuthenticatedUserCredentials
	{
		public MagentoAuthenticatedUserCredentials( string accessToken, string accessTokenSecret, string baseMagentoUrl, string consumerSckretKey, string consumerKey, string soapApiUser, string soapApiKey )
		{
			this.AccessTokenSecret = accessTokenSecret;
			this.AccessToken = accessToken;
			this.BaseMagentoUrl = baseMagentoUrl;
			this.ConsumerKey = consumerKey;
			this.ConsumerSckretKey = consumerSckretKey;
			this.SoapApiUser = soapApiUser;
			this.SoapApiKey = soapApiKey;
		}

		public string AccessTokenSecret { get; private set; }
		public string AccessToken { get; private set; }
		public string BaseMagentoUrl { get; private set; }
		public string ConsumerSckretKey { get; private set; }
		public string ConsumerKey { get; private set; }
		public string SoapApiUser { get; private set; }
		public string SoapApiKey { get; private set; }
	}
}