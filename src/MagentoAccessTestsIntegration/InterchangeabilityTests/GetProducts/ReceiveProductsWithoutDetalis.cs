﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.InterchangeabilityTests.GetProducts
{
	[ Explicit ]
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class ReceiveProductsWithoutDetalis : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( InterchangeabilityTestCases ), nameof(InterchangeabilityTestCases.TestStoresCredentials) ) ]
		public void ProductsReceived( MagentoServiceCredentialsAndConfig credentialsRest, MagentoServiceCredentialsAndConfig credentialsSoap )
		{
			// ------------ Arrange
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.AuthenticatedUserCredentials.SoapApiUser, credentialsRest.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentialsRest.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.Config.VersionByDefault, credentialsRest.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentialsRest.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentialsRest.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );
			var magentoServiceSoap = this.CreateMagentoService( credentialsSoap.AuthenticatedUserCredentials.SoapApiUser, credentialsSoap.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentialsSoap.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentialsSoap.Config.VersionByDefault, credentialsSoap.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentialsSoap.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentialsSoap.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			var swR = Stopwatch.StartNew();
			var getProductsTaskRest = magentoServiceRest.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, includeDetails : false );
			getProductsTaskRest.Wait();
			swR.Stop();

			Task.Delay( 500 ).Wait();
			var swS = Stopwatch.StartNew();
			var getProductsTaskSoap = magentoServiceSoap.GetProductsAsync( CancellationToken.None, new[] { 0, 1 }, includeDetails : false );
			getProductsTaskSoap.Wait();
			swS.Stop();

			// ------------ Assert
			Console.WriteLine( "rest time: " + swR.Elapsed + " soap time: " + swS.Elapsed );

			var thatWasReturnedRest = getProductsTaskRest.Result.OrderBy( x => x.ProductId ).ToList();
			var thatWasReturnedSoap = getProductsTaskSoap.Result.OrderBy( x => x.ProductId ).ToList();

			thatWasReturnedRest.Should().BeEquivalentTo( thatWasReturnedSoap );
			swS.Elapsed.Should().BeGreaterThan( swR.Elapsed );
		}
	}
}