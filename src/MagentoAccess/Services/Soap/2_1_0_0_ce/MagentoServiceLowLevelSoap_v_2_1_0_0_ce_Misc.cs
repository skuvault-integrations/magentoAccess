using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Magento2catalogProductRepositoryV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2integrationAdminTokenServiceV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2salesOrderRepositoryV1_v_2_1_0_0_CE;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetSessionId;
using Netco.Extensions;
using Netco.Logging;
using Newtonsoft.Json;

namespace MagentoAccess.Services.Soap._2_1_0_0_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_2_1_0_0_ce : IMagentoServiceLowLevelSoap
	{
		public string ApiUser { get; private set; }
		public string ApiKey { get; private set; }
		public string Store { get; private set; }
		public string BaseMagentoUrl { get; set; }
		public string RelativeUrl { get; set; }
		public string DefaultApiUrl => MagentoServiceSoapClientFactory.SoapApiUrl;
		public string TokenSecret { get; set; }
		public bool LogRawMessages { get; private set; }

		[ JsonIgnore ]
		[ IgnoreDataMember ]
		public Func< Task< Tuple< string, DateTime > > > PullSessionId { get; set; }

		protected salesOrderRepositoryV1PortTypeClient _magentoSoapService;

		protected integrationAdminTokenServiceV1PortTypeClient _magentoSoapService2;

		protected string _sessionId;

		protected DateTime _sessionIdCreatedAt;

		protected SemaphoreSlim getSessionIdSemaphore;

		private readonly MagentoServiceSoapClientFactory _clientFactory;

		protected const int SessionIdLifeTime = 3590;

		public bool GetStockItemsWithoutSkuImplementedWithPages => true;
		public bool GetOrderByIdForFullInformation => true;
		public bool GetOrdersUsesEntityInsteadOfIncrementId => true;
		
		public string GetServiceVersion()
		{
			return MagentoVersions.M_2_1_0_0;
		}

		public DateTime? LastActivityTime
		{
			get { return null; }
		}

		private void LogTraceGetResponseException( Exception exception )
		{
			MagentoLogger.Log().Trace( exception, "[magento] SOAP throw an exception." );
		}

		public Task< bool > InitAsync( bool suppressExceptions = false )
		{
			return Task.FromResult( true );
		}

		public async Task< GetSessionIdResponse > GetSessionId( CancellationToken cancellationToken, bool throwException = true )
		{
			try
			{
				this.getSessionIdSemaphore.Wait();
				if( !string.IsNullOrWhiteSpace( this._sessionId ) && DateTime.UtcNow.Subtract( this._sessionIdCreatedAt ).TotalSeconds < SessionIdLifeTime )
					return new GetSessionIdResponse( this._sessionId, true );

				var sessionId = await this.PullSessionId().ConfigureAwait( false );

				this._sessionIdCreatedAt = sessionId.Item2;
				this._sessionId = sessionId.Item1;
				this.TokenSecret = this._sessionId;

				return new GetSessionIdResponse( this._sessionId, false );
			}
			catch( Exception exc )
			{
				if( throwException )
					throw new MagentoSoapException( string.Format( "An error occured during GetSessionId()" ), exc );
				else
				{
					this.LogTraceGetResponseException( exc );
					return null;
				}
			}
			finally
			{
				this.getSessionIdSemaphore.Release();
			}
		}

		public MagentoServiceLowLevelSoap_v_2_1_0_0_ce( string apiUser, string apiKey, string baseMagentoUrl, bool logRawMessages, 
			string store, MagentoConfig config )
		{
			this.ApiUser = apiUser;
			this.ApiKey = apiKey;
			this.Store = store;
			this.BaseMagentoUrl = baseMagentoUrl;
			this.LogRawMessages = logRawMessages;

			this._clientFactory = new MagentoServiceSoapClientFactory( baseMagentoUrl, logRawMessages, this.ApiKey, config );
			this.PullSessionId = async () =>
			{
				if( !string.IsNullOrWhiteSpace( apiUser ) )
				{
					var privateClient = this._clientFactory.CreateMagentoServiceAdminClient();
					var integrationAdminTokenServiceV1CreateAdminAccessTokenRequest = new IntegrationAdminTokenServiceV1CreateAdminAccessTokenRequest() { username = this.ApiUser, password = this.ApiKey };
					var loginResponse = await privateClient.integrationAdminTokenServiceV1CreateAdminAccessTokenAsync( integrationAdminTokenServiceV1CreateAdminAccessTokenRequest ).ConfigureAwait( false );
					this._sessionIdCreatedAt = DateTime.UtcNow;
					this._sessionId = loginResponse.integrationAdminTokenServiceV1CreateAdminAccessTokenResponse.result;
				}
				else
				{
					this._sessionId = this.ApiKey;
				}
				if( this._clientFactory != null ) this._clientFactory.Session = this._sessionId;
				return Tuple.Create( this._sessionId, DateTime.UtcNow );
			};

			this.getSessionIdSemaphore = new SemaphoreSlim( 1, 1 );
		}

		public virtual async Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException, CancellationToken cancellationToken, Mark mark = null )
		{
			try
			{
				// Magento doesn't provide method to receive magento vesrion, since Magento2.0 thats why we use backEndMoodules API

				var sessionIdRespnse = await this.GetSessionId( cancellationToken, !suppressException ).ConfigureAwait( false );
				if( sessionIdRespnse == null )
				{
					MagentoLogger.LogTrace( "Can't get session id. Possible reasons: incorrect credentials, user was blocked." );
					return null;
				}
				//var modules = await this.GetBackEndModulesAsync().ConfigureAwait( false );
				var getOrdersResponse = await this.GetOrdersAsync( DateTime.Now, DateTime.Now.AddHours( 1 ), cancellationToken ).ConfigureAwait( false );
				var getProductsRes = await this.GetProductsAsync( 1, null, false, null, cancellationToken ).ConfigureAwait( false );

				//var saveMethodResult = await this.SaveOrderMethodExistAsync().ConfigureAwait( false );
				return /*modules?.Modules != null && modules.Modules.Count > 0 &&*/ getOrdersResponse.Orders.Count() >= 0 && getProductsRes.Products.Count() >= 0 ? new GetMagentoInfoResponse( "2.1.0.0", "CE", this.GetServiceVersion() ) : null;
			}
			catch( Exception exc )
			{
				if( suppressException )
					return null;
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public string ToJsonSoapInfo()
		{
			return $"{{BaseMagentoUrl:{( string.IsNullOrWhiteSpace( this.BaseMagentoUrl ) ? PredefinedValues.NotAvailable : this.BaseMagentoUrl )}, ApiUser:{( string.IsNullOrWhiteSpace( this.ApiUser ) ? PredefinedValues.NotAvailable : this.ApiUser )},ApiKey:{( string.IsNullOrWhiteSpace( this.ApiKey ) ? PredefinedValues.NotAvailable : this.ApiKey )},ApiToken:{( string.IsNullOrWhiteSpace( this._sessionId ) ? PredefinedValues.NotAvailable : this._sessionId )},Store:{( string.IsNullOrWhiteSpace( this.Store ) ? PredefinedValues.NotAvailable : this.Store )}}}";
		}

		private string CreateMethodCallInfo( string methodParameters = "", Mark mark = null, string errors = "", string methodResult = "", string additionalInfo = "", [ CallerMemberName ] string memberName = "", string notes = "" )
		{
			mark = mark ?? Mark.Blank();
			var connectionInfo = this.ToJsonSoapInfo();
			var str = string.Format(
				"{{MethodName:{0}, ConnectionInfo:{1}, MethodParameters:{2}, Mark:\"{3}\"{4}{5}{6}{7}}}",
				memberName,
				connectionInfo,
				methodParameters,
				mark,
				string.IsNullOrWhiteSpace( errors ) ? string.Empty : ", Errors:" + errors,
				string.IsNullOrWhiteSpace( methodResult ) ? string.Empty : ", Result:" + methodResult,
				string.IsNullOrWhiteSpace( notes ) ? string.Empty : ", Notes:" + notes,
				string.IsNullOrWhiteSpace( additionalInfo ) ? string.Empty : ", AdditionalInfo: " + additionalInfo
				);
			return str;
		}

		#region JustForTesting
		public async Task< int > CreateCart( string storeid, CancellationToken cancellationToken )
		{
			return await Task.FromResult( 0 ).ConfigureAwait( false );
		}

		public async Task< string > CreateOrder( int shoppingcartid, string store, CancellationToken cancellationToken )
		{
			return await Task.FromResult( "" ).ConfigureAwait( false );
		}
		
		public async Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store, CancellationToken cancellationToken )
		{
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public async Task< bool > ShoppingCartAddressSet( int shoppingCart, string store, CancellationToken cancellationToken )
		{
			return await Task.FromResult( false ).ConfigureAwait( false );
		}
		
		public async Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store, CancellationToken cancellationToken )
		{
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public async Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store, CancellationToken cancellationToken )
		{
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public async Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store, CancellationToken cancellationToken )
		{
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public class CreateProductModel
		{
			public int IsInStock { get; }
			public string Name { get; }
			public string ProductType { get; }
			public string Sku { get; }

			public CreateProductModel( string name, string sku, int isInStock, string productType )
			{
				this.Name = name;
				this.Sku = sku;
				this.IsInStock = isInStock;
				this.ProductType = productType;
			}
		}

		public async Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType, CancellationToken cancellationToken, Mark markForLog )
		{
			var stockItem = new CreateProductModel( name, sku, isInStock, productType );
			var methodParameters = stockItem.ToJson();
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this._clientFactory.CreateMagentoCatalogProductRepositoryServiceClient();

				var res = new List< UpdateRessult< CreateProductModel > >();
				var stockItems = new List< CreateProductModel > { stockItem };

				await stockItems.DoInBatchAsync( 10, async x =>
				{
					await ActionPolicies.GetAsync.Do( async () =>
					{
						var statusChecker = new StatusChecker( maxCheckCount );
						TimerCallback tcb = statusChecker.CheckStatus;

						privateClient = this._clientFactory.RefreshMagentoCatalogProductRepositoryServiceClient( privateClient );

						var updateResult = new UpdateRessult< CreateProductModel >( x, 0 );
						res.Add( updateResult );

						using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						{
							MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark : markForLog ) );

							var catalogInventoryDataStockItemInterface = new CatalogDataProductInterface()
							{
								sku = x.Sku,
								name = x.Name,
								price = "1",
								priceSpecified = true,
								status = 1,
								statusSpecified = true,
								typeId = productType,
								attributeSetId = 4,
								attributeSetIdSpecified = true,
								weight = "1",
								weightSpecified = true,
							};
							if( productType == "bundle" )
							{
								catalogInventoryDataStockItemInterface.customAttributes = new[]
								{
									new FrameworkAttributeInterface { value = "1", attributeCode = "price_view" },
									new FrameworkAttributeInterface { value = "1", attributeCode = "price_type" }
								};
							}
							var catalogInventoryStockRegistryV1UpdateStockItemBySkuRequest = new CatalogProductRepositoryV1SaveRequest()
							{
								product = catalogInventoryDataStockItemInterface
							};

							var temp = await privateClient.catalogProductRepositoryV1SaveAsync( catalogInventoryStockRegistryV1UpdateStockItemBySkuRequest ).ConfigureAwait( false );

							updateResult.ErrorCode = temp.catalogProductRepositoryV1SaveResponse.result.id;
						}
					} ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( methodParameters, mark : markForLog, methodResult : res.ToJson() ) );

				return res.First().ErrorCode;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during PutStockItemsAsync({methodParameters})", exc );
			}
		}

		public async Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType, CancellationToken cancellationToken )
		{
			return await Task.FromResult( false ).ConfigureAwait( false );
		}
		#endregion

		private class UpdateRessult< T1 >
		{
			public UpdateRessult( T1 itemToUpdate, int errorCode )
			{
				this.ItemToUpdate = itemToUpdate;
				this.ErrorCode = errorCode;
			}

			public int ErrorCode { get; set; }
			public T1 ItemToUpdate { get; set; }
		}
	}
}