using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.FillProductsDetails
{
	[ Explicit ]
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class GetProducsAndFillProductsDetails : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveProducts( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );
			var updatedFrom = DateTime.UtcNow.AddMonths( -15 );

			// ------------ Act
			var productsAsync = magentoService.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, stockItemsOnly : false, updatedFrom : updatedFrom );
			productsAsync.Wait();

			var fillProductsDetailsAsync = magentoService.FillProductsDetailsAsync( productsAsync.Result, CancellationToken.None );
			fillProductsDetailsAsync.Wait();

			// ------------ Assert
			productsAsync.Result.Should().NotBeNullOrEmpty();
			//productsAsync.Result.All( x => x.UpdatedAt.ToDateTimeOrDefault() >= updatedFrom ).Should().BeTrue();
			fillProductsDetailsAsync.Result.Should().NotBeNullOrEmpty();
			fillProductsDetailsAsync.Result.Count().Should().Be( productsAsync.Result.Count() );
			fillProductsDetailsAsync.Result.All( x => x.Categories != null && x.Images != null ).Should().BeTrue();
		}
	}
}