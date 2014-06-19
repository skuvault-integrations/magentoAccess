using MagentoAccess;
using MagentoAccess.Models.Services.Credentials;
using MagentoAccess.Services;
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

		protected TransmitVerificationCodeDelegate transmitVerificationCode;
		protected MagentoService _service;
		protected MagentoService _serviceNotAuth;
		private MagentoSoapCredentials _soapUserCredentials;

		[ SetUp ]
		public void Setup()
		{
			this._testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv", @"..\..\Files\magento_VerifierCode.csv" );
			this._consumer = this._testData.GetMagentoConsumerCredentials();
			this._authorityUrls = this._testData.GetMagentoUrls();
			this._accessToken = this._testData.GetMagentoAccessToken();
			this._soapUserCredentials = this._testData.GetMagentoSoapUser();
			this.transmitVerificationCode = () => this._testData.TransmitVerification();

			this._service = ( this._accessToken == null || string.IsNullOrWhiteSpace( this._accessToken.AccessToken ) || string.IsNullOrWhiteSpace( this._accessToken.AccessTokenSecret ) ) ?
				new MagentoService( new MagentoNonAuthenticatedUserCredentials(
					this._consumer.Key,
					this._consumer.Secret,
					this._authorityUrls.MagentoBaseUrl,
					this._authorityUrls.RequestTokenUrl,
					this._authorityUrls.AuthorizeUrl,
					this._authorityUrls.AccessTokenUrl
					)
					) :
				new MagentoService( new MagentoAuthenticatedUserCredentials(
					this._accessToken.AccessToken,
					this._accessToken.AccessTokenSecret,
					this._authorityUrls.MagentoBaseUrl,
					this._consumer.Secret,
					this._consumer.Key,
					this._soapUserCredentials.ApiUser,
					this._soapUserCredentials.ApiKey
					) );

			this._serviceNotAuth = new MagentoService( new MagentoNonAuthenticatedUserCredentials(
				this._consumer.Key,
				this._consumer.Secret,
				this._authorityUrls.MagentoBaseUrl
				) );

			NetcoLogger.LoggerFactory = new NLogLoggerFactory();

			this._service.AfterGettingToken += this._testData.CreateAccessTokenFile;
			this._serviceNotAuth.AfterGettingToken += this._testData.CreateAccessTokenFile;
		}
	}
}