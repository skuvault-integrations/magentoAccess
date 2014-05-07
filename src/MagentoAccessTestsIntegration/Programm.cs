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

		//[ Test ]
		//public void SimpleCass()
		//{
		//	//------------ Arrange
		//	var webser = new WebRequestServices();
		//	var service = new MagentoServiceLowLevel( webser, "http://192.168.0.104/magento/api/rest/products" );

		//	//------------ Act
		//	//var resTask = service.GetItemsAsync();
		//	//resTask.Wait();

		//	//------------ Assert
		//	//resTask.Result.Should().NotBeEmpty();
		//}

		//[ Test ]
		//public void SimpleCass2()
		//{
		//	//------------ Arrange
		//	var webser = new WebRequestServices();
		//	var service = new MagentoServiceLowLevel( webser, "http://192.168.0.104/magento/api/rest/products" );

		//	//------------ Act
		//	var resTask = service.GetItems();
		//	//resTask.Wait();

		//	//------------ Assert
		//	//resTask.Result.Should().NotBeEmpty();
		//	resTask.Should().NotBeEmpty();
		//}

		//[Test]
		//public void LowLevelOauthManual()
		//{
		//	//------------ Arrange
		//	var testData = new TestData(@"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv");
		//	var consumer = testData.GetMagentoConsumerCredentials();
		//	var authorityUrls = testData.GetMagentoUrls();
		//	var accessToken = testData.GetMagentoAccessToken();
		//	MagentoServiceLowLevelOauth service;

		//	if (accessToken == null)
		//		service = new MagentoServiceLowLevelOauth(consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl);
		//	else
		//		service = new MagentoServiceLowLevelOauth(consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, accessToken.AccessToken, accessToken.AccessTokenSecret);

		//	//------------ Act
		//	if (accessToken == null)
		//	{
		//		var authorizeTask = service.GetAccessToken();
		//		authorizeTask.Wait();
		//		testData.CreateAccessTokenFile(service.AccessToken, service.AccessTokenSecret);
		//	}

		//	var res = service.InvokeGetCallManual("products", true);

		//	//------------ Assert
		//	res.Should().NotBeNullOrWhiteSpace();
		//}
		//[Test]
		//public void LowLevelOauthManualGuest()
		//{
		//	//------------ Arrange
		//	var testData = new TestData(@"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv");
		//	var consumer = testData.GetMagentoConsumerCredentials();
		//	var authorityUrls = testData.GetMagentoUrls();
		//	var accessToken = testData.GetMagentoAccessToken();
		//	MagentoServiceLowLevelOauth service;

		//	if (accessToken == null)
		//		service = new MagentoServiceLowLevelOauth(consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl);
		//	else
		//		service = new MagentoServiceLowLevelOauth(consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, accessToken.AccessToken, accessToken.AccessTokenSecret);

		//	//------------ Act
		//	if (accessToken == null)
		//	{
		//		var authorizeTask = service.GetAccessToken();
		//		authorizeTask.Wait();
		//		testData.CreateAccessTokenFile(service.AccessToken, service.AccessTokenSecret);
		//	}

		//	var res = service.InvokeQuestGetCallManual("products", true);

		//	//------------ Assert
		//	res.Should().NotBeNullOrWhiteSpace();
		//}
		//[Test]
		//public void LowLevelOauthGuestWebServices()
		//{
		//	//------------ Arrange
		//	var testData = new TestData(@"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv");
		//	var consumer = testData.GetMagentoConsumerCredentials();
		//	var authorityUrls = testData.GetMagentoUrls();
		//	var accessToken = testData.GetMagentoAccessToken();
		//	MagentoServiceLowLevelOauth service;

		//	if (accessToken == null)
		//		service = new MagentoServiceLowLevelOauth(consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl);
		//	else
		//		service = new MagentoServiceLowLevelOauth(consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, accessToken.AccessToken, accessToken.AccessTokenSecret);

		//	//------------ Act
		//	if (accessToken == null)
		//	{
		//		var authorizeTask = service.GetAccessToken();
		//		authorizeTask.Wait();
		//		testData.CreateAccessTokenFile(service.AccessToken, service.AccessTokenSecret);
		//	}

		//	var res = service.InvokeQuestGetCallWebServices("products", true);

		//	//------------ Assert
		//	res.Should().NotBeNullOrWhiteSpace();
		//}
	}
}