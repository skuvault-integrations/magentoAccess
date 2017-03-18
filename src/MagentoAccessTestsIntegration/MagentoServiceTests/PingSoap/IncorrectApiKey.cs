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
		public void ThrowException( MagentoServiceSoapCredentials credentials )
		{
			// ------------ Arrange

			// ------------ Act
			Action act = () =>
			{
				var service = this.CreateMagentoService( credentials.SoapApiUser, "incorrectKey", "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion, credentials.GetProductsThreadsLimit, credentials.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

				var magentoInfoAsyncTask = service.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			// ------------ Assert
			act.ShouldThrow< Exception >();
		}
	}
}