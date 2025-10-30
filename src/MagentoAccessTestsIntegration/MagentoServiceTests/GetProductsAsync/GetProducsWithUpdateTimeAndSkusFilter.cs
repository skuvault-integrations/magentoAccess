using System;
using System.Linq;
using System.Threading;
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
	internal class GetProducsWithUpdateTimeAndSkusFilter : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveProducts( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );
			var updatedFrom = DateTime.UtcNow.AddMonths( -7 );

			// ------------ Act
			var getProductsTask1 = magentoService.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, includeDetails : true, updatedFrom : updatedFrom );
			getProductsTask1.Wait();
			var expectedSku = getProductsTask1.Result.Take( 2 ).Select( x => x.Sku ).ToList();

			var getProductsBySkusTask2 = magentoService.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, includeDetails : true, updatedFrom : updatedFrom, skus : expectedSku );
			getProductsBySkusTask2.Wait();
			//------------Assert
			getProductsTask1.Result.Should().NotBeNullOrEmpty();
			getProductsTask1.Result.All( x => x.UpdatedAt.ToDateTimeOrDefault() >= updatedFrom ).Should().BeTrue();
			getProductsBySkusTask2.Result.Count().Should().Be( expectedSku.Count() );
			getProductsBySkusTask2.Result.Select( x => x.Sku ).Should().BeEquivalentTo( expectedSku );
		}
	}
}