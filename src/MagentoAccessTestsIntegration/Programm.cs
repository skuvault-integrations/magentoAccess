using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	public class Programm
	{
		[ Test ]
		public void SimpleCass()
		{
			//------------ Arrange
			var webser = new WebRequestServices();
			var service = new MagentoServiceLowLevel( webser, "http://192.168.0.104/magento/api/rest/products" );

			//------------ Act
			//var resTask = service.GetItemsAsync();
			//resTask.Wait();

			//------------ Assert
			//resTask.Result.Should().NotBeEmpty();
		}

		[ Test ]
		public void SimpleCass2()
		{
			//------------ Arrange
			var webser = new WebRequestServices();
			var service = new MagentoServiceLowLevel( webser, "http://192.168.0.104/magento/api/rest/products" );

			//------------ Act
			var resTask = service.GetItems();
			//resTask.Wait();

			//------------ Assert
			//resTask.Result.Should().NotBeEmpty();
			resTask.Should().NotBeEmpty();
		}

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
				service = new MagentoServiceLowLevelOauth( consumer.Key, consumer.Secret, null, null, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl );
			else
				service = new MagentoServiceLowLevelOauth( consumer.Key, consumer.Secret, accessToken.AccessToken, accessToken.AccessTokenSecret);

			//------------ Act
			string res;
			var authorizeTask = service.GetAccessToken();
			authorizeTask.Wait();
			res = service.InvokeGetCall( true );

			//------------ Assert
			res.Should().NotBeNullOrWhiteSpace();
		}
	}
}