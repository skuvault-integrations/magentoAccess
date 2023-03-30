using System.Collections;
using System.Linq;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using NUnit.Framework;
using static MagentoAccessTestsIntegration.TestEnvironment.TestStoresConfigsVault;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	public static class RepositoryTestCases
	{
		private const string DefaultRestRelativeUrl = "/index.php/rest/V1/"; 
		
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

				return GetActiveConfigs.Where( line => line.V2 == "1" && line.Rest == "1" ).Select( line =>
					new TestCaseData( 
						new RepositoryTestCase 
							{ 
								MagentoPass = MagentoPass.Create( line.MagentoPass ), 
								MagentoLogin = MagentoLogin.Create( line.MagentoLogin ), 
								Url = MagentoUrl.Create( line.MagentoUrl, DefaultRestRelativeUrl ) 
							} 
						).SetName( line.MagentoVersion ) );
			}
		}

		public static IEnumerable GetCliTestStoreCredentials()
		{
			var storeUrl = TestContext.Parameters["url"];
			var login = TestContext.Parameters["login"];
			var password = TestContext.Parameters["password"];
			var magentoUrl = MagentoUrl.Create( storeUrl, DefaultRestRelativeUrl );
			
			if ( !string.IsNullOrWhiteSpace( storeUrl )
				  && !string.IsNullOrWhiteSpace( login )
				  && !string.IsNullOrWhiteSpace( password ) )
			{
				return new [] 
					{ 
						new TestCaseData( 
							new RepositoryTestCase 
							{ 
								Url = magentoUrl, 
								MagentoLogin = MagentoLogin.Create( login ), 
								MagentoPass = MagentoPass.Create( password ) 
							} 
						) 
					};
			}
			
			return null;
		}
	}

	public sealed class RepositoryTestCase
	{
		public MagentoUrl Url { get; set; }
		public AuthorizationToken Token { get; set; }
		public MagentoLogin MagentoLogin { get; set; }
		public MagentoPass MagentoPass { get; set; }
	}
}