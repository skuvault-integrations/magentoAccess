using System.Collections.Generic;
using FluentAssertions;
using MagentoAccess.Models.PutStockItems;
using MagentoAccess.Services;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services
{
	[ TestFixture ]
	public class MeagentoServiceLowLevelTest
	{
		private TestData _testData;
		private MagentoConsumerCredentials _consumer;
		private MagentoUrls _authorityUrls;
		private MagentoAccessToken _accessToken;
		private MagentoServiceLowLevel _service;

		[ SetUp ]
		public void Setup()
		{
			this._testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv" );
			this._consumer = this._testData.GetMagentoConsumerCredentials();
			this._authorityUrls = this._testData.GetMagentoUrls();
			this._accessToken = this._testData.GetMagentoAccessToken();
			this._service = ( this._accessToken == null ) ?
				new MagentoServiceLowLevel( this._consumer.Key, this._consumer.Secret, this._authorityUrls.MagentoBaseUrl, this._authorityUrls.RequestTokenUrl, this._authorityUrls.AuthorizeUrl, this._authorityUrls.AccessTokenUrl ) :
				new MagentoServiceLowLevel( this._consumer.Key, this._consumer.Secret, this._authorityUrls.MagentoBaseUrl, this._accessToken.AccessToken, this._accessToken.AccessTokenSecret );

			if( this._accessToken == null )
			{
				var authorizeTask = this._service.PopulateAccessToken();
				authorizeTask.Wait();
				this._testData.CreateAccessTokenFile( this._service.AccessToken, this._service.AccessTokenSecret );
			}
		}

		[ Test ]
		public void GetOrders_StoreContainsOrders_GetsItems()
		{
			//------------ Arrange

			//------------ Act
			var getOrdersTask = this._service.GetOrdersAsync();
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.Orders.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void PutInventory_StoreContainsInventory_InventoryUpdateResultContainsMessageForEachItem()
		{
			//------------ Arrange
			var inventoryItems = new List< InventoryItem > { new InventoryItem { ItemId = "1", MinQty = 1, ProductId = "1", Qty = 277, StockId = "1" } };

			//------------ Act
			var res = this._service.PutInventory( inventoryItems );

			//------------ Assert
			res.Items.Count.Should().Be( inventoryItems.Count );
		}

		[ Test ]
		public void GetProducts_StoreWithProducts_GetsProducts()
		{
			//------------ Arrange
			//------------ Act
			var res = this._service.GetProducts();

			//------------ Assert
			res.Products.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetInventory_StoreContainsInventory_GetsInventory()
		{
			//------------ Arrange
			//------------ Act
			var res = this._service.GetInventory();

			//------------ Assert
			res.Items.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetProduct_StoreWithProducts_GetsProduct()
		{
			//------------ Arrange
			//------------ Act
			var res = this._service.GetProduct( "1" );

			//------------ Assert
			res.Should().NotBeNull();
		}
	}
}