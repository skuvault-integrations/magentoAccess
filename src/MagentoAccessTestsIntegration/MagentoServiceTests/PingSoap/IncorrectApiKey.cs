using System;
using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.PingSoap
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class IncorrectApiKey : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ThrowException( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange

			// ------------ Act
			Action act = () =>
			{
				var service = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, "incorrectKey", "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.SoapApiKey, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

				var magentoInfoAsyncTask = service.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			// ------------ Assert
			act.ShouldThrow< Exception >();
		}
	}
}