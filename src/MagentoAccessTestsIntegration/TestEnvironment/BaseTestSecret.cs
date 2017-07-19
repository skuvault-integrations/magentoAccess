using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	internal partial class BaseTest
	{
		private IEnumerable< MagentoServiceSoapCredentials > GetTestStoresCredentials()
		{
			//return Environment.ActiveEnvironmentRows.Select( line => new MagentoServiceSoapCredentials() { MagentoVersion = line.Version, SoapApiKey = line.MagentoPass, SoapApiUser = line.MagentoLogin, StoreUrl = line.MagentoUrl });
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
				return Environment.ActiveEnvironmentRows.Select( line => new TestCaseData( new BaseTest.MagentoServiceSoapCredentials { GetProductsThreadsLimit = 30, SessionLifeTimeMs = 3600000, MagentoVersion = line.ServiceVersion, SoapApiKey = line.MagentoPass, SoapApiUser = line.MagentoLogin, StoreUrl = line.MagentoUrl } ).SetName( line.MagentoVersion ) );
				//yield break;
			}
		}
	}
}