using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	internal partial class BaseTest
	{
		private IEnumerable< MagentoServiceSoapCredentials > GetTestStoresCredentials()
		{
			yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.0", SoapApiKey = "123", SoapApiUser = "MaxKits", StoreUrl = "http://localhost:4423/magento-1-9-2-0-ce" };
		}
	}


	public class MagentoTestCases
	{
		public IEnumerable TestStoresCredentials()
		{
			yield return new TestCaseData(new BaseTest.MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.0", SoapApiKey = "123", SoapApiUser = "MaxKits", StoreUrl = "http://localhost:4423/magento-1-9-2-0-ce" }).SetName("magento-1-9-2-0-ce");
		}
	}


}