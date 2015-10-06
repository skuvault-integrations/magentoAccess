using System;
using System.Linq;
using FluentAssertions;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
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

			//------------ Act
			var firstCreatedItem = this._orders.OrderBy( x => x.UpdatedAt.ToDateTimeOrDefault() ).First();
			var lastCreatedItem = this._orders.OrderBy( x => x.UpdatedAt.ToDateTimeOrDefault() ).Last();

			var modifiedFrom = DateTime.Parse( firstCreatedItem.UpdatedAt ).AddSeconds( 1 );
			var modifiedTo = DateTime.Parse( lastCreatedItem.UpdatedAt ).AddSeconds( -1 );

			var getOrdersTask = this._magentoLowLevelSoapService.GetOrdersAsync( modifiedFrom, modifiedTo );
			getOrdersTask.Wait();

			//------------ Assert
			var thatMustBeReturned = this._orders.Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.incrementId ).ToList();
			var thatWasReturned = getOrdersTask.Result.Orders.ToList().Select( x => x.incrementId ).ToList();

			thatWasReturned.Should().BeEquivalentTo( thatMustBeReturned );

			getOrdersTask.Result.Orders.ShouldBeEquivalentTo( this._orders.Take( this._orders.Count() - 1 ).Skip( 1 ) );
		}

		[ Test ]
		public void GetOrders_ByIdsStoreContainsOrders_ReceiveOrders()
		{
			//------------ Arrange

			//------------ Act
			//var ordersIds = new List< string >() { "100000001", "100000002" };
			var ordersIds = this._orders.Select( x => x.incrementId ).ToList();

			var getOrdersTask = this._magentoLowLevelSoapService.GetOrdersAsync( ordersIds );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.Orders.ShouldBeEquivalentTo( this._orders );
		}

		[ Test ]
		public void GetProducts_StoreContainsProducts_ReceiveProducts()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._magentoLowLevelSoapService.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Products.Should().NotBeEmpty();
		}

		[ Test ]
		public void GetStockItems_StoreContainsStockItems_ReceiveStockItems()
		{
			//------------ Arrange

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

			//------------ Act

			var productsAsync = this._magentoLowLevelSoapService.GetStockItemsAsync( this._productsIds.Select( x => x.Value ).ToList() );
			productsAsync.Wait();

			var itemsToUpdate = productsAsync.Result.result.Select( x => new PutStockItem( x.product_id, new catalogInventoryStockItemUpdateEntity() { qty = "123" } ) ).ToList();

			var getProductsTask = this._magentoLowLevelSoapService.PutStockItemsAsync( itemsToUpdate );
			getProductsTask.Wait();

			////

			var productsAsync2 = this._magentoLowLevelSoapService.GetStockItemsAsync( this._productsIds.Select( x => x.Value ).ToList() );
			productsAsync2.Wait();

			var itemsToUpdate2 = productsAsync2.Result.result.Select( x => new PutStockItem( x.product_id, new catalogInventoryStockItemUpdateEntity() { qty = "100500" } ) ).ToList();

			var getProductsTask2 = this._magentoLowLevelSoapService.PutStockItemsAsync( itemsToUpdate2 );
			getProductsTask2.Wait();

			//------------ Assert
			var productsAsync3 = this._magentoLowLevelSoapService.GetStockItemsAsync( this._productsIds.Select( x => x.Value ).ToList() );
			productsAsync3.Wait();

			productsAsync2.Result.result.Should().OnlyContain( x => x.qty.ToDecimalOrDefault() == 123 );
			productsAsync3.Result.result.Should().OnlyContain( x => x.qty.ToDecimalOrDefault() == 100500 );
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