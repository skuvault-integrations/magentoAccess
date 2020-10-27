using System;
using System.Threading;
using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.PingSoap
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class CorrectCredentialsAndUrl : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void NoExceptionThrow( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );
			magentoService.DetermineMagentoVersionAndSetupServiceAsync( CancellationToken.None ).GetAwaiter().GetResult();

			// ------------ Act
			Action act = () =>
			{
				var magentoInfoAsyncTask = magentoService.PingSoapAsync( CancellationToken.None );
				magentoInfoAsyncTask.Wait();
			};

			// ------------ Assert
			act.ShouldNotThrow< Exception >();
		}
	}
}