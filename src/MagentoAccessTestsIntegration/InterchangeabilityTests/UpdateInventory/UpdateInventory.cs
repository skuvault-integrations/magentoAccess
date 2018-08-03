using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Models.PutInventory;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.InterchangeabilityTests.UpdateInventory
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class UpdateInventory : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( InterchangeabilityTestCases ), nameof(InterchangeabilityTestCases.TestStoresCredentials) ) ]
		public void InventoryUpdated( MagentoServiceCredentialsAndConfig credentialsRest, MagentoServiceCredentialsAndConfig credentialsSoap )
		{
			// ------------ Arrange
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.SoapApiUser, credentialsRest.SoapApiKey, "null", "null", "null", "null", credentialsRest.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.MagentoVersion, credentialsRest.GetProductsThreadsLimit, credentialsRest.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );
			var magentoServiceSoap = this.CreateMagentoService( credentialsSoap.SoapApiUser, credentialsSoap.SoapApiKey, "null", "null", "null", "null", credentialsSoap.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsSoap.MagentoVersion, credentialsSoap.GetProductsThreadsLimit, credentialsSoap.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );
			var skus = new string[] { "testsku1", "testsku2", "testsku3", "testsku4" };

			// ------------ Act
			var getProductsTaskRest = magentoServiceRest.GetProductsAsync( new[] { 0, 1 }, skus : skus, includeDetails : true );
			getProductsTaskRest.Wait();
			var inventoryToUpdate = getProductsTaskRest.Result.Where( p => p.ProductType == "simple" ).OrderBy( p => p.ProductId ).Select( p => new Inventory() { ItemId = p.ProductId, ProductId = p.ProductId, Sku = p.Sku, Qty = long.Parse( p.Qty ) + 1 } );
			magentoServiceRest.UpdateInventoryAsync( inventoryToUpdate );

			Task.Delay( 2000 ).Wait();
			var getProductsTaskSoap = magentoServiceSoap.GetProductsAsync( new[] { 0, 1 }, skus : skus, includeDetails : true );
			getProductsTaskSoap.Wait();
			var updatedInventory = getProductsTaskSoap.Result.Where( p => p.ProductType == "simple" ).OrderBy( p => p.ProductId ).Select( p => new Inventory() { ItemId = p.ProductId, ProductId = p.ProductId, Sku = p.Sku, Qty = long.Parse( p.Qty ) } );

			// ------------ Assert
			inventoryToUpdate.Should().BeEquivalentTo( updatedInventory );
		}
	}
}