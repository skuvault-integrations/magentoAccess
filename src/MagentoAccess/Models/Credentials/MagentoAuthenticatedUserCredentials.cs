namespace MagentoAccess.Models.Credentials
{
	public class MagentoAuthenticatedUserCredentials
	{
		public MagentoAuthenticatedUserCredentials(string accessToken, string accessTokenSecret, string baseMagentoUrl, string consumerSckretKey, string consumerKey)
		{
			AccessTokenSecret = accessTokenSecret;
			AccessToken = accessToken;
			BaseMagentoUrl = baseMagentoUrl;
			ConsumerKey = consumerKey; ;
			ConsumerSckretKey = consumerSckretKey;
		}

		public string AccessTokenSecret { get; set; }
		public string AccessToken { get; set; }
		public string BaseMagentoUrl { get; set; }
		public string ConsumerSckretKey { get; set; }
		public string ConsumerKey { get; set; }
	}
}