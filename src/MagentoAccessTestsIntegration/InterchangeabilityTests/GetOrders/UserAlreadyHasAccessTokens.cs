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
		[ TestCaseSource( typeof( InterchangeabilityTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveOrders( MagentoServiceSoapCredentials credentials, MagentoServiceSoapCredentials credentials2 )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );
			var magentoService2 = this.CreateMagentoService( credentials2.SoapApiUser, credentials2.SoapApiKey, "null", "null", "null", "null", credentials2.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials2.MagentoVersion, credentials2.GetProductsThreadsLimit, credentials2.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			// var firstCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).First();
			// var lastCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).Last();

			// var modifiedFrom = new DateTime( ( firstCreatedItem.UpdatedAt ).Ticks, DateTimeKind.Utc ).AddSeconds( 1 );
			// var modifiedTo = new DateTime( ( lastCreatedItem.UpdatedAt ).Ticks, DateTimeKind.Utc ).AddSeconds( -1 );
			var modifiedFrom = new DateTime( 2016, 1, 28, 23, 23, 59 ).AddSeconds( 1 );
			var modifiedTo = new DateTime( 2017, 7, 2, 23, 30, 39 ).AddSeconds( -1 );

			var getOrdersTask = magentoService.GetOrdersAsync( modifiedFrom, modifiedTo, new Mark( "TEST-GET-ORDERS" ) );
			getOrdersTask.Wait();
			var getOrdersTask2 = magentoService2.GetOrdersAsync(modifiedFrom, modifiedTo, new Mark("TEST-GET-ORDERS-2"));
			getOrdersTask2.Wait();

			// ------------ Assert
			// var thatMustBeReturned = this._orders[ credentials.StoreUrl ].Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.OrderIncrementalId ).ToList();
			// var thatWasReturned = getOrdersTask.Result.Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.OrderIncrementalId ).ToList();
			var thatWasReturned = getOrdersTask.Result.OrderBy( x => x.OrderIncrementalId ).ToList();
			var thatWasReturned2 = getOrdersTask2.Result.OrderBy( x => x.OrderIncrementalId ).ToList();

			// thatWasReturned.Should().BeEquivalentTo( thatMustBeReturned );
			thatWasReturned.Should().BeEquivalentTo(thatWasReturned2);
		}
	}
}