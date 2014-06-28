using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Models.Services.Credentials;
using MagentoAccess.Services;
using Netco.Logging;
using Netco.Logging.NLogIntegration;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	internal class BaseTest
	{
		protected TestData _testData;
		private MagentoConsumerCredentials _consumer;
		private MagentoUrls _authorityUrls;
		private MagentoAccessToken _accessToken;

		protected TransmitVerificationCodeDelegate transmitVerificationCode;
		protected MagentoService _magentoService;
		protected MagentoService _magentoServiceNotAuth;
		protected MagentoSoapCredentials _soapUserCredentials;
		protected MagentoServiceLowLevelSoap _magentoLowLevelSoapService;
		protected List< salesOrderListEntity > _orders;
		protected Dictionary< int, string > _productsIds;
		protected MagentoServiceLowLevel _magentoServiceLowLevelRest;
		protected MagentoServiceLowLevel _magentoServiceLowLevelRestNotAuth;

		[ SetUp ]
		public void Setup()
		{
			this._magentoService = ( this._accessToken == null || string.IsNullOrWhiteSpace( this._accessToken.AccessToken ) || string.IsNullOrWhiteSpace( this._accessToken.AccessTokenSecret ) ) ?
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

			this._magentoServiceNotAuth = new MagentoService( new MagentoNonAuthenticatedUserCredentials(
				this._consumer.Key,
				this._consumer.Secret,
				this._authorityUrls.MagentoBaseUrl
				) );

			NetcoLogger.LoggerFactory = new NLogLoggerFactory();

			this._magentoService.AfterGettingToken += this._testData.CreateAccessTokenFile;
			this._magentoServiceNotAuth.AfterGettingToken += this._testData.CreateAccessTokenFile;
		}

		[ TestFixtureSetUp ]
		public void TestFixtureSetup()
		{
			this._testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv", @"..\..\Files\magento_VerifierCode.csv" );
			this._consumer = this._testData.GetMagentoConsumerCredentials();
			this._authorityUrls = this._testData.GetMagentoUrls();
			this._accessToken = this._testData.GetMagentoAccessToken();
			this._soapUserCredentials = this._testData.GetMagentoSoapUser();
			this.transmitVerificationCode = () => this._testData.TransmitVerification();

			this._magentoLowLevelSoapService = new MagentoServiceLowLevelSoap( this._soapUserCredentials.ApiUser, this._soapUserCredentials.ApiKey, this._authorityUrls.MagentoBaseUrl, null );

			this._magentoServiceLowLevelRestNotAuth = new MagentoServiceLowLevel( this._consumer.Key, this._consumer.Secret, this._authorityUrls.MagentoBaseUrl, this._authorityUrls.RequestTokenUrl, this._authorityUrls.AuthorizeUrl, this._authorityUrls.AccessTokenUrl );

			this._magentoServiceLowLevelRest = new MagentoServiceLowLevel( this._consumer.Key, this._consumer.Secret, this._authorityUrls.MagentoBaseUrl, this._accessToken.AccessToken, this._accessToken.AccessTokenSecret );

			this.CreateOrders();
			this.CreateProductstems();
		}

		protected void CreateOrders()
		{
			var ordersIds = new List< string >();

			for( var i = 0; i < 5; i++ )
			{
				var shoppingCartIdTask = this._magentoLowLevelSoapService.CreateCart( "0" );
				shoppingCartIdTask.Wait();
				var _shoppingCartId = shoppingCartIdTask.Result;

				var shoppingCartCustomerSetTask = this._magentoLowLevelSoapService.ShoppingCartGuestCustomerSet( _shoppingCartId, "max", "qwe@qwe.com", "kits", "0" );
				shoppingCartCustomerSetTask.Wait();

				var shoppingCartAddressSet = this._magentoLowLevelSoapService.ShoppingCartAddressSet( _shoppingCartId, "0" );
				shoppingCartAddressSet.Wait();

				var productTask = this._magentoLowLevelSoapService.ShoppingCartAddProduct( _shoppingCartId, "1", "0" );
				productTask.Wait();

				var shippingMenthodTask = this._magentoLowLevelSoapService.ShoppingCartSetShippingMethod( _shoppingCartId, "0" );
				shippingMenthodTask.Wait();

				var paymentMenthodTask = this._magentoLowLevelSoapService.ShoppingCartSetPaymentMethod( _shoppingCartId, "0" );
				paymentMenthodTask.Wait();

				var orderIdTask = this._magentoLowLevelSoapService.CreateOrder( _shoppingCartId, "0" );
				orderIdTask.Wait();
				var orderId = orderIdTask.Result;
				ordersIds.Add( orderId );
				Task.Delay( 1000 );
			}

			var ordersTask = this._magentoLowLevelSoapService.GetOrdersAsync( ordersIds );
			ordersTask.Wait();
			this._orders = ordersTask.Result.result.OrderBy( x => x.updated_at ).ToList();
		}

		protected void CreateProductstems()
		{
			this._productsIds = new Dictionary< int, string >();

			var createProuctsTasks = new List< Task >();

			for( var i = 0; i < 5; i++ )
			{
				var tiks = DateTime.UtcNow.Ticks.ToString();
				var sku = string.Format( "TddTestSku{0}_{1}", i, tiks );
				var name = string.Format( "TddTestName{0}_{1}", i, tiks );
				var shoppingCartIdTask = this._magentoLowLevelSoapService.CreateProduct( "0", name, sku );
				createProuctsTasks.Add( shoppingCartIdTask );
				//shoppingCartIdTask.Wait();
				this._productsIds.Add( shoppingCartIdTask.Result, sku );
			}

			var commonTask = Task.WhenAll( createProuctsTasks );
			commonTask.Wait();
		}
	}
}