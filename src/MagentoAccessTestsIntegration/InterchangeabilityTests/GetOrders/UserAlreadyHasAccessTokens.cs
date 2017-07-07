using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetOrders;
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

			Task.Delay(10000).Wait();
			var swR = Stopwatch.StartNew();
			var getOrdersTaskRest = magentoServiceRest.GetOrdersAsync( modifiedFrom, modifiedTo, new Mark( "TEST-GET-ORDERS" ) );
			getOrdersTaskRest.Wait();
			swR.Stop();
			Task.Delay(10000).Wait();
			var swS = Stopwatch.StartNew();
			var getOrdersTask2 = magentoServiceSoap.GetOrdersAsync( modifiedFrom, modifiedTo, new Mark( "TEST-GET-ORDERS-2" ) );
			getOrdersTask2.Wait();
			swS.Stop();
			Task.Delay(10000).Wait();
			//------------Assert
			var thatWasReturnedRest = getOrdersTaskRest.Result.OrderBy( x => x.OrderIncrementalId ).ToList();
			var thatWasReturnedSoap = getOrdersTask2.Result.OrderBy( x => x.OrderIncrementalId ).ToList();
			
			//these fields works in rest, but doesn't in soap. We need to skip them for assert.
			Action< Order > action = x =>
			{
				x.OrderId = string.Empty;
				x.ShippingAddressId = string.Empty;
				x.ShippingFirstname = string.Empty;
				x.ShippingLastname = string.Empty;
				x.ShippingMethod = string.Empty;
				x.ShippingName = string.Empty;
			};

			thatWasReturnedSoap.ForEach(action );
			thatWasReturnedRest.ForEach(action );


			thatWasReturnedRest.Should().BeEquivalentTo( thatWasReturnedSoap );
			swS.Elapsed.Should().BeGreaterThan( swR.Elapsed );
		}
	}
}