using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Services;
using MagentoAccess.Services.Rest;
using MagentoAccess.Services.Soap._1_14_1_0_ee;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;
using MagentoAccess.Services.Soap._1_9_2_1_ce;
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
		protected MagentoService _magentoServiceNotAuth;
		protected MagentoSoapCredentials _soapUserCredentials;
		protected MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE _magentoLowLevelSoapVFrom17To19CeService;
		protected MagentoServiceLowLevelSoap_v_1_14_1_0_EE _magentoServiceLowLevelSoapV11410Ee;
		protected List< MagentoAccess.Models.Services.Soap.GetOrders.Order > _orders;
		protected Dictionary< int, string > _productsIds;
		protected MagentoServiceLowLevelRestRest _magentoServiceLowLevelRestRestRestRest;
		protected MagentoServiceLowLevelRestRest _magentoServiceLowLevelRestRestRestRestNotAuth;
		protected MagentoServiceLowLevelSoap_v_1_9_2_1_ce _magentoServiceLowLevelSoapV_1_9_2_1_ce;
		protected MagentoServiceLowLevelSoap_v_1_9_2_1_ce _magentoLowLevelSoapForCreatingTestEnvironment;

		protected IMagentoService CreateMagentoService( string apiUser, string apiKey, string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret, string magentoBaseUrl, string requestTokenUrl, string authorizeUrl, string accessTokenUrl )
		{
			return ( string.IsNullOrWhiteSpace( accessToken ) || string.IsNullOrWhiteSpace( accessTokenSecret ) ) ?
				new MagentoService( new MagentoNonAuthenticatedUserCredentials(
					consumerKey,
					consumerSecret,
					magentoBaseUrl,
					requestTokenUrl,
					authorizeUrl,
					accessTokenUrl
					)
					) :
				new MagentoService( new MagentoAuthenticatedUserCredentials(
					accessToken,
					accessTokenSecret,
					magentoBaseUrl,
					consumerSecret,
					consumerKey,
					apiUser, apiKey
					) );
		}

		[ SetUp ]
		public void Setup()
		{
			this._magentoServiceNotAuth = new MagentoService( new MagentoNonAuthenticatedUserCredentials(
				this._consumer.Key,
				this._consumer.Secret,
				this._authorityUrls.MagentoBaseUrl
				) );

			NetcoLogger.LoggerFactory = new NLogLoggerFactory();

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

			this._magentoLowLevelSoapVFrom17To19CeService = new MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE( this._soapUserCredentials.ApiUser, this._soapUserCredentials.ApiKey, this._authorityUrls.MagentoBaseUrl, null );

			this._magentoServiceLowLevelSoapV11410Ee = new MagentoServiceLowLevelSoap_v_1_14_1_0_EE( this._soapUserCredentials.ApiUser, this._soapUserCredentials.ApiKey, this._authorityUrls.MagentoBaseUrl, null );

			this._magentoServiceLowLevelSoapV_1_9_2_1_ce = new MagentoServiceLowLevelSoap_v_1_9_2_1_ce(this._soapUserCredentials.ApiUser, this._soapUserCredentials.ApiKey, this._authorityUrls.MagentoBaseUrl, null);

			this._magentoServiceLowLevelRestRestRestRestNotAuth = new MagentoServiceLowLevelRestRest( this._consumer.Key, this._consumer.Secret, this._authorityUrls.MagentoBaseUrl, this._authorityUrls.RequestTokenUrl, this._authorityUrls.AuthorizeUrl, this._authorityUrls.AccessTokenUrl );

			this._magentoServiceLowLevelRestRestRestRest = new MagentoServiceLowLevelRestRest( this._consumer.Key, this._consumer.Secret, this._authorityUrls.MagentoBaseUrl, this._accessToken.AccessToken, this._accessToken.AccessTokenSecret );

			this._magentoLowLevelSoapForCreatingTestEnvironment = _magentoServiceLowLevelSoapV_1_9_2_1_ce;

			//this.CreateProductstems();
			//this.CreateOrders();
		}
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			//this.DeleteProducts();
		}

		protected void CreateOrders()
		{
			var ordersIds = new List< string >();

			var testStoresCredentials = GetTestStoresCredentials();
			foreach( var credentials in testStoresCredentials )
			{
				var ordersModels = new List< CreateOrderModel >();
				for( var i = 0; i < 5; i++ )
				{
					ordersModels.Add( new CreateOrderModel() { StoreId = "0", CustomerFirstName = "max", CustomerMail = "qwe@qwe.com", CustomerLastName = "kits", ProductIds = this._productsIds.Values } );
				}

				var magentoService = CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com" );
				var creationResult = magentoService.CreateOrderAsync( ordersModels );
				creationResult.Wait();

				var ordersTask = this._magentoLowLevelSoapForCreatingTestEnvironment.GetOrdersAsync( ordersIds );
				ordersTask.Wait();
				this._orders = ordersTask.Result.Orders.ToList().OrderBy( x => x.UpdatedAt ).ToList();
			}
		}

		protected void CreateProducts()
		{
			this._productsIds = new Dictionary< int, string >();

			var createProuctsTasks = new List< Task >();

			var testStoresCredentials = GetTestStoresCredentials();
			foreach( var credentials in testStoresCredentials )
			{
				var source = new List< CreateProductModel >();
				for( var i = 0; i < 5; i++ )
				{
					var tiks = DateTime.UtcNow.Ticks.ToString();
					var sku = string.Format( "TddTestSku{0}_{1}", i, tiks );
					var name = string.Format( "TddTestName{0}_{1}", i, tiks );
					source.Add( new CreateProductModel( "0", sku, name, 1 ) );
				}
				var magentoService = CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com" );
				var creationResult = magentoService.CreateProductAsync( source );
				creationResult.Wait();
				this._productsIds.AddRange( creationResult.Result.ToDictionary( x => x.Result, y => y.Sku ) );
			}

			var commonTask = Task.WhenAll( createProuctsTasks );
			commonTask.Wait();
		}

		protected void DeleteProducts()
		{
			try
			{
				var testStoresCredentials = GetTestStoresCredentials();

				foreach( var credentials in testStoresCredentials )
				{
					var productsToRemove = GetOnlyProductsCreatedForThisTests();
					var productsToRemoveDeleteProductModels = productsToRemove.Select( p => new DeleteProductModel( "0", 0, p.ProductId, "" ) ).ToList();

					var magentoService = CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com" );
					var deleteres = magentoService.DeleteProductAsync( productsToRemoveDeleteProductModels );

					deleteres.Wait();
				}
			}
			catch( Exception exception )
			{
				Console.WriteLine( "Can't delete products {0}", exception );
			}
		}

		//protected Dictionary< string, IEnumerable< Product > > GetOnlyProductsCreatedForThisTests()
		protected  IEnumerable< Product > GetOnlyProductsCreatedForThisTests()
		{
			var testStoresCredentials = GetTestStoresCredentials();
			var res = new Dictionary< string, IEnumerable< Product > >();
			foreach( var credentials in testStoresCredentials )
			{
				var magentoService = CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com" );
				var getProductsTask = magentoService.GetProductsAsync();
				getProductsTask.Wait();

				var allProductsinMagent = getProductsTask.Result.ToList();
				var onlyProductsCreatedForThisTests = allProductsinMagent.Where( x => this._productsIds.ContainsKey( int.Parse( x.ProductId ) ) );
				res[ credentials.StoreUrl ] = onlyProductsCreatedForThisTests;
			}
			//return res;
			return Enumerable.Empty<Product>();
		}
		internal class MagentoServiceSoapCredentials
		{
			public string SoapApiUser { get; set; }
			public string SoapApiKey { get; set; }
			public string StoreUrl { get; set; }
		}

		internal IEnumerable< MagentoServiceSoapCredentials > GetTestStoresCredentials()
		{
			if( _testData == null )
				_testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv", @"..\..\Files\magento_VerifierCode.csv" );
			return _testData._accessTokensFromFile.Zip( _testData._storesUrlsFromFile, ( x, y ) => new MagentoServiceSoapCredentials { SoapApiKey = x.SoapApiKey, SoapApiUser = x.SoapUserName, StoreUrl = y.MagentoBaseUrl } );
		}
	}
}