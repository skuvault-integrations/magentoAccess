using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using MagentoAccess;
using MagentoAccessTestsIntegration.TestEnvironment;
using Netco.Logging;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.DetermineMagentoVersion
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class InCorrectApiKey : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveNull( MagentoServiceCredentialsAndConfig credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.AuthenticatedUserCredentials.SoapApiUser, credentials.AuthenticatedUserCredentials.SoapApiKey + "_incorrectKey", "null", "null", "null", "null", credentials.AuthenticatedUserCredentials.BaseMagentoUrl, "http://w.com", "http://w.com", "http://w.com", credentials.Config.VersionByDefault, credentials.AuthenticatedUserCredentials.GetProductsThreadsLimit, credentials.AuthenticatedUserCredentials.SessionLifeTimeMs, true, credentials.Config.UseVersionByDefaultOnly, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			var getOrdersTask = magentoService.DetermineMagentoVersionAsync( Mark.Blank(), CancellationToken.None );
			getOrdersTask.Wait();

			// ------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Any( x => x.SoapWorks && string.Compare( x.StoreVersion, credentials.Config.VersionByDefault, StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeFalse();
		}
	}
}