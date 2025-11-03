using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Models.PutInventory;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.InterchangeabilityTests.UpdateInventory
{
	[ Explicit ]
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class UpdateInventoryBySku : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( InterchangeabilityTestCases ), nameof(InterchangeabilityTestCases.TestStoresCredentials) ) ]
		public void InventoryUpdated( MagentoServiceCredentialsAndConfig credentialsRest, MagentoServiceCredentialsAndConfig credentialsSoap )
		{
			// ------------ Arrange
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.AuthenticatedUserCredentials.SoapApiUser, credentialsRest.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentialsRest.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.Config.VersionByDefault, credentialsRest.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentialsRest.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentialsRest.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );
			var magentoServiceSoap = this.CreateMagentoService( credentialsSoap.AuthenticatedUserCredentials.SoapApiUser, credentialsSoap.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentialsSoap.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentialsSoap.Config.VersionByDefault, credentialsSoap.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentialsSoap.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentialsSoap.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );
			var skus = new[] { "testsku1", "testsku2", "testsku3", "testsku4" };

			// ------------ Act
			var getProductsRestTask = magentoServiceRest.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, skus : skus, includeDetails : false );
			getProductsRestTask.Wait();
			var inventoryToUpdate = getProductsRestTask.Result.Where( p => p.ProductType == "simple" ).OrderBy( p => p.ProductId ).Select( p => new InventoryBySku() { Sku = p.Sku, Qty = long.Parse( p.Qty ) + 1 } );
			magentoServiceRest.UpdateInventoryBySkuAsync( inventoryToUpdate, CancellationToken.None );

			Task.Delay( 2000 ).Wait();
			var getProductsSoapTask = magentoServiceSoap.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, skus : skus, includeDetails : false );
			getProductsSoapTask.Wait();
			var updatedInventory = getProductsSoapTask.Result.Where( p => p.ProductType == "simple" ).OrderBy( p => p.ProductId ).Select( p => new InventoryBySku { Sku = p.Sku, Qty = long.Parse( p.Qty ) } );

			// ------------ Assert
			inventoryToUpdate.Should().BeEquivalentTo( updatedInventory );
		}

		[ Test ]
		[ TestCaseSource( typeof( InterchangeabilityTestCases ), nameof(InterchangeabilityTestCases.TestStoresCredentials) ) ]
		public async Task UpdateInventoryWhenItWasOutOfStock( MagentoServiceCredentialsAndConfig credentialsRest, MagentoServiceCredentialsAndConfig credentialsSoap )
		{
			var sku = "testsku1";
			var newQuantity = 99;
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.AuthenticatedUserCredentials.SoapApiUser, credentialsRest.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentialsRest.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.Config.VersionByDefault, credentialsRest.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentialsRest.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentialsRest.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

			// update to zero quantity first
			var updateSkuQuantityToZeroRequest = new InventoryBySku() { Sku = sku, Qty = 0 };
			await magentoServiceRest.UpdateInventoryBySkuAsync( new InventoryBySku[] { updateSkuQuantityToZeroRequest }, CancellationToken.None );

			var updateSkuQuantityRequest = new InventoryBySku() { Sku = sku, Qty = newQuantity };
			await magentoServiceRest.UpdateInventoryBySkuAsync( new InventoryBySku[] { updateSkuQuantityRequest }, CancellationToken.None );

			var searchProductsResult = magentoServiceRest.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, skus : new string[] { sku }, includeDetails : false ).Result;
			var updatedProduct = searchProductsResult.First();

			// ------------ Assert
			updatedProduct.Qty.Should().Be( updateSkuQuantityRequest.Qty.ToString() );
			updatedProduct.IsInStock.Should().Be( true );
		}

		[ Test ]
		[ TestCaseSource( typeof( InterchangeabilityTestCases ), nameof(InterchangeabilityTestCases.TestStoresCredentials) ) ]
		public async Task UpdateInventoryWhenItWasInStock( MagentoServiceCredentialsAndConfig credentialsRest, MagentoServiceCredentialsAndConfig credentialsSoap )
		{
			var sku = "testsku1";
			var previousQuantity = 99;
			var newQuantity = 0;
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.AuthenticatedUserCredentials.SoapApiUser, credentialsRest.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentialsRest.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.Config.VersionByDefault, credentialsRest.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentialsRest.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentialsRest.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

			var updateSkuQuantityToZeroRequest = new InventoryBySku() { Sku = sku, Qty = previousQuantity };
			await magentoServiceRest.UpdateInventoryBySkuAsync( new InventoryBySku[] { updateSkuQuantityToZeroRequest }, CancellationToken.None );

			var updateSkuQuantityRequest = new InventoryBySku() { Sku = sku, Qty = newQuantity };
			await magentoServiceRest.UpdateInventoryBySkuAsync( new InventoryBySku[] { updateSkuQuantityRequest }, CancellationToken.None );

			var searchProductsResult = magentoServiceRest.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, skus : new string[] { sku }, includeDetails : false ).Result;
			var updatedProduct = searchProductsResult.First();

			// ------------ Assert
			updatedProduct.Qty.Should().Be( updateSkuQuantityRequest.Qty.ToString() );
			updatedProduct.IsInStock.Should().Be( false );
		}
	}
}