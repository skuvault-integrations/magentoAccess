namespace MagentoAccess.Models.Credentials
{
	public class MagentoNonAuthenticatedUserCredentials
	{
		public string ConsumerKey { get; set; }
		public string ConsumerSckretKey { get; set; }
		public string BaseMagentoUrl { get; set; }
		public string RequestTokenUrl { get; set; }
		public string AuthorizeUrl { get; set; }
		public string AccessTokenUrl { get; set; }
	}
}