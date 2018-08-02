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
		public void ReceiveStoreVersion( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			magentoService.InitAsync( false ).Wait();
			var getOrdersTask = magentoService.DetermineMagentoVersionAsync();
			getOrdersTask.Wait();

			// ------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Should().NotBeNull();
			pingSoapInfo.Any( x => x.SoapWorks && string.Compare( x.Version, credentials.MagentoVersion, StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeTrue();
		}

		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveStoreVersionAndSetup( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", null, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			// ------------ Act
			var getOrdersTask = magentoService.DetermineMagentoVersionAndSetupServiceAsync();
			getOrdersTask.Wait();

			// ------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Should().NotBeNull();
			( pingSoapInfo.SoapWorks && string.Compare( pingSoapInfo.Version, credentials.MagentoVersion, StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeTrue();
		}
	}
}