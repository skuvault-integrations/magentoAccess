using System.Collections;
using MagentoAccess.Services.Rest.v2x.WebRequester;
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
				yield return new TestCaseData( new RepositoryTestCase() { MagentoPass = MagentoPass.Create("MaxKitsenko"), MagentoLogin = MagentoLogin.Create("MaxKitsenko"), Url = MagentoUrl.Create("http://yourmagentostore") } ).SetName( "magento-2-0-2-0-ce" );
				yield return new TestCaseData( new RepositoryTestCase() { MagentoPass = MagentoPass.Create("MaxKitsenko"), MagentoLogin = MagentoLogin.Create("MaxKitsenko"), Url = MagentoUrl.Create("http://yourmagentostore") } ).SetName( "magento-2-1-0-0-ce" );
				yield return new TestCaseData( new RepositoryTestCase() { MagentoPass = MagentoPass.Create("MaxKitsenko"), MagentoLogin = MagentoLogin.Create("MaxKitsenko"), Url = MagentoUrl.Create("http://yourmagentostore") } ).SetName( "magento-2-0-7-0-ce" );
				yield break;
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