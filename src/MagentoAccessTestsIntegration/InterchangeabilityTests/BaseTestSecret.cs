using System.Collections;
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
				yield return new TestCaseData(
					new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "R2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-1-0-0-ce" },
					new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-1-0-0-ce" }
				).SetName( "magento-2-1-5-0-ce" );
			}
		}
	}
}