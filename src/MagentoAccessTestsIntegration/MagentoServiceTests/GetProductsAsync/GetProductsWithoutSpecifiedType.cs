using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.GetProductsAsync
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class GetProductsWithoutSpecifiedType : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveProducts( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, credentials.Config.StoreCode, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			var getProductsTask1 = magentoService.GetProductsAsync( new[] { 0, 1 }, includeDetails : true, productType : "simple", excludeProductByType : true );
			var getProductsTask2 = magentoService.GetProductsAsync( new[] { 0, 1 }, includeDetails : true, productType : "bundle", excludeProductByType : true );
			Task.WhenAll( getProductsTask1, getProductsTask2 ).Wait();

			// ------------ Assert
			getProductsTask1.Result.Should().NotBeNullOrEmpty();
			getProductsTask2.Result.Should().NotBeNullOrEmpty();

			getProductsTask1.Result.All( x => x.ProductType != "simple" ).Should().BeTrue();
			getProductsTask2.Result.All( x => x.ProductType != "bundle" ).Should().BeTrue();
		}
	}
}