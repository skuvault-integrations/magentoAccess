using System.Collections;
using System.Linq;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

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
				return Environment.ActiveEnvironmentRows.Where( line => line.V2 == "1" && line.Rest == "1" ).Select( line =>
					new TestCaseData( new RepositoryTestCase { MagentoPass = MagentoPass.Create( line.MagentoPass ), MagentoLogin = MagentoLogin.Create( line.MagentoLogin ), Url = MagentoUrl.Create( line.MagentoUrl ) } ).SetName( line.MagentoVersion ) );
			}
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