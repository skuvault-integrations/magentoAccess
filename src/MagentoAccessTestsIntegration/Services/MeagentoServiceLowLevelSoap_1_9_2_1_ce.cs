using System;
using System.Linq;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services.Soap._1_14_1_0_ee;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services
{
	[ TestFixture ]
	[ Ignore( "Since Test cases used for MagentoService" ) ]
	internal class MeagentoServiceLowLevelSoap_1_9_2_1_ce : BaseTest
	{
		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetOrders_ByDatesStoreContainsOrders_ReceiveOrders( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act
			var firstCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).First();
			var lastCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).Last();

			var modifiedFrom = firstCreatedItem.UpdatedAt.AddSeconds( 1 );
			var modifiedTo = lastCreatedItem.UpdatedAt.AddSeconds( -1 );

			var getOrdersTask = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.GetOrdersAsync( modifiedFrom, modifiedTo );
			getOrdersTask.Wait();

			//------------ Assert
			var thatMustBeReturned = this._orders[ credentials.StoreUrl ].Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.OrderIncrementalId ).ToList();
			var thatWasReturned = getOrdersTask.Result.Orders.ToList().Select( x => x.incrementId ).ToList();

			thatWasReturned.Should().BeEquivalentTo( thatMustBeReturned );

			getOrdersTask.Result.Orders.ShouldBeEquivalentTo( this._orders[ credentials.StoreUrl ].Take( this._orders[ credentials.StoreUrl ].Count() - 1 ).Skip( 1 ) );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetOrders_ByIdsStoreContainsOrders_ReceiveOrders( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act
			//var ordersIds = new List< string >() { "100000001", "100000002" };
			var ordersIds = this._orders[ credentials.StoreUrl ].Select( x => x.OrderIncrementalId ).ToList();

			var getOrdersTask = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.GetOrdersAsync( ordersIds );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.Orders.ShouldBeEquivalentTo( this._orders[ credentials.StoreUrl ] );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetProducts_StoreContainsProducts_ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Products.Should().NotBeEmpty();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetStockItems_StoreContainsStockItems_ReceiveStockItems( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act
			//var skusorids = new List< string >() { "501shirt", "311" };
			var skusorids = this._productsIds[ credentials.StoreUrl ].Select( ( kv, i ) => i % 2 == 0 ? kv.Key.ToString() : kv.Value ).ToList();

			var getProductsTask = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.GetStockItemsAsync( skusorids );
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.InventoryStockItems.Select( x => x.ProductId ).ShouldBeEquivalentTo( this._productsIds[ credentials.StoreUrl ].Select( x => x.Key ) );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetSessionId_StoreContainsUser_ReceiveSessionId( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.GetSessionId( false );
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetSessionId_IncorrectApiUser_NoExceptionThrowns( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act

			Action act = () =>
			{
				var service = new MagentoServiceLowLevelSoap_v_1_14_1_0_EE(
					"incorrect api user",
					credentials.SoapApiKey,
					credentials.StoreUrl,
					null );

				var getProductsTask = service.GetSessionId( false );
				getProductsTask.Wait();
			};

			//------------ Assert

			act.ShouldNotThrow();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void UpdateInventory_StoreWithItems_ItemsUpdated( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act

			var productsAsync = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.GetStockItemsAsync( this._productsIds[ credentials.StoreUrl ].Select( x => x.Value ).ToList() );
			productsAsync.Wait();

			var updateFirsttimeQty = 123;
			var itemsToUpdate = productsAsync.Result.InventoryStockItems.Select( x => new PutStockItem( new Inventory() { Qty = updateFirsttimeQty, ProductId = x.ProductId } ) ).ToList();

			var getProductsTask = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.PutStockItemsAsync( itemsToUpdate );
			getProductsTask.Wait();

			////

			var productsAsync2 = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.GetStockItemsAsync( this._productsIds[ credentials.StoreUrl ].Select( x => x.Value ).ToList() );
			productsAsync2.Wait();

			var updateSecondTimeQty = 100500;
			var itemsToUpdate2 = productsAsync2.Result.InventoryStockItems.Select( x => new PutStockItem( new Inventory() { Qty = updateSecondTimeQty, ProductId = x.ProductId } ) ).ToList();

			var getProductsTask2 = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.PutStockItemsAsync( itemsToUpdate2 );
			getProductsTask2.Wait();

			//------------ Assert
			var productsAsync3 = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.GetStockItemsAsync( this._productsIds[ credentials.StoreUrl ].Select( x => x.Value ).ToList() );
			productsAsync3.Wait();

			productsAsync2.Result.InventoryStockItems.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == updateFirsttimeQty );
			productsAsync3.Result.InventoryStockItems.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == updateSecondTimeQty );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetMagentoInfoAsync_StoreExist_StoreVersionRecived( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act

			var productsAsync = this._magentoServiceLowLevelSoapV_1_9_2_1_ce.GetMagentoInfoAsync();
			productsAsync.Wait();

			//------------ Assert
			productsAsync.Result.MagentoVersion.Should().NotBeNullOrWhiteSpace();
		}
	}
}