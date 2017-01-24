using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	internal partial class BaseTest
	{
		private IEnumerable< MagentoServiceSoapCredentials > GetTestStoresCredentials()
		{
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-2-0-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.1", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-2-1-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.2", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-2-2-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.3", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-2-3-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.2.4", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-2-4-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "2.0.2.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-2-0-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-1-0-0-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "2.0.7.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-7-0-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.8.1.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-8-1-0-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.0.1", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-0-1-ce" };
			//yield return new MagentoServiceSoapCredentials() { MagentoVersion = "1.9.1.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-1-0-ce" };
			yield break;
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
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "1.14.3.1", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/31/" } ).SetName( "magento-1-14-3-1-ee" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "1.8.1.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-8-1-0-ce" } ).SetName( "magento-1-8-1-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "1.9.0.1", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-0-1-ce" } ).SetName( "magento-1-9-0-1-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "1.9.2.1", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-2-1-ce" } ).SetName( "magento-1-9-2-1-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "1.9.2.2", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-2-2-ce" } ).SetName( "magento-1-9-2-2-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "1.9.2.3", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-2-3-ce" } ).SetName( "magento-1-9-2-3-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "1.9.2.4", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-2-4-ce" } ).SetName( "magento-1-9-2-4-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "1.9.3.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-3-0-ce" } ).SetName( "magento-1-9-3-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "1.9.3.1", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-1-9-3-1-ce" } ).SetName( "magento-1-9-3-1-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.0.2.0", SoapApiKey = "password", SoapApiUser = "MaxKitsenko", StoreUrl = "http://yourmagentourl/Magento-2-0-2-0-ce" } ).SetName( "magento-2-0-2-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.0.2.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-4-0-ce" } ).SetName( "magento-2-0-4-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.0.2.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-5-0-ce" } ).SetName( "magento-2-0-5-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.0.2.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-6-0-ce" } ).SetName( "magento-2-0-6-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-7-0-ce" } ).SetName( "magento-2-0-7-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaxKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-8-0-ce" } ).SetName( "magento-2-0-8-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaxKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-9-0-ce" } ).SetName( "magento-2-0-9-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-10-0-ce" } ).SetName( "magento-2-0-10-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-0-11-0-ce" } ).SetName( "magento-2-0-11-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaximKitsenko", StoreUrl = "http://yourmagentourl/magento-2-1-0-0-ce" } ).SetName( "magento-2-1-0-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaxKitsenko", StoreUrl = "http://yourmagentourl/magento-2-1-2-0-ce" } ).SetName( "magento-2-1-2-0-ce" );
				yield return new TestCaseData( new BaseTest.MagentoServiceSoapCredentials() { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = "2.1.0.0", SoapApiKey = "password", SoapApiUser = "MaxKitsenko", StoreUrl = "http://yourmagentourl/magento-2-1-3-0-ce/" } ).SetName( "magento-2-1-3-0-ce" );
			}
		}
	}
}