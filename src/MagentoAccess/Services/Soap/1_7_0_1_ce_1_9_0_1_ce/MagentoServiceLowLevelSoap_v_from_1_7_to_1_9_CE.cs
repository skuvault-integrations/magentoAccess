using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetSessionId;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using Netco.Logging;

namespace MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE : MagentoServiceLowLevelSoapBase, IMagentoServiceLowLevelSoap
	{
		private readonly MagentoServiceSoapClientFactory _clientFactory;
		private readonly Mage_Api_Model_Server_Wsi_HandlerPortTypeClient _magentoSoapService;
		public string GetServiceVersion()
		{
			return MagentoVersions.M_1_9_0_1;
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
				this._getSessionIdSemaphore.Wait();
				if( !string.IsNullOrWhiteSpace( this._sessionId ) && DateTime.UtcNow.Subtract( this._sessionIdCreatedAt ).TotalSeconds < this._sessionIdLifeTime )
					return new GetSessionIdResponse( this._sessionId, true );

				var sessionId = await this.PullSessionId().ConfigureAwait( false );

				this._sessionIdCreatedAt = sessionId.Item2;
				this._sessionId = sessionId.Item1;

				return new GetSessionIdResponse( this._sessionId, false );
				;
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
				this._getSessionIdSemaphore.Release();
			}
		}

		public MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE( string apiUser, string apiKey, string baseMagentoUrl, string relativeUrl, string store, 
			int getProductsMaxThreads, bool logRawMessages, int sessionIdLifeTime, MagentoConfig config )
			: base( apiUser, apiKey, baseMagentoUrl, relativeUrl, store, getProductsMaxThreads, logRawMessages, sessionIdLifeTime ) 
		{
			this._clientFactory = new MagentoServiceSoapClientFactory( baseMagentoUrl, relativeUrl, logRawMessages, config );
			this._magentoSoapService = this._clientFactory.GetClient();
			Magento1xxxHelper = new Magento1xxxHelper(this);
			PullSessionId = async () =>
			{
				var privateClient = this._clientFactory.GetClient();
				var loginResponse = await privateClient.loginAsync(this.ApiUser, this.ApiKey).ConfigureAwait(false);
				return Tuple.Create(loginResponse.result, DateTime.UtcNow);
			};	
		}

		private sealed class MagentoServiceSoapClientFactory : BaseMagentoServiceSoapClientFactory< Mage_Api_Model_Server_Wsi_HandlerPortTypeClient, Mage_Api_Model_Server_Wsi_HandlerPortType >
		{
			public MagentoServiceSoapClientFactory( string baseMagentoUrl, string relativeUrl, bool logRawMessages, MagentoConfig config ) 
				: base( baseMagentoUrl, relativeUrl, logRawMessages, config )
			{
			}

			protected override Mage_Api_Model_Server_Wsi_HandlerPortTypeClient CreateClient()
			{
				var soapApiUrl = string.IsNullOrWhiteSpace( this._relativeUrl ) ? DefaultSoapApiUrl : this._relativeUrl;
				var endPoint = new List< string > { this._baseMagentoUrl, soapApiUrl }.BuildUrl();
				var customBinding = CustomBinding( this._baseMagentoUrl, MessageVersion.Soap11, this._config.BindingDecompressionEnabled );
				var magentoSoapService = new Mage_Api_Model_Server_Wsi_HandlerPortTypeClient( customBinding, new EndpointAddress( endPoint ) );

				magentoSoapService.Endpoint.Behaviors.Add( new CustomBehavior() { LogRawMessages = this._logRawMessages } );

				return magentoSoapService;
			}
		}

		private static void AddFilter( filters filters, string value, string key, string valueKey )
		{
			var temp = filters.complex_filter.ToList();
			temp.Add( new complexFilter() { key = key, value = new associativeEntity() { key = valueKey, value = value } } );
			filters.complex_filter = temp.ToArray();
		}

		public virtual async Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( CancellationToken cancellationToken, string rootCategory = "1" )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this._clientFactory.GetClient();

				var res = new catalogCategoryTreeResponse();
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogCategoryTreeAsync( sessionId.SessionId, rootCategory, "0" ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new GetCategoryTreeResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during GetCategoriesTree({rootCategory})", exc );
			}
		}

		public virtual async Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, CancellationToken cancellationToken, bool throwException = true )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this._clientFactory.GetClient();

				var res = new catalogProductAttributeMediaListResponse();
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductAttributeMediaListAsync( sessionId.SessionId, getProductAttributeMediaListRequest.ProductId, "0", "1" ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new ProductAttributeMediaListResponse( res, getProductAttributeMediaListRequest.ProductId, getProductAttributeMediaListRequest.Sku );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during GetProductAttributeMediaListAsync({getProductAttributeMediaListRequest})", exc );
			}
		}

		public virtual async Task< CatalogProductAttributeInfoResponse > GetManufacturersInfoAsync( string attribute, CancellationToken cancellationToken )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductAttributeInfoResponse();
				var privateClient = this._clientFactory.GetClient();

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );
					try // this try catch block is crunch for 1.7
					{
						using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
							res = await privateClient.catalogProductAttributeInfoAsync( sessionId.SessionId, attribute ).ConfigureAwait( false );
					}
					catch( Exception exc )
					{
						if( exc.Message.ToLower().Contains( "Procedure 'catalogProductAttributeInfoRequestParam' not present".ToLower() )
						    && this.StoreVersion.StartsWith( "1.7" ) )
							res = null;
						else
							throw;
					}
				} ).ConfigureAwait( false );

				return new CatalogProductAttributeInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetManufacturerAsync()" ), exc );
			}
		}

		public async Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts, CancellationToken cancellationToken )
		{
			return await this.Magento1xxxHelper.FillProductDetails( resultProducts ).ConfigureAwait( false );
		}

		public Task< InventoryStockItemListResponse > GetStockItemsWithoutSkuAsync( IEnumerable< string > skusOrIds, IEnumerable< int > scopes, CancellationToken cancellationToken, Mark mark = null )
		{
			throw new NotImplementedException();
		}

		public virtual async Task< CatalogProductInfoResponse > GetProductInfoAsync( CatalogProductInfoRequest request, CancellationToken cancellationToken, bool throwException = true )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductInfoResponse();
				var privateClient = this._clientFactory.GetClient();


				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;
					
					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );
					var attributes = new catalogProductRequestAttributes { additional_attributes = request.custAttributes ?? new string[ 0 ] };

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductInfoAsync( sessionId.SessionId, request.ProductId, "0", attributes, "1" ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new CatalogProductInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during GetProductInfoAsync({request.ToJson()})", exc );
			}
		}

		public virtual async Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds, IEnumerable< int > scopes, CancellationToken cancellationToken, Mark mark = null )
		{
			try
			{
				var skusArray = skusOrIds.ToArray();

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogInventoryStockItemListResponse();
				var privateClient = this._clientFactory.GetClient();

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogInventoryStockItemListAsync( sessionId.SessionId, skusArray ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new InventoryStockItemListResponse( res );
			}
			catch( Exception exc )
			{
				var productsBriefInfo = string.Join( "|", skusOrIds );
				throw new MagentoSoapException( $"An error occured during GetStockItemsAsync({productsBriefInfo})", exc );
			}
		}

		public virtual async Task< IEnumerable< RpcInvoker.RpcRequestResponse< PutStockItem, object > > > PutStockItemsAsync( List< PutStockItem > stockItems, CancellationToken cancellationToken, Mark mark = null )
		{
			var methodParameters = stockItems.ToJson();
			try
			{
				var stockItemsProcessed = stockItems.Select( x =>
				{
					var catalogInventoryStockItemUpdateEntity = ( x.Qty > 0 ) ?
						new catalogInventoryStockItemUpdateEntity() { is_in_stock = 1, is_in_stockSpecified = true, qty = x.Qty.ToString() } :
						new catalogInventoryStockItemUpdateEntity() { is_in_stock = 0, is_in_stockSpecified = false, qty = x.Qty.ToString() };
					return Tuple.Create( x, catalogInventoryStockItemUpdateEntity );
				} );

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this._clientFactory.GetClient();

				RpcInvoker.RpcResponse< catalogInventoryStockItemMultiUpdateResponse > serverResponse = null;
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

						var catalogInventoryStockItemUpdateEntities = stockItemsProcessed.Select( x => x.Item2 ).ToArray();

						serverResponse = await RpcInvoker.SuppressExceptions( async () => await privateClient.catalogInventoryStockItemMultiUpdateAsync( sessionId.SessionId, stockItemsProcessed.Select( x => x.Item1.ItemId ).ToArray(), catalogInventoryStockItemUpdateEntities ).ConfigureAwait( false ) ).ConfigureAwait( false );

						var updateBriefInfo = string.Format( "{{Success:{0}}}", serverResponse.Result.result );
						MagentoLogger.LogTraceEnded( CreateMethodCallInfo( methodParameters, mark : mark, methodResult : updateBriefInfo ) );
					}
				} ).ConfigureAwait( false );

				var result = stockItems.Select( y => new RpcInvoker.RpcRequestResponse< PutStockItem, object >( y, new RpcInvoker.RpcResponse< object >( serverResponse?.ErrorCode ?? RpcInvoker.SoapErrorCode.Unknown, serverResponse?.Result, serverResponse?.Exception ) ) );
				return result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during PutStockItemsAsync({methodParameters})", exc );
			}
		}

		public virtual async Task< bool > PutStockItemAsync( PutStockItem putStockItem, CancellationToken cancellationToken, Mark markForLog )
		{
			try
			{
				var methodParameters = new List< PutStockItem > { putStockItem }.ToJson();

				var catalogInventoryStockItemUpdateEntity = ( putStockItem.Qty > 0 ) ?
					new catalogInventoryStockItemUpdateEntity() { is_in_stock = 1, is_in_stockSpecified = true, qty = putStockItem.Qty.ToString() } :
					new catalogInventoryStockItemUpdateEntity() { is_in_stock = 0, is_in_stockSpecified = false, qty = putStockItem.Qty.ToString() };

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 120000;

				var res = false;
				var privateClient = this._clientFactory.GetClient();


				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						MagentoLogger.LogTraceStarted( CreateMethodCallInfo( methodParameters, markForLog ) );

						var temp = await privateClient.catalogInventoryStockItemUpdateAsync( sessionId.SessionId, putStockItem.ProductId, catalogInventoryStockItemUpdateEntity ).ConfigureAwait( false );

						res = temp.result > 0;

						var updateBriefInfo = string.Format( "{{Success:{0}}}", res );
						MagentoLogger.LogTraceEnded( CreateMethodCallInfo( methodParameters, markForLog, methodResult : updateBriefInfo ) );
					}
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				var productsBriefInfo = new List< PutStockItem > { putStockItem }.ToJson();
				throw new MagentoSoapException( $"An error occured during PutStockItemsAsync({productsBriefInfo})", exc );
			}
		}
		
		public virtual async Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException, CancellationToken cancellationToken, Mark mark = null )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new magentoInfoResponse();
				var privateClient = this._clientFactory.GetClient();

				await ActionPolicies.GetWithMarkAsync( mark ).Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.magentoInfoAsync( sessionId.SessionId ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new GetMagentoInfoResponse( res, this.GetServiceVersion() );
			}
			catch( Exception exc )
			{
				if( suppressException )
					return null;
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		#region JustForTesting
		public async Task< int > CreateCart( string storeid, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var res = await this._magentoSoapService.shoppingCartCreateAsync( sessionId.SessionId, storeid ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during GetMagentoInfoAsync({storeid})", exc );
			}
		}

		public async Task< string > CreateOrder( int shoppingcartid, string store, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var res = await this._magentoSoapService.shoppingCartOrderAsync( sessionId.SessionId, shoppingcartid, store, null ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< int > CreateCustomer(
			CancellationToken cancellationToken,
			string email = "na@na.com",
			string firstname = "firstname",
			string lastname = "lastname",
			string password = "password",
			int websiteId = 0,
			int storeId = 0,
			int groupId = 0
			)
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var customerCustomerEntityToCreate = new customerCustomerEntityToCreate
				{
					email = email,
					firstname = firstname,
					lastname = lastname,
					password = password,
					website_id = websiteId,
					store_id = storeId,
					group_id = groupId
				};
				var res = await this._magentoSoapService.customerCustomerCreateAsync( sessionId.SessionId, customerCustomerEntityToCreate ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartCustomerSet( int shoppingCart, int customerId, string customerPass, string store, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var cutomers = await this._magentoSoapService.customerCustomerListAsync( sessionId.SessionId, new filters() ).ConfigureAwait( false );

				var customer = cutomers.result.First( x => x.customer_id == customerId );

				var customerShoppingCart = new shoppingCartCustomerEntity
				{
					confirmation = ( customer.confirmation ? 1 : 0 ).ToString( CultureInfo.InvariantCulture ),
					customer_id = customer.customer_id,
					customer_idSpecified = customer.customer_idSpecified,
					email = customer.email,
					firstname = customer.firstname,
					group_id = customer.group_id,
					group_idSpecified = customer.group_idSpecified,
					lastname = customer.lastname,
					mode = "customer",
					password = customerPass,
					store_id = customer.store_id,
					store_idSpecified = customer.store_idSpecified,
					website_id = customer.website_id,
					website_idSpecified = customer.website_idSpecified
				};
				var res = await this._magentoSoapService.shoppingCartCustomerSetAsync( sessionId.SessionId, shoppingCart, customerShoppingCart, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var customer = new shoppingCartCustomerEntity
				{
					email = customerMail,
					firstname = customerfirstname,
					lastname = customerlastname,
					website_id = 0,
					store_id = 0,
					mode = "guest",
				};

				var res = await this._magentoSoapService.shoppingCartCustomerSetAsync( sessionId.SessionId, shoppingCart, customer, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartAddressSet( int shoppingCart, string store, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var customerAddressEntities = new shoppingCartCustomerAddressEntity[ 2 ];

				customerAddressEntities[ 0 ] = new shoppingCartCustomerAddressEntity
				{
					mode = "shipping",
					firstname = "testFirstname",
					lastname = "testLastname",
					company = "testCompany",
					street = "testStreet",
					city = "testCity",
					region = "testRegion",
					postcode = "testPostcode",
					country_id = "1",
					telephone = "0123456789",
					fax = "0123456789",
					is_default_shipping = 0,
					is_default_billing = 0
				};
				customerAddressEntities[ 1 ] = new shoppingCartCustomerAddressEntity
				{
					mode = "billing",
					firstname = "testFirstname",
					lastname = "testLastname",
					company = "testCompany",
					street = "testStreet",
					city = "testCity",
					region = "testRegion",
					postcode = "testPostcode",
					country_id = "1",
					telephone = "0123456789",
					fax = "0123456789",
					is_default_shipping = 0,
					is_default_billing = 0
				};

				var res = await this._magentoSoapService.shoppingCartCustomerAddressesAsync( sessionId.SessionId, shoppingCart, customerAddressEntities, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< bool > DeleteCustomer( int customerId, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var res = await this._magentoSoapService.customerCustomerDeleteAsync( sessionId.SessionId, customerId ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during DeleteCustomer()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var shoppingCartProductEntities = new shoppingCartProductEntity[ 1 ];

				shoppingCartProductEntities[ 0 ] = new shoppingCartProductEntity { product_id = productId, qty = 3 };

				var res = await this._magentoSoapService.shoppingCartProductAddAsync( sessionId.SessionId, shoppingCartId, shoppingCartProductEntities, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during ShoppingCartAddProduct()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var cartPaymentMethodEntity = new shoppingCartPaymentMethodEntity
				{
					po_number = null,
					//method = "checkmo",
					method = "checkmo",
					//method = "'cashondelivery'",
					cc_cid = null,
					cc_owner = null,
					cc_number = null,
					cc_type = null,
					cc_exp_year = null,
					cc_exp_month = null
				};

				var res = await this._magentoSoapService.shoppingCartPaymentMethodAsync( sessionId.SessionId, shoppingCartId, cartPaymentMethodEntity, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during ShoppingCartAddProduct()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				var res = await this._magentoSoapService.shoppingCartShippingListAsync( sessionId.SessionId, shoppingCartId, store ).ConfigureAwait( false );

				var shippings = res.result;
				var shipping = shippings.First();

				var shippingMethodResponse = await this._magentoSoapService.shoppingCartShippingMethodAsync( sessionId.SessionId, shoppingCartId, shipping.code, store ).ConfigureAwait( false );

				return shippingMethodResponse.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during ShoppingCartAddProduct()" ), exc );
			}
		}

		public async Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType, CancellationToken cancellationToken, Mark markForLog )
		{
			try
			{
				var result = 0;
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );
					var res0 = await this._magentoSoapService.catalogCategoryAttributeCurrentStoreAsync( sessionId.SessionId, storeId ).ConfigureAwait( false );

					var catalogProductCreateEntity = new catalogProductCreateEntity
					{
						name = name,
						description = "Product description",
						short_description = "Product short description",
						weight = "10",
						status = "1",
						visibility = "4",
						price = "100",
						tax_class_id = "1",
						categories = new[] { res0.result.ToString() },
						category_ids = new[] { res0.result.ToString() },
						stock_data = new catalogInventoryStockItemUpdateEntity { qty = "100", is_in_stockSpecified = true, is_in_stock = isInStock, manage_stock = 1, use_config_manage_stock = 0, use_config_min_qty = 0, use_config_min_sale_qty = 0, is_qty_decimal = 0 }
					};

					var res = await this._magentoSoapService.catalogProductCreateAsync( sessionId.SessionId, "simple", "4", sku, catalogProductCreateEntity, storeId ).ConfigureAwait( false );

					//product id
					result = res.result;
				} ).ConfigureAwait( false );
				return result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during CreateProduct({storeId})", exc );
			}
		}

		public async Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType, CancellationToken cancellationToken )
		{
			try
			{
				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );
				var res = await this._magentoSoapService.catalogCategoryRemoveProductAsync( sessionId.SessionId, categoryId, productId, identiferType ).ConfigureAwait( false );

				//product id
				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during DeleteProduct({storeId})", exc );
			}
		}
		#endregion

		private static class ClientBaseActionRunner
		{
			public static async Task< Tuple< TClientResponse, bool > > RunWithAbortAsync< TClientResponse, TClient >( int delayBeforeCheck, Func< Task< TClientResponse > > func, ClientBase< TClient > cleintBase ) where TClient : class
			{
				var statusChecker = new StatusChecker( 2 );
				TimerCallback tcb = statusChecker.CheckStatus3< TClient >;

				using( var stateTimer = new Timer( tcb, cleintBase, 1000, delayBeforeCheck ) )
				{
					var clientResponse = await func().ConfigureAwait( false );
					stateTimer.Change( Timeout.Infinite, Timeout.Infinite );
					return Tuple.Create( clientResponse, statusChecker.IsAborted );
				}
			}
		}

		private async Task< TResult > GetWithAsync< TResult, TServerResponse >( Func< TServerResponse, TResult > converter, Func< Mage_Api_Model_Server_Wsi_HandlerPortTypeClient, string, Task< TServerResponse > > action, int abortAfter, CancellationToken cancellationToken, bool suppressException = false, [ CallerMemberName ] string callerName = null ) where TServerResponse : new()
		{
			try
			{
				var res = new TServerResponse();
				var privateClient = this._clientFactory.GetClient();

				await ActionPolicies.GetAsync.Do( async () =>
				{
					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

					var temp = await ClientBaseActionRunner.RunWithAbortAsync(
						abortAfter,
						async () => res = await action( privateClient, sessionId.SessionId ).ConfigureAwait( false ),
						privateClient ).ConfigureAwait( false );

					if( temp.Item2 )
						throw new TaskCanceledException();
				} ).ConfigureAwait( false );

				return converter( res );
			}
			catch( Exception exc )
			{
				if( suppressException )
				{
					return default(TResult);
				}
				throw new MagentoSoapException( $"An error occured during{callerName}->{nameof( this.GetWithAsync )}", exc );
			}
		}

		public string ToJsonSoapInfo()
		{
			return string.Format( "{{BaseMagentoUrl:{0}, ApiUser:{1},ApiKey:{2},Store:{3}}}",
				string.IsNullOrWhiteSpace( this.BaseMagentoUrl ) ? PredefinedValues.NotAvailable : this.BaseMagentoUrl,
				string.IsNullOrWhiteSpace( this.ApiUser ) ? PredefinedValues.NotAvailable : this.ApiUser,
				string.IsNullOrWhiteSpace( this.ApiKey ) ? PredefinedValues.NotAvailable : this.ApiKey,
				string.IsNullOrWhiteSpace( this.Store ) ? PredefinedValues.NotAvailable : this.Store
				);
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
	}
}