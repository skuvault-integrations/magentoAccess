namespace MagentoAccess.Models.Services.Credentials
{
	public class MagentoAuthenticatedUserCredentials
	{
		public MagentoAuthenticatedUserCredentials( string accessToken, string accessTokenSecret, string baseMagentoUrl, string consumerSckretKey, string consumerKey, string soapUserName, string soapUserPassword )
		{
			this.AccessTokenSecret = accessTokenSecret;
			this.AccessToken = accessToken;
			this.BaseMagentoUrl = baseMagentoUrl;
			this.ConsumerKey = consumerKey;
			this.ConsumerSckretKey = consumerSckretKey;
			this.SoapUserName = soapUserName;
			this.SoapUserPassword = soapUserPassword;
		}

		public string AccessTokenSecret { get; private set; }
		public string AccessToken { get; private set; }
		public string BaseMagentoUrl { get; private set; }
		public string ConsumerSckretKey { get; private set; }
		public string ConsumerKey { get; private set; }
		public string SoapUserName { get; private set; }
		public string SoapUserPassword { get; private set; }
	}
}