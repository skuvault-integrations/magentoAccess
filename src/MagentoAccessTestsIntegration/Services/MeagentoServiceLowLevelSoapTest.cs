using System;
using System.Linq;
using FluentAssertions;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Services;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services
{
	[ TestFixture ]
	internal class MeagentoServiceLowLevelSoapTest : BaseTest
	{
		[ Test ]
		public void GetOrders_ByDatesStoreContainsOrders_ReceiveOrders()
		{
			//------------ Arrange
			this.CreateOrders();

			//------------ Act
			var modifiedFrom = DateTime.Parse( this._orders.First().updated_at ).AddSeconds( 1 );
			var modifiedTo = DateTime.Parse( this._orders.Last().updated_at ).AddSeconds( -1 );
			var getOrdersTask = this._magentoLowLevelSoapService.GetOrdersAsync( modifiedFrom, modifiedTo );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.result.ShouldBeEquivalentTo( this._orders.Take( this._orders.Count() - 1 ).Skip( 1 ) );
		}

		[ Test ]
		public void GetOrders_ByIdsStoreContainsOrders_ReceiveOrders()
		{
			//------------ Arrange
			this.CreateOrders();

			//------------ Act
			//var ordersIds = new List< string >() { "100000001", "100000002" };
			var ordersIds = this._orders.Select( x => x.increment_id ).ToList();

			var getOrdersTask = this._magentoLowLevelSoapService.GetOrdersAsync( ordersIds );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.result.ShouldBeEquivalentTo( this._orders );
		}

		[ Test ]
		public void GetProducts_StoreContainsProducts_ReceiveProducts()
		{
			//------------ Arrange
			this.CreateProductstems();

			//------------ Act
			var getProductsTask = this._magentoLowLevelSoapService.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.result.Should().NotBeEmpty();
		}

		[ Test ]
		public void GetStockItems_StoreContainsStockItems_ReceiveStockItems()
		{
			//------------ Arrange
			this.CreateProductstems();

			//------------ Act
			//var skusorids = new List< string >() { "501shirt", "311" };
			var skusorids = this._productsIds.Select( ( kv, i ) => i % 2 == 0 ? kv.Key.ToString() : kv.Value ).ToList();

			var getProductsTask = this._magentoLowLevelSoapService.GetStockItemsAsync( skusorids );
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.result.Select( x => x.product_id ).ShouldBeEquivalentTo( this._productsIds.Select( x => x.Key ) );
		}

		[ Test ]
		public void GetSessionId_StoreContainsUser_ReceiveSessionId()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._magentoLowLevelSoapService.GetSessionId( false );
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull();
		}

		[ Test ]
		public void GetSessionId_IncorrectApiUser_NoExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act

			Action act = () =>
			{
				var service = new MagentoServiceLowLevelSoap(
					"incorrect api user",
					this._testData.GetMagentoSoapUser().ApiKey,
					this._testData.GetMagentoUrls().MagentoBaseUrl,
					null );

				var getProductsTask = service.GetSessionId( false );
				getProductsTask.Wait();
			};

			//------------ Assert

			act.ShouldNotThrow();
		}

		[ Test ]
		public void UpdateInventory_StoreWithItems_ItemsUpdated()
		{
			//------------ Arrange
			this.CreateProductstems();

			//------------ Act

			var productsAsync = this._magentoLowLevelSoapService.GetStockItemsAsync( this._productsIds.Select( x => x.Value ).ToList() );
			productsAsync.Wait();

			var itemsToUpdate = productsAsync.Result.result.Select( x => new PutStockItem( x.product_id, new catalogInventoryStockItemUpdateEntity() { qty = "123" } ) ).ToList();

			var getProductsTask = this._magentoLowLevelSoapService.PutStockItemsAsync( itemsToUpdate );
			getProductsTask.Wait();

			//------------ Assert
			var productsAsync2 = this._magentoLowLevelSoapService.GetStockItemsAsync( this._productsIds.Select( x => x.Value ).ToList() );
			productsAsync2.Wait();

			var itemsToUpdate2 = productsAsync.Result.result.Select( x => new PutStockItem( x.product_id, new catalogInventoryStockItemUpdateEntity() { qty = x.qty } ) ).ToList();
			itemsToUpdate2.Should().BeEquivalentTo( itemsToUpdate );
		}

		[ Test ]
		public void GetMagentoInfoAsync_StoreExist_StoreVersionRecived()
		{
			//------------ Arrange

			//------------ Act

			var productsAsync = this._magentoLowLevelSoapService.GetMagentoInfoAsync();
			productsAsync.Wait();

			//------------ Assert
			productsAsync.Result.result.magento_version.Should().NotBeNullOrWhiteSpace();
		}
	}
}