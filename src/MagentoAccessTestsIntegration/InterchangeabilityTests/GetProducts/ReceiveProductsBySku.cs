using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.InterchangeabilityTests.GetProducts
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class InterchangeabilityTests_ReceiveProductsBySku : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( InterchangeabilityTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveProductsBySku( MagentoServiceSoapCredentials credentialsRest, MagentoServiceSoapCredentials credentialsSoap )
		{
			// ------------ Arrange
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.SoapApiUser, credentialsRest.SoapApiKey, "null", "null", "null", "null", credentialsRest.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.MagentoVersion, credentialsRest.GetProductsThreadsLimit, credentialsRest.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );
			var magentoServiceSoap = this.CreateMagentoService( credentialsSoap.SoapApiUser, credentialsSoap.SoapApiKey, "null", "null", "null", "null", credentialsSoap.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsSoap.MagentoVersion, credentialsSoap.GetProductsThreadsLimit, credentialsSoap.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );
			var skus = new string[] { "testsku1", "testsku2" };

			// ------------ Act
			var swR = Stopwatch.StartNew();
			var getProductsTaskRest = magentoServiceRest.GetProductsAsync( new[] { 0, 1 }, skus : skus, includeDetails : true );
			getProductsTaskRest.Wait();
			swR.Stop();

			Task.Delay( 500 ).Wait();
			var swS = Stopwatch.StartNew();
			var getProductsTaskSoap = magentoServiceSoap.GetProductsAsync( new[] { 0, 1 }, skus : skus, includeDetails : true );
			getProductsTaskSoap.Wait();
			swS.Stop();

			// ------------ Assert
			Console.WriteLine( "rest time: " + swR.Elapsed + " soap time: " + swS.Elapsed );

			var thatWasReturnedRest = getProductsTaskRest.Result.OrderBy( x => x.ProductId ).ToList();
			var thatWasReturnedSoap = getProductsTaskSoap.Result.OrderBy( x => x.ProductId ).ToList();
			thatWasReturnedSoap.ForEach( item =>
			{
				item.Categories = null;
			} );
			thatWasReturnedRest.Should().BeEquivalentTo( thatWasReturnedSoap );
			swS.Elapsed.Should().BeGreaterThan( swR.Elapsed );
		}
	}
}