namespace MagentoAccess.Models.Credentials
{
	public class MagentoAuthenticatedUserCredentials
	{
		public string AccessTokenSecret { get; set; }
		public string AccessToken { get; set; }
		public string BaseMagentoUrl { get; set; }
		public string ConsumerSckretKey { get; set; }
		public string ConsumerKey { get; set; }
	}
}