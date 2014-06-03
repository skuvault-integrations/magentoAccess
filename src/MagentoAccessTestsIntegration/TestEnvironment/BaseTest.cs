using MagentoAccess;
using MagentoAccess.Models.Services.Credentials;
using Netco.Logging;
using Netco.Logging.NLogIntegration;
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
		protected MagentoService _serviceNotAuth;

		[ SetUp ]
		public void Setup()
		{
			this._testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv" );
			this._consumer = this._testData.GetMagentoConsumerCredentials();
			this._authorityUrls = this._testData.GetMagentoUrls();
			this._accessToken = this._testData.GetMagentoAccessToken();

			this._service = ( this._accessToken == null || string.IsNullOrWhiteSpace( this._accessToken.AccessToken ) || string.IsNullOrWhiteSpace( this._accessToken.AccessTokenSecret ) ) ?
				new MagentoService( new MagentoNonAuthenticatedUserCredentials(
					this._consumer.Key,
					this._consumer.Secret,
					this._authorityUrls.MagentoBaseUrl,
					this._authorityUrls.RequestTokenUrl,
					this._authorityUrls.AuthorizeUrl,
					this._authorityUrls.AccessTokenUrl
					) ) :
				new MagentoService( new MagentoAuthenticatedUserCredentials(
					this._accessToken.AccessToken,
					this._accessToken.AccessTokenSecret,
					this._authorityUrls.MagentoBaseUrl,
					this._consumer.Secret,
					this._consumer.Key
					) );

			this._serviceNotAuth = new MagentoService( new MagentoNonAuthenticatedUserCredentials(
				this._consumer.Key,
				this._consumer.Secret,
				this._authorityUrls.MagentoBaseUrl,
				this._authorityUrls.RequestTokenUrl,
				this._authorityUrls.AuthorizeUrl,
				this._authorityUrls.AccessTokenUrl
				) );

			NetcoLogger.LoggerFactory = new NLogLoggerFactory();

			//if( this._accessToken == null )
			//{
			//	var authorizeTask = this._service.PopulateAccessToken();
			//	authorizeTask.Wait();
			//	this._testData.CreateAccessTokenFile( this._service.AccessToken, this._service.AccessTokenSecret );
			//}

			//this._service = new MagentoService( new MagentoAuthenticatedUserCredentials(
			//	this._accessToken.AccessToken,
			//	this._accessToken.AccessTokenSecret,
			//	this._authorityUrls.MagentoBaseUrl,
			//	this._consumer.Secret,
			//	this._consumer.Key
			//	) );

			this._service.AfterGettingToken += this._testData.CreateAccessTokenFile;
			this._serviceNotAuth.AfterGettingToken += this._testData.CreateAccessTokenFile;
		}
	}
}