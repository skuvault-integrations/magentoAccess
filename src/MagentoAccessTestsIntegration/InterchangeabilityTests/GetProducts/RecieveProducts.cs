using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.InterchangeabilityTests.GetProducts
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class InterchangeabilityTests_RecieveProducts : BaseTest
	{
		[Test]
		[TestCaseSource( typeof( InterchangeabilityTestCases ), "TestStoresCredentials" )]
		public void ReceiveProductsWithoutDetalis( MagentoServiceSoapCredentials credentialsRest, MagentoServiceSoapCredentials credentialsSoap )
		{
			// ------------ Arrange
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.SoapApiUser, credentialsRest.SoapApiKey, "null", "null", "null", "null", credentialsRest.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.MagentoVersion, credentialsRest.GetProductsThreadsLimit, credentialsRest.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );
			var magentoServiceSoap = this.CreateMagentoService( credentialsSoap.SoapApiUser, credentialsSoap.SoapApiKey, "null", "null", "null", "null", credentialsSoap.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsSoap.MagentoVersion, credentialsSoap.GetProductsThreadsLimit, credentialsSoap.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			var swR = Stopwatch.StartNew();
			var getProductsTaskRest = magentoServiceRest.GetProductsAsync( new[] { 0, 1 }, includeDetails : false );
			getProductsTaskRest.Wait();
			swR.Stop();

			Task.Delay( 2000 ).Wait();
			var swS = Stopwatch.StartNew();
			var getProductsTaskSoap = magentoServiceSoap.GetProductsAsync( new[] { 0, 1 }, includeDetails : false );
			getProductsTaskSoap.Wait();
			swS.Stop();

			// ------------ Assert
			Console.WriteLine( "rest time: " + swR.Elapsed + " soap time: " + swS.Elapsed );
			
			var thatWasReturnedRest = getProductsTaskRest.Result.OrderBy(x => x.ProductId).ToList();
			var thatWasReturnedSoap = getProductsTaskSoap.Result.OrderBy(x => x.ProductId).ToList();
			//thatWasReturnedSoap.ForEach( soapPreparer );
			//thatWasReturnedRest.ForEach( restPreparer );
			thatWasReturnedRest.Should().BeEquivalentTo( thatWasReturnedSoap );
			swS.Elapsed.Should().BeGreaterThan( swR.Elapsed );
		}
	}
}
