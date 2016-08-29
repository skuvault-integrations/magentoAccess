using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	internal partial class BaseTest
	{
		private IEnumerable<MagentoServiceSoapCredentials> GetTestStoresCredentials()
		{
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.0", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-2-0-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.1", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-2-1-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.2", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-2-2-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.3", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-2-3-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "2.0.2.0", SoapApiKey = "123123", SoapApiUser = "SoapUser", StoreUrl = "http://127.0.0.1:7777/magento-2-0-2-0-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.8.1.0", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-8-1-0-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.0.1", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-0-1-ce" };
			yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.1.0", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://199.48.164.39:4423//magento-1-9-1-0-ce" };
		}
	}


	public static class GeneralTestCases
	{
		/// <summary>
		/// GetTestStoresCredentials shoud return the same credentials as this method
		/// </summary>
		/// <returns></returns>
		public static IEnumerable TestStoresCredentials
		{
			get
			{
				//yield return new TestCaseData(new BaseTest.MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.0", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-2-0-ce" }).SetName("magento-1-9-2-0-ce");
				//yield return new TestCaseData(new BaseTest.MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.1", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-2-1-ce" }).SetName("magento-1-9-2-1-ce");
				//yield return new TestCaseData(new BaseTest.MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.2", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-2-2-ce" }).SetName("magento-1-9-2-2-ce");
				//yield return new TestCaseData(new BaseTest.MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.3", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-2-3-ce" }).SetName("magento-1-9-2-2-ce");
				//yield return new TestCaseData(new BaseTest.MagentoServiceSoapCredentials() { MagentoVersion = "2.0.2.0", SoapApiKey = "123123", SoapApiUser = "SoapUser", StoreUrl = "http://127.0.0.1:7777/magento-2-0-2-0-ce" }).SetName("magento-2-0-2-0-ce");
				//yield return new TestCaseData(new BaseTest.MagentoServiceSoapCredentials() { MagentoVersion = "1.8.1.0", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-8-1-0-ce" }).SetName("magento-1-8-1-0-ce");
				//yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { MagentoVersion = "1.9.0.1", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://127.0.0.1:7777/magento-1-9-0-1-ce" } ).SetName( "magento-1-9-0-1-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { MagentoVersion = "1.9.1.0", SoapApiKey = "123123", SoapApiUser = "ForAutoTestingDoNotChangePass123123", StoreUrl = "http://199.48.164.39:4423//magento-1-9-1-0-ce" } ).SetName( "magento-1-9-0-1-ce" );
			}
		}
	}


}