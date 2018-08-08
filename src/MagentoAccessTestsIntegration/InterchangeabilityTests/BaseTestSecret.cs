using System.Collections;
using System.Linq;
using MagentoAccess;
using MagentoAccess.Models.Credentials;
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
				return TestStoresConfigsVault.GetActiveConfigs.Where( line => line.V2 == "1" && line.Rest == "1" ).Select( line =>
					new TestCaseData(
						new BaseTest.MagentoServiceCredentialsAndConfig
						{
							AuthenticatedUserCredentials = new MagentoAuthenticatedUserCredentials(
								line.MagentoLogin,
								line.MagentoPass,
								line.MagentoUrl,
								line.MagentoPass,
								line.MagentoLogin,
								line.MagentoLogin,
								line.MagentoPass,
								30,
								3600000,
								true
							),

							Config = new MagentoConfig()
							{
								UseVersionByDefaultOnly = line.UseVersionByDefaultOnly,
								VersionByDefault = "R2.1.0.0",
							}
						},
						new BaseTest.MagentoServiceCredentialsAndConfig
						{
							AuthenticatedUserCredentials = new MagentoAuthenticatedUserCredentials(
								line.MagentoLogin,
								line.MagentoPass,
								line.MagentoUrl,
								line.MagentoPass,
								line.MagentoLogin,
								line.MagentoLogin,
								line.MagentoPass,
								30,
								3600000,
								true
							),

							Config = new MagentoConfig()
							{
								UseVersionByDefaultOnly = line.UseVersionByDefaultOnly,
								VersionByDefault = line.ServiceVersion,
							}
						}
					).SetName( line.MagentoVersion ) ).ToList();
			}
		}
	}
}