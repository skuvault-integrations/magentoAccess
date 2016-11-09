using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MagentoAccess.Models.Services.Rest.v1x.PutStockItems;
using MagentoAccess.Models.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v1x
{
	[ TestFixture ]
	[ Ignore( "Since REST has no goo implementation yet" ) ]
	internal class MeagentoServiceLowLevelRestTest : BaseTest
	{
		[ Test ]
		public void GetOrders_StoreContainsOrders_ReceiveOrders()
		{
			//------------ Arrange

			//------------ Act
			var getOrdersTask = this._magentoServiceLowLevelRestRestRestRest.GetOrdersAsync();
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.Orders.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetOrders_StoreContainsOrders_ReceiveOrders2()
		{
			//------------ Arrange

			//------------ Act
			var sc = new SearchCriteria()
			{
				filter_groups = new List< FilterGroup >()
				{
					new FilterGroup()
					{
						filters = new List< Filter > { new Filter( @"updated_at", @"2016-07-01 00:00:00", Filter.ConditionType.GreaterThan ) }
					}
				},
			};

			//------------ Assert
			var qwe = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
				.Parameters( sc )
				.Url( MagentoUrl.Create( "http://xxx" ) )
				;

			;
			var res = qwe.RunAsync();
			res.Wait();
		}

		[ Test ]
		public void PutInventory_StoreContainsInventory_ReceiveSucceessMessagesForEachRequestedItem()
		{
			//------------ Arrange
			var inventoryItems = new List< StockItem > { new StockItem { ItemId = this._productsIds.First().Key.ToString(), MinQty = 1, ProductId = this._productsIds.First().Key.ToString(), Qty = 277, StockId = "1" } };

			//------------ Act
			var putInventoryTask = this._magentoServiceLowLevelRestRestRestRest.PutStockItemsAsync( inventoryItems );
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
			var getProductsTask = this._magentoServiceLowLevelRestRestRestRest.GetProductsAsync( 1, 2 );
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Products.Count().Should().Be( 2 );
		}

		[ Test ]
		public void GetInventory_StoreContainsInventory_ReceiveInventory()
		{
			//------------ Arrange
			//------------ Act
			var getInventoryTask = this._magentoServiceLowLevelRestRestRestRest.GetStockItemsAsync( 1, 100 );
			getInventoryTask.Wait();

			//------------ Assert
			getInventoryTask.Result.Items.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetProduct_StoreWithProducts_ReceiveProduct()
		{
			//------------ Arrange
			//------------ Act
			var res = this._magentoServiceLowLevelRestRestRestRest.GetProductAsync( this._productsIds.First().Key.ToString() );

			//------------ Assert
			res.Result.Should().NotBeNull();
		}
	}
}