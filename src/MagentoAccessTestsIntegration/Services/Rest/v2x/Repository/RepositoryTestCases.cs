using System.Collections;
using System.Linq;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using NUnit.Framework;
using static MagentoAccessTestsIntegration.TestEnvironment.TestStoresConfigsVault;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	public static class RepositoryTestCases
	{
		/// <summary>
		/// GetTestStoresCredentials shoud return the same credentials as this method
		/// </summary>
		/// <returns></returns>
		public static IEnumerable TestCases
		{
			get
			{
				var cliTestStoreCredentials = GetCliTestStoreCredentials();

				if ( cliTestStoreCredentials != null )
					return cliTestStoreCredentials;

				return TestEnvironment.TestStoresConfigsVault.GetActiveConfigs.Where( line => line.V2 == "1" && line.Rest == "1" ).Select( line =>
					new TestCaseData( new RepositoryTestCase { MagentoPass = MagentoPass.Create( line.MagentoPass ), MagentoLogin = MagentoLogin.Create( line.MagentoLogin ), Url = MagentoUrl.Create( line.MagentoUrl ) } ).SetName( line.MagentoVersion ) );
			}
		}

		public static IEnumerable GetCliTestStoreCredentials()
		{
			string url = TestContext.Parameters["url"];
			string login = TestContext.Parameters["login"];
			string password = TestContext.Parameters["password"];

			if (!( string.IsNullOrEmpty( url )
				  || string.IsNullOrEmpty( login )
				  || string.IsNullOrEmpty( password ) ) )
				return new TestCaseData[] { new TestCaseData( new RepositoryTestCase() { Url = MagentoUrl.Create( url ), MagentoLogin = MagentoLogin.Create( login ), MagentoPass = MagentoPass.Create( password ) } ) };
			
			return null;
		}
	}

	public class RepositoryTestCase
	{
		public MagentoUrl Url { get; set; }
		public AuthorizationToken Token { get; set; }
		public MagentoLogin MagentoLogin { get; set; }
		public MagentoPass MagentoPass { get; set; }
	}
}