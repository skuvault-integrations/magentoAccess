using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.GetProductsAsync
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class GetOnlySpecialByTypeProducs : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			var getProductsTask1 = magentoService.GetProductsAsync( new[] { 0, 1 }, includeDetails : true, productType : "simple", excludeProductByType : false, mark : new Mark( nameof( ReceiveProducts ) + "_s" ) );
			var getProductsTask2 = magentoService.GetProductsAsync( new[] { 0, 1 }, includeDetails : true, productType : "bundle", excludeProductByType : false, mark : new Mark( nameof( ReceiveProducts ) + "_b" ) );
			Task.WhenAll( getProductsTask1, getProductsTask2 ).Wait();

			// ------------ Assert
			getProductsTask1.Result.Should().NotBeNullOrEmpty();
			getProductsTask2.Result.Should().NotBeNullOrEmpty();

			getProductsTask1.Result.All( x => x.ProductType == "simple" ).Should().BeTrue();
			getProductsTask2.Result.All( x => x.ProductType == "bundle" ).Should().BeTrue();
		}
	}
}