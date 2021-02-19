using System;
using System.Linq;
using System.Threading;
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
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser,
				credentials.AuthenticatedUserCredentials.SoapApiKey,
				"null",
				"null",
				"null",
				"null",
				credentials.AuthenticatedUserCredentials.BaseMagentoUrl,
				"http://w.com",
				"http://w.com",
				"http://w.com",
				credentials.Config.VersionByDefault,
				credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit,
				credentials.AuthenticatedUserCredentials.SessionLifeTimeMs,
				false,
				credentials.Config.UseVersionByDefaultOnly,
				ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			magentoService.InitAsync( false ).Wait();
			var getOrdersTask = magentoService.DetermineMagentoVersionAsync( CancellationToken.None );
			getOrdersTask.Wait();

			// ------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Should().NotBeNull();
			pingSoapInfo.Any( x => x.SoapWorks && string.Compare( x.ServiceUsedVersion,
				                       credentials.Config.VersionByDefault,
				                       StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeTrue();
		}

		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ),"TestStoresCredentials" ) ]
		public void ReceiveStoreVersionAndSetup( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser,
				credentials.AuthenticatedUserCredentials.SoapApiKey,
				"null",
				"null",
				"null",
				"null",
				credentials.AuthenticatedUserCredentials.BaseMagentoUrl,
				"http://w.com",
				"http://w.com",
				"http://w.com",
				credentials.Config.VersionByDefault,
				credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit,
				credentials.AuthenticatedUserCredentials.SessionLifeTimeMs,
				false,
				credentials.Config.UseVersionByDefaultOnly,
				ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			var getOrdersTask = magentoService.DetermineMagentoVersionAndSetupServiceAsync( CancellationToken.None );
			getOrdersTask.Wait();

			// ------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Should().NotBeNull();
			( pingSoapInfo.SoapWorks && string.Compare( pingSoapInfo.ServiceUsedVersion, credentials.Config.VersionByDefault, StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeTrue();
		}
	}
}