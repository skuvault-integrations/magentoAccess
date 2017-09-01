using System.Linq;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models;
using MagentoAccess.Models.PutInventory;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	internal class MagentoServiceTest : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void UpdateInventoryAsync_UserAlreadyHasAccessTokens_ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			//------------ Act
			var onlyProductsCreatedForThisTests = this.GetOnlyProductsCreatedForThisTests( magentoService );
			var updateFirstTimeQty = 123;
			var updateInventoryTask = magentoService.UpdateInventoryAsync( onlyProductsCreatedForThisTests.ToInventory( x => updateFirstTimeQty ).ToList() );
			updateInventoryTask.Wait();

			/////
			var onlyProductsCreatedForThisTests2 = this.GetOnlyProductsCreatedForThisTests( magentoService );

			var updateSecondTimeQty = 100500;
			var updateInventoryTask2 = magentoService.UpdateInventoryAsync( onlyProductsCreatedForThisTests2.ToInventory( x => updateSecondTimeQty ).ToList() );
			updateInventoryTask2.Wait();

			//------------ Assert
			var onlyProductsCreatedForThisTests3 = this.GetOnlyProductsCreatedForThisTests( magentoService );

			onlyProductsCreatedForThisTests2.Should().NotBeNullOrEmpty();
			onlyProductsCreatedForThisTests2.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == updateFirstTimeQty );
			onlyProductsCreatedForThisTests3.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == updateSecondTimeQty );
		}

		[ Ignore("can corrupt data") ]
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void UpdateInventoryBYSkuAsync_UserAlreadyHasAccessTokens_ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			//------------ Act
			var getProductsTask = magentoService.GetProductsAsync( new[] { 0, 1 } );
			getProductsTask.Wait();

			var allProductsinMagent = getProductsTask.Result.ToList();
			var onlyProductsCreatedForThisTests = allProductsinMagent.Where( x => this._productsIds[ credentials.StoreUrl ].ContainsKey( int.Parse( x.ProductId ) ) );

			var itemsToUpdate = onlyProductsCreatedForThisTests.Select( x => new InventoryBySku() { Sku = x.Sku, Qty = 123 } );

			var updateInventoryTask = magentoService.UpdateInventoryBySkuAsync( itemsToUpdate, new[] { 0, 1 } );
			updateInventoryTask.Wait();

			/////

			var getProductsTask2 = magentoService.GetProductsAsync( new[] { 0, 1 } );
			getProductsTask2.Wait();

			var allProductsinMagent2 = getProductsTask2.Result.ToList();
			var onlyProductsCreatedForThisTests2 = allProductsinMagent2.Where( x => this._productsIds[ credentials.StoreUrl ].ContainsKey( int.Parse( x.ProductId ) ) );

			var itemsToUpdate2 = onlyProductsCreatedForThisTests2.Select( x => new InventoryBySku() { Sku = x.Sku, Qty = 100500 } );

			var updateInventoryTask2 = magentoService.UpdateInventoryBySkuAsync( itemsToUpdate2, new[] { 0, 1 } );
			updateInventoryTask2.Wait();

			//------------ Assert
			var getProductsTask3 = magentoService.GetProductsAsync( new[] { 0, 1 } );
			getProductsTask3.Wait();

			var allProductsinMagent3 = getProductsTask3.Result.ToList();
			var onlyProductsCreatedForThisTests3 = allProductsinMagent3.Where( x => this._productsIds[ credentials.StoreUrl ].ContainsKey( int.Parse( x.ProductId ) ) );

			onlyProductsCreatedForThisTests2.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == 123 );
			onlyProductsCreatedForThisTests3.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == 100500 );
		}
	}
}