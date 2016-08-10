using System;
using System.Linq;
using FluentAssertions;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	[ Parallelizable ]
	internal class DetermineMagentoVersionTest : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void DetermineMagentoVersionAsync_CorrectCredentials_ReceiveStoreVersion( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", null );

			// ------------ Act
			var getOrdersTask = magentoService.DetermineMagentoVersionAsync();
			getOrdersTask.Wait();

			// ------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Should().NotBeNull();
			pingSoapInfo.Any( x => x.SoapWorks && string.Compare( x.Version, credentials.MagentoVersion, StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeTrue();
		}

		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void DetermineMagentoVersionAsync_InCorrectApiKey_ReceiveNull( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey + "_incorrectKey", "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion );

			// ------------ Act
			var getOrdersTask = magentoService.DetermineMagentoVersionAsync();
			getOrdersTask.Wait();

			// ------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Any( x => x.SoapWorks && string.Compare( x.Version, credentials.MagentoVersion, StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeFalse();
		}
	}
}