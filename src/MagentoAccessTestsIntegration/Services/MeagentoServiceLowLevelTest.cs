using FluentAssertions;
using MagentoAccess.Services;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services
{
	[ TestFixture ]
	public class MeagentoServiceLowLevelTest
	{
		[ Test ]
		public void GetOrders_StoreContainsOrders_GetsItems()
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

		[ Test ]
		public void PutInventory_StoreContainsInventory_InventoryUpdateResultontainsUpdateStatus()
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
		public void GetProducts_StoreWithProducts_GetsProducts()
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
		public void GetInventory_StoreContainsInventory_GetsInventory()
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
		public void GetProduct_StoreWithProducts_GetsProduct()
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
	}
}