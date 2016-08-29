using System;
using FluentAssertions;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.PingSoap
{
	[ TestFixture ]
	[ Parallelizable ]
	internal class IncorrectApiUser : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ThrowException( MagentoServiceSoapCredentials credentials )
		{
			// can be red for magento 2.0 since user doesn't used in magento2.0 version
			// ------------ Arrange

			// ------------ Act
			Action act = () =>
			{
				var service = this.CreateMagentoService( "incorrectuser", credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion );

				var magentoInfoAsyncTask = service.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			// ------------ Assert
			act.ShouldThrow< Exception >();
		}
	}
}