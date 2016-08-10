using System;
using System.Linq;
using FluentAssertions;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.DetermineMagentoVersion
{
	[ TestFixture ]
	[ Parallelizable ]
	internal class CorrectCredentials : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveStoreVersion( MagentoServiceSoapCredentials credentials )
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
	}
}