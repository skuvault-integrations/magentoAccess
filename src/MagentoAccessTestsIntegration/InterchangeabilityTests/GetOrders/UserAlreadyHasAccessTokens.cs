using System;
using System.Linq;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.InterchangeabilityTests.GetOrders
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class UserAlreadyHasAccessTokens : BaseTest	
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveOrders( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			// var firstCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).First();
			// var lastCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).Last();

			// var modifiedFrom = new DateTime( ( firstCreatedItem.UpdatedAt ).Ticks, DateTimeKind.Utc ).AddSeconds( 1 );
			// var modifiedTo = new DateTime( ( lastCreatedItem.UpdatedAt ).Ticks, DateTimeKind.Utc ).AddSeconds( -1 );
			var modifiedFrom = new DateTime( 2016, 1, 28, 23, 23, 59 ).AddSeconds( 1 );
			var modifiedTo = new DateTime( 2017, 7, 2, 23, 30, 39 ).AddSeconds( -1 );
			var getOrdersTask = magentoService.GetOrdersAsync( modifiedFrom, modifiedTo, new Mark( "TEST-GET-ORDERS" ) );
			getOrdersTask.Wait();

			// ------------ Assert
			// var thatMustBeReturned = this._orders[ credentials.StoreUrl ].Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.OrderIncrementalId ).ToList();
			// var thatWasReturned = getOrdersTask.Result.Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.OrderIncrementalId ).ToList();
			var thatWasReturned = getOrdersTask.Result.Select( x => x.OrderIncrementalId ).ToList();

			// thatWasReturned.Should().BeEquivalentTo( thatMustBeReturned );
			thatWasReturned.Should().NotBeNullOrEmpty();
		}
	}
}