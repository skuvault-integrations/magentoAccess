using MagentoAccess;
using MagentoAccess.Models.Credentials;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	public class BaseTest
	{
		private TestData _testData;
		private MagentoConsumerCredentials _consumer;
		private MagentoUrls _authorityUrls;
		private MagentoAccessToken _accessToken;
		protected MagentoService _service;

		[ SetUp ]
		public void Setup()
		{
			this._testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv" );
			this._consumer = this._testData.GetMagentoConsumerCredentials();
			this._authorityUrls = this._testData.GetMagentoUrls();
			this._accessToken = this._testData.GetMagentoAccessToken();
			this._service = new MagentoService( new MagentoAuthenticatedUserCredentials( this._accessToken.AccessToken, this._accessToken.AccessTokenSecret, this._authorityUrls.MagentoBaseUrl, this._consumer.Key, this._consumer.Secret ) );
			this._service.AfterGettingToken += this._testData.CreateAccessTokenFile;
		}
	}
}