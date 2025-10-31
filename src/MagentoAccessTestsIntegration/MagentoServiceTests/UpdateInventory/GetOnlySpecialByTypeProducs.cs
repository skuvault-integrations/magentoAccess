using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models;
using MagentoAccess.Models.PutInventory;
using MagentoAccessTestsIntegration.TestEnvironment;
using Netco.Extensions;
using Netco.Logging;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.GetProductsAsync
{
	[ Explicit ]
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class UpdateInventory : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void InventoryUpdated( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );
			var rnd = new Random( DateTime.UtcNow.Millisecond );
			var val = rnd.Next( 100, 1500 );

			// ------------ Act
			//get products
			var getProductsTask1 = magentoService.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, includeDetails : true, productType : "simple", excludeProductByType : false, mark : new Mark( nameof( InventoryUpdated ) + "_s" ) );
			Task.WhenAll( getProductsTask1 ).Wait();
			var initialSkus = getProductsTask1.Result./*Where( x => x.Sku.Contains( "estSku1" ) ).*/ToList();
			var src = initialSkus.ToInventory( x => ( int )x.Qty.ToDecimalOrDefault() );
			var inventories = src as IList< Inventory > ?? src.ToList();
			inventories.ForEach( x => x.Qty = val );

			//update
			var updateInventoryTask = magentoService.UpdateInventoryAsync( inventories, CancellationToken.None );
			Task.WhenAll( updateInventoryTask ).Wait();

			//get products
			var getProductsTask3 = magentoService.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, includeDetails : true, productType : "simple", excludeProductByType : false, mark : new Mark( nameof( InventoryUpdated ) + "_s" ) );
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