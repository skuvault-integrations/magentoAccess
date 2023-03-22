namespace MagentoAccess.Models.Credentials
{
	public sealed class MagentoAuthenticatedUserCredentials
	{
		public MagentoAuthenticatedUserCredentials( string accessToken, string accessTokenSecret, string baseMagentoUrl, 
			string consumerSecretKey, string consumerKey, string soapApiUser, string soapApiKey, int getProductsThreadsLimit, 
			int sessionLifeTimeMs, bool logRawMessages )
		{
			this.AccessTokenSecret = accessTokenSecret;
			this.AccessToken = accessToken;
			this.BaseMagentoUrl = baseMagentoUrl;
			this.ConsumerKey = consumerKey;
			this.ConsumerSecretKey = consumerSecretKey;
			this.SoapApiUser = soapApiUser;
			this.SoapApiKey = soapApiKey;
			this.GetProductsThreadsLimit = getProductsThreadsLimit;
			this.SessionLifeTimeMs = sessionLifeTimeMs;
			this.LogRawMessages = logRawMessages;
		}

		public string AccessTokenSecret { get; }
		public string AccessToken { get; }
		public string BaseMagentoUrl { get; }
		public string ConsumerSecretKey { get; }
		public string ConsumerKey { get; }
		public string SoapApiUser { get; }
		public string SoapApiKey { get; }
		public int GetProductsThreadsLimit { get; }
		public int SessionLifeTimeMs { get; }
		public bool LogRawMessages { get; }
	}
}