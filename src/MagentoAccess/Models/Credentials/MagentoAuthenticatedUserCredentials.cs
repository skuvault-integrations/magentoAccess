namespace MagentoAccess.Models.Credentials
{
	public class MagentoAuthenticatedUserCredentials
	{
		public MagentoAuthenticatedUserCredentials( string accessToken, string accessTokenSecret, string baseMagentoUrl, 
			string consumerSecretKey, string consumerKey, string soapApiUser, string soapApiKey, int getProductsThreadsLimit, 
			int sessionLifeTimeMs, bool logRawMessages, string relativeUrl = "" )
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
			this.RelativeUrl = relativeUrl;
		}
		
		/// <summary>
		/// RelativeUrl setter
		/// It is needed because the field could be changed after initialization in case of http308
		/// </summary>
		/// <param name="relativeUrl"></param>
		/// <returns></returns>
		public MagentoAuthenticatedUserCredentials SetRelativeUrl( string relativeUrl )
		{
			this.RelativeUrl = relativeUrl;
			return this;
		}

		public string AccessTokenSecret { get; private set; }
		public string AccessToken { get; private set; }
		public string BaseMagentoUrl { get; private set; }
		public string ConsumerSecretKey { get; private set; }
		public string ConsumerKey { get; private set; }
		public string SoapApiUser { get; private set; }
		public string SoapApiKey { get; private set; }
		public int GetProductsThreadsLimit { get; private set; }
		public int SessionLifeTimeMs { get; private set; }
		public bool LogRawMessages { get; private set; }
		/// <summary>
		/// RelativeUrl could be used instead of the default path to Magento / SOAP endpoint. 
		/// </summary>
		public string RelativeUrl { get; private set; }
	}
}