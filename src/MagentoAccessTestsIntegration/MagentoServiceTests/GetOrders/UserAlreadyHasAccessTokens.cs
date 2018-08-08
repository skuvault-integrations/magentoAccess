using System;
using System.Linq;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccessTestsIntegration.TestEnvironment;
using Netco.Logging;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.GetOrders
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class UserAlreadyHasAccessTokens : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveOrders( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			// var firstCreatedItem = this._orders[ credentials.AuthenticatedUserCredentials.SoapApiKey ].OrderBy( x => x.UpdatedAt ).First();
			// var lastCreatedItem = this._orders[ credentials.AuthenticatedUserCredentials.SoapApiKey ].OrderBy( x => x.UpdatedAt ).Last();

			// var modifiedFrom = new DateTime( ( firstCreatedItem.UpdatedAt ).Ticks, DateTimeKind.Utc ).AddSeconds( 1 );
			// var modifiedTo = new DateTime( ( lastCreatedItem.UpdatedAt ).Ticks, DateTimeKind.Utc ).AddSeconds( -1 );
			var modifiedFrom = new DateTime( 2018, 1, 28, 23, 23, 59 ).AddSeconds( 1 );
			var modifiedTo = new DateTime( 2018, 2, 2, 23, 30, 39 ).AddSeconds( -1 );
			var getOrdersTask = magentoService.GetOrdersAsync( modifiedFrom, modifiedTo, new Mark( "TEST-GET-ORDERS" ) );
			getOrdersTask.Wait();

			// ------------ Assert
			// var thatMustBeReturned = this._orders[ credentials.AuthenticatedUserCredentials.SoapApiKey ].Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.OrderIncrementalId ).ToList();
			// var thatWasReturned = getOrdersTask.Result.Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.OrderIncrementalId ).ToList();
			var thatWasReturned = getOrdersTask.Result.Select( x => x.OrderIncrementalId ).ToList();

			// thatWasReturned.Should().BeEquivalentTo( thatMustBeReturned );
			thatWasReturned.Should().NotBeNullOrEmpty();
			getOrdersTask.Result.All( x => x.Items.All( y => !string.IsNullOrWhiteSpace( y.Sku ) ) ).Should().BeTrue();
		}
	}
}