namespace MagentoAccess.Models.Credentials
{
	public class MagentoAuthenticatedUserCredentials
	{
		public MagentoAuthenticatedUserCredentials( string accessToken, string accessTokenSecret, string baseMagentoUrl, string consumerSckretKey, string consumerKey )
		{
			this.AccessTokenSecret = accessTokenSecret;
			this.AccessToken = accessToken;
			this.BaseMagentoUrl = baseMagentoUrl;
			this.ConsumerKey = consumerKey;
			;
			this.ConsumerSckretKey = consumerSckretKey;
		}

		public string AccessTokenSecret { get; set; }
		public string AccessToken { get; set; }
		public string BaseMagentoUrl { get; set; }
		public string ConsumerSckretKey { get; set; }
		public string ConsumerKey { get; set; }
	}
}