using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetOpenAuth.Messaging;
using MagentoAccess;
using MagentoAccess.Models.CreateOrders;
using MagentoAccess.Models.CreateProducts;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.DeleteProducts;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Soap.GetOrders;
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
		protected ConcurrentDictionary< string,List< MagentoAccess.Models.GetOrders.Order >> _orders;
		protected ConcurrentDictionary< string, Dictionary< int, string > > _productsIds;
		protected MagentoServiceLowLevelRestRest _magentoServiceLowLevelRestRestRestRest;
		protected MagentoServiceLowLevelRestRest _magentoServiceLowLevelRestRestRestRestNotAuth;
		protected MagentoServiceLowLevelSoap_v_1_9_2_1_ce _magentoLowLevelSoapForCreatingTestEnvironment;

		protected IMagentoService CreateMagentoService( string apiUser, string apiKey, string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret, string magentoBaseUrl, string requestTokenUrl, string authorizeUrl, string accessTokenUrl, string magentoVersionByDefault )
		{
			return (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(accessTokenSecret)) ?
				new MagentoService(new MagentoNonAuthenticatedUserCredentials(
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
					), string.IsNullOrWhiteSpace( magentoVersionByDefault ) ? null : new MagentoConfig() { VersionByDefault = magentoVersionByDefault } );
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

			this._magentoServiceLowLevelRestRestRestRestNotAuth = new MagentoServiceLowLevelRestRest( this._consumer.Key, this._consumer.Secret, this._authorityUrls.MagentoBaseUrl, this._authorityUrls.RequestTokenUrl, this._authorityUrls.AuthorizeUrl, this._authorityUrls.AccessTokenUrl );

			this._magentoServiceLowLevelRestRestRestRest = new MagentoServiceLowLevelRestRest( this._consumer.Key, this._consumer.Secret, this._authorityUrls.MagentoBaseUrl, this._accessToken.AccessToken, this._accessToken.AccessTokenSecret );

			//this.CreateProductstems();
			//this.CreateOrders();
		}

		[ TestFixtureTearDown ]
		public void TestFixtureTearDown()
		{
			//this.DeleteProducts();
		}

		protected void CreateOrders()
		{
			var ordersIds = new List< string >();
			var testStoresCredentials = GetTestStoresCredentials();
			Parallel.ForEach( testStoresCredentials, credentials =>
			{
				try
				{
					var ordersModels = new List< CreateOrderModel >();
					for( var i = 0; i < 5; i++ )
					{
						ordersModels.Add( new CreateOrderModel() { StoreId = "0", CustomerFirstName = "max", CustomerMail = "qwe@qwe.com", CustomerLastName = "kits", ProductIds = this._productsIds[ credentials.StoreUrl ].Keys.Select( x => x.ToString() ) } );
					}

					var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", null );
					var creationResult = magentoService.CreateOrderAsync( ordersModels );
					creationResult.Wait();
					ordersIds = creationResult.Result.Select(x => x.OrderId).ToList();
					var ordersTask = magentoService.GetOrdersAsync(ordersIds);
					ordersTask.Wait();

					if( this._orders == null )
						this._orders = new ConcurrentDictionary<string, List<MagentoAccess.Models.GetOrders.Order>>();

					this._orders[credentials.StoreUrl] = new List<MagentoAccess.Models.GetOrders.Order>();
					var ordersToAdd = ordersTask.Result.ToList().OrderBy( x => x.UpdatedAt );
					this._orders[ credentials.StoreUrl ].AddRange( ordersToAdd );
				}
				catch( Exception exception )
				{
				}
			} );
		}

		protected void CreateProducts()
		{
			try
			{
				this._productsIds = new ConcurrentDictionary< string, Dictionary< int, string > >(); // new Dictionary< int, string >();

				var testStoresCredentials = GetTestStoresCredentials();

				Parallel.ForEach( testStoresCredentials, credentials =>
				{
					try
					{
						var source = new List< CreateProductModel >();
						for( var i = 0; i < 5; i++ )
						{
							var tiks = DateTime.UtcNow.Ticks.ToString();
							var sku = string.Format( "TddTestSku{0}_{1}", i, tiks );
							var name = string.Format( "TddTestName{0}_{1}", i, tiks );
							source.Add( new CreateProductModel( "0", sku, name, 1 ) );
						}
						var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", null );
						var creationResult = magentoService.CreateProductAsync( source );

						creationResult.Wait();
						this._productsIds[ credentials.StoreUrl ] = new Dictionary< int, string >();
						this._productsIds[ credentials.StoreUrl ].AddRange( creationResult.Result.ToDictionary( x => x.Result, y => y.Sku ) );
					}
					catch( Exception exception )
					{
					}
				} );
			}
			catch
			{
			}
		}

		protected void DeleteProducts()
		{
			try
			{
				var testStoresCredentials = GetTestStoresCredentials();

				foreach( var credentials in testStoresCredentials )
				{
					var productsToRemove = GetOnlyProductsCreatedForThisTests( credentials );
					var productsToRemoveDeleteProductModels = productsToRemove.Select( p => new DeleteProductModel( "0", 0, p.ProductId, "" ) ).ToList();

					var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", null );
					var deleteres = magentoService.DeleteProductAsync( productsToRemoveDeleteProductModels );

					deleteres.Wait();
				}
			}
			catch( Exception exception )
			{
				Console.WriteLine( "Can't delete products {0}", exception );
			}
		}


		protected IEnumerable< Product > GetOnlyProductsCreatedForThisTests( MagentoServiceSoapCredentials magentoServiceSoapCredentials )
		{
			var magentoService = this.CreateMagentoService( magentoServiceSoapCredentials.SoapApiUser, magentoServiceSoapCredentials.SoapApiKey, "null", "null", "null", "null", magentoServiceSoapCredentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", null );
			var getProductsTask = magentoService.GetProductsAsync();
			getProductsTask.Wait();

			var allProductsinMagent = getProductsTask.Result.ToList();
			var onlyProductsCreatedForThisTests = allProductsinMagent.Where( x => this._productsIds[ magentoServiceSoapCredentials.StoreUrl ].ContainsKey( int.Parse( x.ProductId ) ) );
			return onlyProductsCreatedForThisTests;
		}

		internal class MagentoServiceSoapCredentials
		{
			public string SoapApiUser { get; set; }
			public string SoapApiKey { get; set; }
			public string StoreUrl { get; set; }
			public string MagentoVersion{ get; set; }

			public override string ToString()
			{
				return StoreUrl.ToString();
			}
		}

		internal IEnumerable< MagentoServiceSoapCredentials > GetTestStoresCredentials()
		{
			// Resharper test runner can't process cases loaded runtime
			//if( _testData == null )
			//	_testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv", @"..\..\Files\magento_VerifierCode.csv" );
			//var magentoServiceSoapCredentialses = _testData._accessTokensFromFile.Zip( _testData._storesUrlsFromFile, ( x, y ) => new MagentoServiceSoapCredentials { SoapApiKey = x.SoapApiKey, SoapApiUser = x.SoapUserName, StoreUrl = y.MagentoBaseUrl } );
			yield return new MagentoServiceSoapCredentials() { StoreUrl = "http://199.48.164.39:4423/Magento-2-0-2-0-ce", SoapApiUser = "user", SoapApiKey = "vdvm4gi9egyp5arq1914oeqilrlwk1sb", MagentoVersion="2.0.2.0" };
		}
	}
}