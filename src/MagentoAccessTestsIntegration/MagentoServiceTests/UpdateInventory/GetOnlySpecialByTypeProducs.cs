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
			var getProductsTask1 = magentoService.GetProductsAsync( new[] { 0, 1 }, includeDetails : true, productType : "simple", excludeProductByType : false, mark : new Mark( nameof( InventoryUpdated ) + "_s" ) );
			Task.WhenAll( getProductsTask1 ).Wait();

			var src = getProductsTask1.Result.DeepClone().ToInventory( x => int.Parse( x.Qty ) );
			var inventories = src as IList< Inventory > ?? src.ToList();
			inventories.ForEach( x => x.Qty = val );
			var getProductsTask2 = magentoService.UpdateInventoryAsync( inventories );
			Task.WhenAll( getProductsTask2 ).Wait();

			var getProductsTask3 = magentoService.GetProductsAsync( new[] { 0, 1 }, includeDetails : true, productType : "simple", excludeProductByType : false, mark : new Mark( nameof( InventoryUpdated ) + "_s" ) );
			Task.WhenAll( getProductsTask3 ).Wait();

			// ------------ Assert
			getProductsTask1.Result.Should().NotBeNullOrEmpty();
			getProductsTask3.Result.Should().NotBeNullOrEmpty();

			getProductsTask1.Result.Any( x => x.Qty != val.ToString() ).Should().BeTrue();
			getProductsTask3.Result.All( x => x.Qty == val.ToString() ).Should().BeTrue();
		}
	}
}