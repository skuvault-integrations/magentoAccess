using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Services;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	public class Programm
	{
		[ Test ]
		public void LowLevelOauth()
		{
			//------------ Arrange
			var testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv" );
			var consumer = testData.GetMagentoConsumerCredentials();
			var authorityUrls = testData.GetMagentoUrls();
			var accessToken = testData.GetMagentoAccessToken();
			MagentoServiceLowLevelOauth service;

			if( accessToken == null )
				service = new MagentoServiceLowLevelOauth( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl );
			else
				service = new MagentoServiceLowLevelOauth( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, accessToken.AccessToken, accessToken.AccessTokenSecret );

			//------------ Act
			if( accessToken == null )
			{
				var authorizeTask = service.GetAccessToken();
				authorizeTask.Wait();
				testData.CreateAccessTokenFile( service.AccessToken, service.AccessTokenSecret );
			}

			var res = service.InvokeGetCall( "products", true );

			//------------ Assert
			res.Should().NotBeNullOrWhiteSpace();
		}
	}
}