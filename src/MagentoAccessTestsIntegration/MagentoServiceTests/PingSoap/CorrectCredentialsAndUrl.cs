using System;
using System.Threading;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Exceptions;
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
			IMagentoService magentoService = this.InitMagentoService( credentials );
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

		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void MagentoService_NoExceptionThrow_WhenPermanentRedirectExceptionOccurs_andRelativeUrlChanged( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			IMagentoService magentoService = null;
			try
			{
				magentoService = this.InitMagentoService( credentials );
			}
			catch( Exception exception )
			{
				if ( !IsPermanentRedirectException( exception ) ) 
					throw;

				credentials.AuthenticatedUserCredentials.SetRelativeUrl( "/rest/V1/" );
				magentoService = this.InitMagentoService( credentials );
			}

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

		private IMagentoService InitMagentoService( MagentoServiceCredentialsAndConfig credentials )
		{
			return this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", 
				credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, 
				credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs,
				false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems, credentials.AuthenticatedUserCredentials.RelativeUrl );
		}

		private static bool IsPermanentRedirectException( Exception exception )
		{
			return exception?.InnerException is PermanentRedirectException;
		}
	}
}