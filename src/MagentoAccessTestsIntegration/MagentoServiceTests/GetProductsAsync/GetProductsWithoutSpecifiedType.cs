using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.GetProductsAsync
{
	[ TestFixture ]
	[ Parallelizable ]
	internal class GetProductsWithoutSpecifiedType : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs );

			// ------------ Act
			var getProductsTask1 = magentoService.GetProductsAsync( includeDetails : true, productType : "simple", excludeProductByType : true );
			var getProductsTask2 = magentoService.GetProductsAsync( includeDetails : true, productType : "bundle", excludeProductByType : true );
			Task.WhenAll( getProductsTask1, getProductsTask2 ).Wait();

			// ------------ Assert
			getProductsTask1.Result.Should().NotBeNullOrEmpty();
			getProductsTask2.Result.Should().NotBeNullOrEmpty();

			getProductsTask1.Result.All( x => x.ProductType != "simple" ).Should().BeTrue();
			getProductsTask2.Result.All( x => x.ProductType != "bundle" ).Should().BeTrue();
		}
	}
}