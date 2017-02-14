using System;
using FluentAssertions;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.MagentoServiceTests.PingSoap
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class IncorrectUrl : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void ThrowException( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange

			// ------------ Act
			Action act = () =>
			{
				var service = this.CreateMagentoService( "incorrectuser", credentials.SoapApiKey, "null", "null", "null", "null", "http://w.com", "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false );

				var magentoInfoAsyncTask = service.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			// ------------ Assert
			act.ShouldThrow< Exception >();
		}
	}
}