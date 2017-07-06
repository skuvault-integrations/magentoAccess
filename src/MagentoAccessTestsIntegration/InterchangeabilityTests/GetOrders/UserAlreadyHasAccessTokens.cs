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
		public void ReceiveOrders( MagentoServiceSoapCredentials credentialsRest, MagentoServiceSoapCredentials credentialsSoap )
		{
			// ------------ Arrange
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.SoapApiUser, credentialsRest.SoapApiKey, "null", "null", "null", "null", credentialsRest.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.MagentoVersion, credentialsRest.GetProductsThreadsLimit, credentialsRest.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );
			var magentoServiceSoap = this.CreateMagentoService( credentialsSoap.SoapApiUser, credentialsSoap.SoapApiKey, "null", "null", "null", "null", credentialsSoap.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsSoap.MagentoVersion, credentialsSoap.GetProductsThreadsLimit, credentialsSoap.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			//------------Act
			var modifiedFrom = new DateTime( 2016, 1, 28, 23, 23, 59 ).AddSeconds( 1 );
			var modifiedTo = new DateTime( 2017, 7, 2, 23, 30, 39 ).AddSeconds( -1 );

			var getOrdersTask = magentoServiceRest.GetOrdersAsync( modifiedFrom, modifiedTo, new Mark( "TEST-GET-ORDERS" ) );
			getOrdersTask.Wait();
			var getOrdersTask2 = magentoServiceSoap.GetOrdersAsync( modifiedFrom, modifiedTo, new Mark( "TEST-GET-ORDERS-2" ) );
			getOrdersTask2.Wait();

			//------------Assert
			var thatWasReturned = getOrdersTask.Result.OrderBy( x => x.OrderIncrementalId ).ToList();
			var thatWasReturned2 = getOrdersTask2.Result.OrderBy( x => x.OrderIncrementalId ).ToList();

			thatWasReturned.Should().BeEquivalentTo( thatWasReturned2 );
		}
	}
}