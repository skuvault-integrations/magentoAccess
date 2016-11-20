using System.Linq;
using FluentAssertions;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.GetProductsAsync
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class UserAlreadyHasAccessTokens : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs );

			// ------------ Act
			var getProductsTask = magentoService.GetProductsAsync( includeDetails : false );
			getProductsTask.Wait();

			// ------------ Assert
			var items = getProductsTask.Result.ToList();
			var dupl = items.GroupBy( x => x.ProductId ).Count( x => x.Count() > 1 );
			dupl.Should().Be( 0 );
			getProductsTask.Result.Should().NotBeNullOrEmpty();
		}
	}
}