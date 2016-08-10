using System;
using System.Linq;
using FluentAssertions;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	[ Parallelizable ]
	internal class InCorrectApiKey : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ReceiveNull( MagentoServiceSoapCredentials credentials )
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