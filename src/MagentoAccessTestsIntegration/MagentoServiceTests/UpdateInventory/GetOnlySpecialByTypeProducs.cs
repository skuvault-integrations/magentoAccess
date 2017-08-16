using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models;
using MagentoAccess.Models.PutInventory;
using MagentoAccessTestsIntegration.TestEnvironment;
using Netco.Extensions;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.GetProductsAsync
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class UpdateInventory : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void InventoryUpdated( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );
			var rnd = new Random( DateTime.UtcNow.Millisecond );
			var val = rnd.Next( 100, 1500 );

			// ------------ Act
			//get products
			var getProductsTask1 = magentoService.GetProductsAsync( new[] { 0, 1 }, includeDetails : true, productType : "simple", excludeProductByType : false, mark : new Mark( nameof( InventoryUpdated ) + "_s" ) );
			Task.WhenAll( getProductsTask1 ).Wait();
			var initialSkus = getProductsTask1.Result./*Where( x => x.Sku.Contains( "estSku1" ) ).*/ToList();
			var src = initialSkus.ToInventory( x => ( int )x.Qty.ToDecimalOrDefault() );
			var inventories = src as IList< Inventory > ?? src.ToList();
			inventories.ForEach( x => x.Qty = val );

			//update
			var updateInventoryTask = magentoService.UpdateInventoryAsync( inventories );
			Task.WhenAll( updateInventoryTask ).Wait();

			//get products
			var getProductsTask3 = magentoService.GetProductsAsync( new[] { 0, 1 }, includeDetails : true, productType : "simple", excludeProductByType : false, mark : new Mark( nameof( InventoryUpdated ) + "_s" ) );
			Task.WhenAll( getProductsTask3 ).Wait();
			var resultSku = getProductsTask3.Result/*.Where( x => x.Sku.Contains( "estSku1" ) )*/;

			// ------------ Assert
			initialSkus.Should().NotBeNullOrEmpty();
			resultSku.Should().NotBeNullOrEmpty();

			initialSkus.Any( x => x.Qty.ToDecimalOrDefault() != val ).Should().BeTrue();
			resultSku.All( x => x.Qty.ToDecimalOrDefault() == val ).Should().BeTrue();
		}
	}
}