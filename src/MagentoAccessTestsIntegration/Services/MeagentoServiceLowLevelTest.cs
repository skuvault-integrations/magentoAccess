using System.Collections.Generic;
using System.Linq;
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
		public void GetOrders_StoreContainsOrders_ReceiveOrders()
		{
			//------------ Arrange

			//------------ Act
			var getOrdersTask = this._service.GetOrdersAsync();
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.Orders.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void PutInventory_StoreContainsInventory_ReceiveSucceessMessagesForEachRequestedItem()
		{
			//------------ Arrange
			var inventoryItems = new List< StockItem > { new StockItem { ItemId = "1", MinQty = 1, ProductId = "1", Qty = 277, StockId = "1" } };

			//------------ Act
			var putInventoryTask = this._service.PutStockItemsAsync( inventoryItems );
			putInventoryTask.Wait();

			//------------ Assert
			putInventoryTask.Result.Items.Count.Should().Be( inventoryItems.Count );
			putInventoryTask.Result.Items.TrueForAll( x => x.Code == "200" ).Should().BeTrue();
		}

		[ Test ]
		public void GetProducts_StoreWithProducts_ReceiveProducts()
		{
			//------------ Arrange
			//------------ Act
			var getProductsTask = this._service.GetProductsAsync( 1, 2 );
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Products.Count().Should().Be( 2 );
		}

		[ Test ]
		public void GetInventory_StoreContainsInventory_ReceiveInventory()
		{
			//------------ Arrange
			//------------ Act
			var getInventoryTask = this._service.GetStockItemsAsync( 1, 100 );
			getInventoryTask.Wait();

			//------------ Assert
			getInventoryTask.Result.Items.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetProduct_StoreWithProducts_ReceiveProduct()
		{
			//------------ Arrange
			//------------ Act
			var res = this._service.GetProductAsync( "1" );

			//------------ Assert
			res.Should().NotBeNull();
		}
	}
}