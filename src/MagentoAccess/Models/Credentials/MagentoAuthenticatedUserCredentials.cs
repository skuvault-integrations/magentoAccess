namespace MagentoAccess.Models.Credentials
{
	public class MagentoAuthenticatedUserCredentials
	{
		public MagentoAuthenticatedUserCredentials( string accessToken, string accessTokenSecret, string baseMagentoUrl, string consumerSckretKey, string consumerKey, string soapApiUser, string soapApiKey, int getProductsThreadsLimit, int sessionLifeTimeMs, bool logRawMessages )
		{
			this.AccessTokenSecret = accessTokenSecret;
			this.AccessToken = accessToken;
			this.BaseMagentoUrl = baseMagentoUrl;
			this.ConsumerKey = consumerKey;
			this.ConsumerSckretKey = consumerSckretKey;
			this.SoapApiUser = soapApiUser;
			this.SoapApiKey = soapApiKey;
			this.GetProductsThreadsLimit = getProductsThreadsLimit;
			this.SessionLifeTimeMs = sessionLifeTimeMs;
			this.LogRawMessages = logRawMessages;
		}

		//public MagentoAuthenticatedUserCredentials( string accessToken, string accessTokenSecret, string baseMagentoUrl, string consumerSckretKey, string consumerKey, string soapApiUser, string soapApiKey ) : this( accessToken, accessTokenSecret, baseMagentoUrl, consumerSckretKey, consumerKey, soapApiUser, soapApiKey, 4, 3590 )
		//{
		//}

		public string AccessTokenSecret { get; private set; }
		public string AccessToken { get; private set; }
		public string BaseMagentoUrl { get; private set; }
		public string ConsumerSckretKey { get; private set; }
		public string ConsumerKey { get; private set; }
		public string SoapApiUser { get; private set; }
		public string SoapApiKey { get; private set; }
		public int GetProductsThreadsLimit { get; private set; }
		public int SessionLifeTimeMs { get; private set; }
		public bool LogRawMessages { get; private set; }
	}
}