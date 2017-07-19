using System.Collections;
using System.Linq;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.InterchangeabilityTests
{
	public static class InterchangeabilityTestCases
	{
		/// <summary>
		/// GetTestStoresCredentials shoud return the same credentials as this method
		/// </summary>
		/// <returns></returns>
		public static IEnumerable TestStoresCredentials
		{
			get
			{
				return Environment.ActiveEnvironmentRows.Where( line => line.V2 == "1" && line.Rest == "1" ).Select( line =>
					new TestCaseData(
						new BaseTest.MagentoServiceSoapCredentials { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "R2.1.0.0", SoapApiKey = line.MagentoPass, SoapApiUser = line.MagentoLogin, StoreUrl = line.MagentoUrl },
						new BaseTest.MagentoServiceSoapCredentials { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = line.ServiceVersion, SoapApiKey = line.MagentoPass, SoapApiUser = line.MagentoLogin, StoreUrl = line.MagentoUrl }
					).SetName( line.MagentoVersion ) ).ToList();
			}
		}
	}
}