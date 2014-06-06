using System.Collections.Generic;
using CuttingEdge.Conditions;
using MagentoAccess.Misc;

namespace MagentoAccess.Models.Services.Credentials
{
	public class MagentoNonAuthenticatedUserCredentials
	{
		public MagentoNonAuthenticatedUserCredentials( string consumerKey, string consumerSckretKey, string baseMagentoUrl, string requestTokenUrl, string authorizeUrl, string accessTokenUrl )
		{
			this.ConsumerKey = consumerKey;
			this.ConsumerSckretKey = consumerSckretKey;
			this.BaseMagentoUrl = baseMagentoUrl;
			this.RequestTokenUrl = requestTokenUrl;
			this.AuthorizeUrl = authorizeUrl;
			this.AccessTokenUrl = accessTokenUrl;
		}

		public MagentoNonAuthenticatedUserCredentials( string consumerKey, string consumerSckretKey, string baseMagentoUrl )
			: this(
				consumerKey,
				consumerSckretKey,
				baseMagentoUrl,
				new List< string > { baseMagentoUrl, "oauth/initiate" }.BuildUrl(),
				new List< string > { baseMagentoUrl, "admin/oauth_authorize" }.BuildUrl(),
				new List< string > { baseMagentoUrl, "ooauth/token" }.BuildUrl()
				)
		{
			Condition.Ensures( baseMagentoUrl ).EndsWith( "/" );
		}

		public string ConsumerKey { get; private set; }
		public string ConsumerSckretKey { get; private set; }
		public string BaseMagentoUrl { get; private set; }
		public string RequestTokenUrl { get; private set; }
		public string AuthorizeUrl { get; private set; }
		public string AccessTokenUrl { get; private set; }
	}
}