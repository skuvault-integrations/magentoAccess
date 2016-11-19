using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.GetProductsAsync
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class GetProducsWithUpdateTimeFilter : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs );
			var updatedFrom = DateTime.UtcNow.AddMonths( -1 );

			// ------------ Act
			var getProductsTask1 = magentoService.GetProductsAsync( updatedFrom : updatedFrom, includeDetails : true );
			var getProductsTask2 = magentoService.GetProductsAsync( includeDetails : true );
			Task.WhenAll( getProductsTask1, getProductsTask2 ).Wait();

			// ------------ Assert
			getProductsTask1.Result.Should().NotBeNullOrEmpty();
			getProductsTask2.Result.Should().NotBeNullOrEmpty();

			getProductsTask1.Result.All( x => x.UpdatedAt.ToDateTimeOrDefault() >= updatedFrom ).Should().BeTrue();
			getProductsTask2.Result.Count().Should().BeGreaterThan( getProductsTask1.Result.Count() );
		}
	}
}