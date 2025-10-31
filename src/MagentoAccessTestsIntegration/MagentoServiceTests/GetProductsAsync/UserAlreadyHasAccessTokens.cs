using System.Linq;
using System.Threading;
using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.GetProductsAsync
{
	[ Explicit ]
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class UserAlreadyHasAccessTokens : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveProducts( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			var getProductsTask = magentoService.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, includeDetails : false );
			getProductsTask.Wait();

			// ------------ Assert
			var items = getProductsTask.Result.ToList();
			var dupl = items.GroupBy( x => x.ProductId ).Count( x => x.Count() > 1 );
			dupl.Should().Be( 0 );
			getProductsTask.Result.Should().NotBeNullOrEmpty();
		}
	}
}