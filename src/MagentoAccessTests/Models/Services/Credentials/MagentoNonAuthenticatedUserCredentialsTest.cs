using FluentAssertions;
using MagentoAccess.Models.Services.Credentials;
using NUnit.Framework;

namespace MagentoAccessTests.Models.Services.Credentials
{
	[ TestFixture ]
	public class MagentoNonAuthenticatedUserCredentialsTest
	{
		[ Test ]
		public void Constructor_TakesBaseUri_BuildsAuthenticationUri()
		{
			//------------ Arrange

			//------------ Act
			var magentoNonAuthenticatedUserCredentials = new MagentoNonAuthenticatedUserCredentials(
				"does not metter",
				"does not metter",
				"http://192.168.0.104/magento/" );
			//------------ Assert
			magentoNonAuthenticatedUserCredentials.AccessTokenUrl.Should().Be( "http://192.168.0.104/magento/oauth/token/" );
			magentoNonAuthenticatedUserCredentials.AuthorizeUrl.Should().Be( "http://192.168.0.104/magento/admin/oauth_authorize/" );
			magentoNonAuthenticatedUserCredentials.RequestTokenUrl.Should().Be( "http://192.168.0.104/magento/oauth/initiate/" );
		}
	}
}