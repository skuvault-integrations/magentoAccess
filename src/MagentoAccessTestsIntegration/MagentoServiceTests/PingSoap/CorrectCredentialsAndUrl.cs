using System;
using System.Threading;
using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using Netco.Logging;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.PingSoap
{
	[ Explicit ]
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
			IMagentoService magentoService = this.InitMagentoService( credentials );
			magentoService.DetermineMagentoVersionAndSetupServiceAsync( Mark.Blank(), CancellationToken.None ).GetAwaiter().GetResult();

			// ------------ Act
			Action act = () =>
			{
				var magentoInfoAsyncTask = magentoService.PingSoapAsync( CancellationToken.None );
				magentoInfoAsyncTask.Wait();
			};

			// ------------ Assert
			act.Should().NotThrow< Exception >();
		}
		
		private IMagentoService InitMagentoService( MagentoServiceCredentialsAndConfig credentials )
		{
			return this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", 
				credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, 
				credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs,
				false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );
		}
	}
}