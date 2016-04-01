using System;
using System.Linq;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services.Soap;
using MagentoAccess.Services.Soap._1_9_2_1_ce;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services
{
	[ TestFixture ]
	internal class MeagentoServiceLowLevelSoap_1_9_2_1_ce : BaseTest
	{
		protected IMagentoServiceLowLevelSoap CreateMagentoInternalSoapService( MagentoServiceSoapCredentials soapCredentials )
		{
			return new MagentoServiceLowLevelSoap_v_1_9_2_1_ce( soapCredentials.SoapApiUser, soapCredentials.SoapApiKey, soapCredentials.StoreUrl, null );
		}

		protected IMagentoServiceLowLevelSoap CreateMagentoInternalSoapService( string soapApiUser, string soapApiKey, string storeUrl )
		{
			return new MagentoServiceLowLevelSoap_v_1_9_2_1_ce( soapApiUser, soapApiKey, storeUrl, null );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetOrders_ByDatesStoreContainsOrders_ReceiveOrders( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoInternalService = CreateMagentoInternalSoapService( credentials );

			//------------ Act
			var firstCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).First();
			var lastCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).Last();

			var modifiedFrom = firstCreatedItem.UpdatedAt.AddSeconds( 1 );
			var modifiedTo = lastCreatedItem.UpdatedAt.AddSeconds( -1 );

			var getOrdersTask = magentoInternalService.GetOrdersAsync( modifiedFrom, modifiedTo );
			getOrdersTask.Wait();

			//------------ Assert
			var thatMustBeReturned = this._orders[ credentials.StoreUrl ].Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x =>
				new
				{
					OrderId = x.OrderId,
					OrderIncrementalId = x.OrderIncrementalId,
					UpdatedAt = x.UpdatedAt,
				}
				).ToList();
			var thatWasReturned = getOrdersTask.Result.Orders.Select( x =>
				new
				{
					OrderId = x.OrderId,
					OrderIncrementalId = x.incrementId,
					UpdatedAt = x.UpdatedAt.ToDateTimeOrDefault(),
				}
				).ToList();

			thatWasReturned.Should().BeEquivalentTo( thatMustBeReturned );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetOrders_ByIdsStoreContainsOrders_ReceiveOrders( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoInternalService = CreateMagentoInternalSoapService( credentials );

			//------------ Act
			//var ordersIds = new List< string >() { "100000001", "100000002" };
			var mustBeReturned = this._orders[ credentials.StoreUrl ];
			var ordersIds = mustBeReturned.Select( x => x.OrderIncrementalId ).ToList();

			var getOrdersTask = magentoInternalService.GetOrdersAsync( ordersIds );
			getOrdersTask.Wait();

			//------------ Assert
			var wasReturned = getOrdersTask.Result.Orders.Select(
				x =>
					new
					{
						OrderId = x.OrderId,
						OrderIncrementalId = x.incrementId,
						UpdatedAt = x.UpdatedAt,
					}
				);
			var mustBeReturnedBriefInfo = mustBeReturned.Select(
				x =>
					new
					{
						OrderId = x.OrderId,
						OrderIncrementalId = x.OrderIncrementalId,
						UpdatedAt = x.UpdatedAt,
					}
				);
			wasReturned.ShouldBeEquivalentTo( mustBeReturnedBriefInfo );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetProducts_StoreContainsProducts_ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoInternalService = CreateMagentoInternalSoapService( credentials );

			//------------ Act
			var getProductsTask = magentoInternalService.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Products.Should().NotBeEmpty();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetStockItems_StoreContainsStockItems_ReceiveStockItems( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoInternalService = CreateMagentoInternalSoapService( credentials );

			//------------ Act
			//var skusorids = new List< string >() { "501shirt", "311" };
			var skusorids = this._productsIds[ credentials.StoreUrl ].Select( ( kv, i ) => i % 2 == 0 ? kv.Key.ToString() : kv.Value ).ToList();

			var getProductsTask = magentoInternalService.GetStockItemsAsync( skusorids );
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.InventoryStockItems.Select( x => x.ProductId ).ShouldBeEquivalentTo( this._productsIds[ credentials.StoreUrl ].Select( x => x.Key ) );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetSessionId_StoreContainsUser_ReceiveSessionId( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoInternalService = CreateMagentoInternalSoapService( credentials );

			//------------ Act
			var getProductsTask = magentoInternalService.GetSessionId( false );
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
				var service = CreateMagentoInternalSoapService( "incorrect api user", credentials.SoapApiKey, credentials.StoreUrl );

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
			var magentoInternalService = CreateMagentoInternalSoapService( credentials );

			//------------ Act

			var productsAsync = magentoInternalService.GetStockItemsAsync( this._productsIds[ credentials.StoreUrl ].Select( x => x.Value ).ToList() );
			productsAsync.Wait();

			var updateFirsttimeQty = 123;
			var itemsToUpdate = productsAsync.Result.InventoryStockItems.Select( x => new PutStockItem( new Inventory() { Qty = updateFirsttimeQty, ProductId = x.ProductId } ) ).ToList();

			var getProductsTask = magentoInternalService.PutStockItemsAsync( itemsToUpdate, null );
			getProductsTask.Wait();

			////

			var productsAsync2 = magentoInternalService.GetStockItemsAsync( this._productsIds[ credentials.StoreUrl ].Select( x => x.Value ).ToList() );
			productsAsync2.Wait();

			var updateSecondTimeQty = 100500;
			var itemsToUpdate2 = productsAsync2.Result.InventoryStockItems.Select( x => new PutStockItem( new Inventory() { Qty = updateSecondTimeQty, ProductId = x.ProductId } ) ).ToList();

			var getProductsTask2 = magentoInternalService.PutStockItemsAsync( itemsToUpdate2, null );
			getProductsTask2.Wait();

			//------------ Assert
			var productsAsync3 = magentoInternalService.GetStockItemsAsync( this._productsIds[ credentials.StoreUrl ].Select( x => x.Value ).ToList() );
			productsAsync3.Wait();

			productsAsync2.Result.InventoryStockItems.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == updateFirsttimeQty );
			productsAsync3.Result.InventoryStockItems.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == updateSecondTimeQty );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetMagentoInfoAsync_StoreExist_StoreVersionRecived( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoInternalService = CreateMagentoInternalSoapService( credentials );

			//------------ Act

			var productsAsync = magentoInternalService.GetMagentoInfoAsync( false );
			productsAsync.Wait();

			//------------ Assert
			productsAsync.Result.MagentoVersion.Should().NotBeNullOrWhiteSpace();
		}
	}
}