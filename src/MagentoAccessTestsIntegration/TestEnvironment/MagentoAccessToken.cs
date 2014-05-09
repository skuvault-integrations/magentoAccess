namespace MagentoAccessTestsIntegration.TestEnvironment
{
	public class MagentoAccessToken
	{
		public MagentoAccessToken( string accessToken, string accessTokenSecret )
		{
			this.AccessToken = accessToken;
			this.AccessTokenSecret = accessTokenSecret;
		}

		public string AccessToken { get; private set; }

		public string AccessTokenSecret { get; private set; }
	}
}