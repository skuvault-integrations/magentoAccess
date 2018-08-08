using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess;
using MagentoAccess.Models.Credentials;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	internal partial class BaseTest
	{
		private IEnumerable< MagentoServiceCredentialsAndConfig > GetTestStoresCredentials()
		{
			//return Environment.GetActiveConfigs.Select( line => new MagentoServiceCredentialsAndConfig() { MagentoVersion = line.Version, SoapApiKey = line.MagentoPass, SoapApiUser = line.MagentoLogin, StoreUrl = line.MagentoUrl });
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
				return TestStoresConfigsVault.GetActiveConfigs.Select( line =>
				{
					var magentoServiceCredentialsAndConfig = new BaseTest.MagentoServiceCredentialsAndConfig
					{
						AuthenticatedUserCredentials = new MagentoAuthenticatedUserCredentials(
							line.MagentoLogin,
							line.MagentoPass,
							line.MagentoUrl,
							line.MagentoPass,
							line.MagentoLogin,
							line.MagentoLogin,
							line.MagentoPass,
							line.GetProductThreadsLimit,
							3600000,
							true
						),
						Config = new MagentoConfig
						{
							UseVersionByDefaultOnly = line.UseVersionByDefaultOnly,
							VersionByDefault = line.ServiceVersion,
						}
					};
					return new TestCaseData(
						magentoServiceCredentialsAndConfig ).SetName( line.MagentoVersion );
				} );
				//yield break;
			}
		}
	}
}