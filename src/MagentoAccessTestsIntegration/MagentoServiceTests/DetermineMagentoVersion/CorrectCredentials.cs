using System;
using System.Linq;
using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.DetermineMagentoVersion
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class CorrectCredentials : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveStoreVersion( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.SoapApiKey, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			magentoService.InitAsync( false ).Wait();
			var getOrdersTask = magentoService.DetermineMagentoVersionAsync();
			getOrdersTask.Wait();

			// ------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Should().NotBeNull();
			pingSoapInfo.Any( x => x.SoapWorks && string.Compare( x.Version, credentials.Config.VersionByDefault, StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeTrue();
		}

		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveStoreVersionAndSetup( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey, "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.SoapApiKey, "http://w.com", "http://w.com", "http://w.com", null, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, false, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			var getOrdersTask = magentoService.DetermineMagentoVersionAndSetupServiceAsync();
			getOrdersTask.Wait();

			// ------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Should().NotBeNull();
			( pingSoapInfo.SoapWorks && string.Compare( pingSoapInfo.Version, credentials.Config.VersionByDefault, StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeTrue();
		}
	}
}