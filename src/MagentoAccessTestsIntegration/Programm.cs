using FluentAssertions;
using MagentoAccess.Services;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	public class Programm
	{
		[ Test ]
		public void GetProductTest()
		{
			//------------ Arrange
			var testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv" );
			var consumer = testData.GetMagentoConsumerCredentials();
			var authorityUrls = testData.GetMagentoUrls();
			var accessToken = testData.GetMagentoAccessToken();
			MagentoServiceLowLevel service;

			if( accessToken == null )
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl );
			else
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, accessToken.AccessToken, accessToken.AccessTokenSecret );

			//------------ Act
			if( accessToken == null )
			{
				var authorizeTask = service.GetAccessToken();
				authorizeTask.Wait();
				testData.CreateAccessTokenFile( service.AccessToken, service.AccessTokenSecret );
			}

			var res = service.GetProduct( "1" );

			//------------ Assert
			res.Should().NotBeNull();
		}

		[ Test ]
		public void GetProductsTest()
		{
			//------------ Arrange
			var testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv" );
			var consumer = testData.GetMagentoConsumerCredentials();
			var authorityUrls = testData.GetMagentoUrls();
			var accessToken = testData.GetMagentoAccessToken();
			MagentoServiceLowLevel service;

			if( accessToken == null )
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl );
			else
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, accessToken.AccessToken, accessToken.AccessTokenSecret );

			//------------ Act
			if( accessToken == null )
			{
				var authorizeTask = service.GetAccessToken();
				authorizeTask.Wait();
				testData.CreateAccessTokenFile( service.AccessToken, service.AccessTokenSecret );
			}

			var res = service.GetProducts();

			//------------ Assert
			res.Should().NotBeNull();
		}

		[ Test ]
		public void GetInventoryTest()
		{
			//------------ Arrange
			var testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv" );
			var consumer = testData.GetMagentoConsumerCredentials();
			var authorityUrls = testData.GetMagentoUrls();
			var accessToken = testData.GetMagentoAccessToken();
			MagentoServiceLowLevel service;

			if( accessToken == null )
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl );
			else
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, accessToken.AccessToken, accessToken.AccessTokenSecret );

			//------------ Act
			if( accessToken == null )
			{
				var authorizeTask = service.GetAccessToken();
				authorizeTask.Wait();
				testData.CreateAccessTokenFile( service.AccessToken, service.AccessTokenSecret );
			}

			var res = service.GetInventory();

			//------------ Assert
			res.Should().NotBeNull();
		}

		[ Test ]
		public void PutInventoryTest()
		{
			//------------ Arrange
			var testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv" );
			var consumer = testData.GetMagentoConsumerCredentials();
			var authorityUrls = testData.GetMagentoUrls();
			var accessToken = testData.GetMagentoAccessToken();
			MagentoServiceLowLevel service;

			if( accessToken == null )
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl );
			else
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, accessToken.AccessToken, accessToken.AccessTokenSecret );

			//------------ Act
			if( accessToken == null )
			{
				var authorizeTask = service.GetAccessToken();
				authorizeTask.Wait();
				testData.CreateAccessTokenFile( service.AccessToken, service.AccessTokenSecret );
			}

			var res = service.PutInventory();

			//------------ Assert
			res.Should().NotBeNull();
		}

		[ Test ]
		public void GetOrdersTest()
		{
			//------------ Arrange
			var testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv" );
			var consumer = testData.GetMagentoConsumerCredentials();
			var authorityUrls = testData.GetMagentoUrls();
			var accessToken = testData.GetMagentoAccessToken();
			MagentoServiceLowLevel service;

			if( accessToken == null )
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, authorityUrls.RequestTokenUrl, authorityUrls.AuthorizeUrl, authorityUrls.AccessTokenUrl );
			else
				service = new MagentoServiceLowLevel( consumer.Key, consumer.Secret, authorityUrls.MagentoBaseUrl, accessToken.AccessToken, accessToken.AccessTokenSecret );

			//------------ Act
			if( accessToken == null )
			{
				var authorizeTask = service.GetAccessToken();
				authorizeTask.Wait();
				testData.CreateAccessTokenFile( service.AccessToken, service.AccessTokenSecret );
			}

			var res = service.GetOrders();

			//------------ Assert
			res.Should().NotBeNull();
		}
	}
}