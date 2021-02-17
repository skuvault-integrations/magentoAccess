using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetOrders;
using MagentoAccessTestsIntegration.TestEnvironment;
using Netco.Logging;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.InterchangeabilityTests.GetOrders
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class ReceiveOrders : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( InterchangeabilityTestCases ), nameof(InterchangeabilityTestCases.TestStoresCredentials) ) ]
		public void OrdersReceived( MagentoServiceCredentialsAndConfig credentialsRest, MagentoServiceCredentialsAndConfig credentialsSoap )
		{
			// ------------ Arrange
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.AuthenticatedUserCredentials.SoapApiUser, credentialsRest.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentialsRest.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.Config.VersionByDefault, credentialsRest.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentialsRest.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentialsRest.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );
			var magentoServiceSoap = this.CreateMagentoService( credentialsSoap.AuthenticatedUserCredentials.SoapApiUser, credentialsSoap.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentialsSoap.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentialsSoap.Config.VersionByDefault, credentialsSoap.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentialsSoap.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentialsSoap.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

			//------------Act
			var modifiedFrom = new DateTime( 2017, 6, 2, 23, 23, 59 ).AddSeconds( 1 );
			var modifiedTo = new DateTime( 2017, 7, 2, 23, 30, 39 ).AddSeconds( -1 );

			var swR = Stopwatch.StartNew();
			var getOrdersTaskRest = magentoServiceRest.GetOrdersAsync( modifiedFrom, modifiedTo, new Mark( "TEST-GET-ORDERS" ) );
			getOrdersTaskRest.Wait();
			swR.Stop();

			Task.Delay( 10000 ).Wait();
			var swS = Stopwatch.StartNew();
			var getOrdersTask2 = magentoServiceSoap.GetOrdersAsync( modifiedFrom, modifiedTo, new Mark( "TEST-GET-ORDERS-2" ) );
			getOrdersTask2.Wait();
			swS.Stop();

			//------------Assert
			Console.WriteLine( "rest time: " + swR.Elapsed + " soap time: " + swS.Elapsed );


			//these fields works in rest, but doesn't in soap. We need to skip them for assert.
			Action< Order > soapPreparer = x =>
			{
				var ba = x.Addresses.FirstOrDefault( y => y.Item1 == AddressTypeEnum.Billing );
				if( ba?.Item2?.Street != null )
					ba.Item2.Street = ba.Item2.Street.Replace( ",", "" ).Replace( " ", "" ).Replace( "\n", "" );

				var sa = x.Addresses.FirstOrDefault( y => y.Item1 == AddressTypeEnum.Shipping );
				if( sa?.Item2?.Street != null )
					sa.Item2.Street = sa.Item2.Street.Replace( ",", "" ).Replace( " ", "" ).Replace( "\n", "" );
			};

			//soap uses null for such values, but rest empty string
			Action< Order > restPreparer = x =>
			{
				var ba = x.Addresses.FirstOrDefault( y => y.Item1 == AddressTypeEnum.Billing );
				if( ba?.Item2?.Region == string.Empty )
					ba.Item2.Region = null;

				if( ba?.Item2?.Street != null )
					ba.Item2.Street = ba.Item2.Street.Replace( ",", "" ).Replace( " ", "" ).Replace( "\n", "" );

				if( ba?.Item2?.Company == string.Empty )
					ba.Item2.Company = null;

				var sa = x.Addresses.FirstOrDefault( y => y.Item1 == AddressTypeEnum.Shipping );
				if( sa?.Item2?.Region == string.Empty )
					sa.Item2.Region = null;

				if( sa?.Item2?.Company == string.Empty )
					sa.Item2.Company = null;

				if( sa?.Item2?.Street != null )
					sa.Item2.Street = sa.Item2.Street.Replace( ",", "" ).Replace( " ", "" ).Replace( "\n", "" );

				x.OrderId = string.Empty;
				x.ShippingAddressId = null;
				x.ShippingFirstname = null;
				x.ShippingLastname = null;
				x.ShippingMethod = null;
				x.ShippingName = null;
			};

			var thatWasReturnedRest = getOrdersTaskRest.Result.OrderBy(x => x.OrderIncrementalId).ToList();
			var thatWasReturnedSoap = getOrdersTask2.Result.OrderBy(x => x.OrderIncrementalId).ToList();
			thatWasReturnedSoap.ForEach( soapPreparer );
			thatWasReturnedRest.ForEach( restPreparer );
			thatWasReturnedRest.Should().BeEquivalentTo( thatWasReturnedSoap );
			swS.Elapsed.Should().BeGreaterThan( swR.Elapsed );// I know this thing shouldn't be tested here. I hope, it will be fixed soon and will be moved to appropriate place
		}
	}
}