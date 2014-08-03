using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;

namespace MagentoAccess.Services
{
	internal class MagentoServiceLowLevelSoap : IMagentoServiceLowLevelSoap
	{
		public string ApiUser { get; private set; }

		public string ApiKey { get; private set; }

		public string Store { get; private set; }

		public string BaseMagentoUrl { get; set; }

		protected const string SoapApiUrl = "index.php/api/v2_soap/index/";

		protected Mage_Api_Model_Server_Wsi_HandlerPortTypeClient _magentoSoapService;

		protected string _sessionId;

		protected DateTime _sessionIdCreatedAt;

		protected const int SessionIdLifeTime = 3590;

		private void LogTraceGetResponseException( Exception exception )
		{
			MagentoLogger.Log().Trace( exception, "[magento] SOAP throw an exception." );
		}

		internal async Task< string > GetSessionId( bool throwException = true )
		{
			try
			{
				if( !string.IsNullOrWhiteSpace( this._sessionId ) && DateTime.UtcNow.Subtract( this._sessionIdCreatedAt ).TotalSeconds < SessionIdLifeTime )
					return this._sessionId;

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 120000;
				var statusChecker = new StatusChecker( maxCheckCount );
				TimerCallback tcb = statusChecker.CheckStatus;

				var res = string.Empty;

				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					if( privateClient.State != CommunicationState.Opened )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						var loginResponse = await privateClient.loginAsync( this.ApiUser, this.ApiKey ).ConfigureAwait( false );
						this._sessionIdCreatedAt = DateTime.UtcNow;
						this._sessionId = loginResponse.result;
						res = this._sessionId;
					}
				} ).ConfigureAwait( false );

				return res;
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
		}

		public MagentoServiceLowLevelSoap( string apiUser, string apiKey, string baseMagentoUrl, string store )
		{
			this.ApiUser = apiUser;
			this.ApiKey = apiKey;
			this.Store = store;
			this.BaseMagentoUrl = baseMagentoUrl;

			this._magentoSoapService = this.CreateMagentoServiceClient( baseMagentoUrl );
		}

		private Mage_Api_Model_Server_Wsi_HandlerPortTypeClient CreateMagentoServiceClient( string baseMagentoUrl )
		{
			var textMessageEncodingBindingElement = new TextMessageEncodingBindingElement
			{
				MessageVersion = MessageVersion.Soap11,
				WriteEncoding = new UTF8Encoding()
			};

			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl }.BuildUrl();

			var httpTransportBindingElement = new HttpTransportBindingElement
			{
				DecompressionEnabled = false,
				MaxReceivedMessageSize = 999999999,
				MaxBufferSize = 999999999,
				MaxBufferPoolSize = 999999999,
				KeepAliveEnabled = true,
				AllowCookies = false
			};

			var myTextMessageEncodingBindingElement = new MyTextMessageEncodingBindingElement( textMessageEncodingBindingElement, "qwe" )
			{
				MessageVersion = MessageVersion.Soap11,
			};

			ICollection< BindingElement > bindingElements = new List< BindingElement >();
			var httpBindingElement = httpTransportBindingElement;
			var textBindingElement = myTextMessageEncodingBindingElement;
			bindingElements.Add( textBindingElement );
			bindingElements.Add( httpBindingElement );

			var customBinding = new CustomBinding( bindingElements ) { ReceiveTimeout = new TimeSpan( 0, 2, 30, 0 ), SendTimeout = new TimeSpan( 0, 2, 30, 0 ), OpenTimeout = new TimeSpan( 0, 2, 30, 0 ), CloseTimeout = new TimeSpan( 0, 2, 30, 0 ), Name = "CustomHttpBinding" };

			var magentoSoapService = new Mage_Api_Model_Server_Wsi_HandlerPortTypeClient( customBinding, new EndpointAddress( endPoint ) );

			magentoSoapService.Endpoint.Behaviors.Add( new CustomBehavior() );

			return magentoSoapService;
		}

		public virtual async Task< salesOrderListResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			try
			{
				filters filters;

				if( string.IsNullOrWhiteSpace( this.Store ) )
					filters = new filters { complex_filter = new complexFilter[ 2 ] };
				else
				{
					filters = new filters { complex_filter = new complexFilter[ 3 ] };
					filters.complex_filter[ 2 ] = new complexFilter { key = "store_id", value = new associativeEntity { key = "in", value = this.Store } };
				}

				filters.complex_filter[ 1 ] = new complexFilter { key = "updated_at", value = new associativeEntity { key = "from", value = modifiedFrom.ToSoapParameterString() } };
				filters.complex_filter[ 0 ] = new complexFilter { key = "updated_at", value = new associativeEntity { key = "to", value = modifiedTo.ToSoapParameterString() } };

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;
				var statusChecker = new StatusChecker( maxCheckCount );
				TimerCallback tcb = statusChecker.CheckStatus;

				var res = new salesOrderListResponse();

				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					if( privateClient.State != CommunicationState.Opened )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.salesOrderListAsync( sessionId, filters ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				//crutch for magento 1.7 
				res.result = res.result.Where( x => x.updated_at.ToDateTimeOrDefault() >= modifiedFrom && x.updated_at.ToDateTimeOrDefault() <= modifiedTo ).ToArray();

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrdersAsync(modifiedFrom:{0},modifiedTo{1})", modifiedFrom, modifiedTo ), exc );
			}
		}

		public virtual async Task< salesOrderListResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			var ordersIdsAgregated = string.Empty;
			try
			{
				ordersIdsAgregated = string.Join(",", ordersIds);

				filters filters;
				if( string.IsNullOrWhiteSpace( this.Store ) )
					filters = new filters { complex_filter = new complexFilter[ 1 ] };
				else
				{
					filters = new filters { complex_filter = new complexFilter[ 2 ] };
					filters.complex_filter[ 1 ] = new complexFilter { key = "store_id", value = new associativeEntity { key = "in", value = this.Store } };
				}

				filters.complex_filter[ 0 ] = new complexFilter { key = "increment_id", value = new associativeEntity { key = "in", value = ordersIdsAgregated } };

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;
				var statusChecker = new StatusChecker( maxCheckCount );
				TimerCallback tcb = statusChecker.CheckStatus;

				var res = new salesOrderListResponse();

				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					if( privateClient.State != CommunicationState.Opened )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.salesOrderListAsync( sessionId, filters ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrdersAsync({0})", ordersIdsAgregated ), exc );
			}
		}

		public virtual async Task< catalogProductListResponse > GetProductsAsync()
		{
			try
			{
				var filters = new filters { filter = new associativeEntity[ 0 ] };

				var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;
				var statusChecker = new StatusChecker( maxCheckCount );
				TimerCallback tcb = statusChecker.CheckStatus;

				var res = new catalogProductListResponse();
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					if( privateClient.State != CommunicationState.Opened )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductListAsync( sessionId, filters, store ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		public virtual async Task< catalogInventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			try
			{
				var skusArray = skusOrIds.ToArray();

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;
				var statusChecker = new StatusChecker( maxCheckCount );
				TimerCallback tcb = statusChecker.CheckStatus;

				var res = new catalogInventoryStockItemListResponse();
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					if( privateClient.State != CommunicationState.Opened )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogInventoryStockItemListAsync( sessionId, skusArray ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				var productsBriefInfo = string.Join( "|", skusOrIds );
				throw new MagentoSoapException( string.Format( "An error occured during GetStockItemsAsync({0})", productsBriefInfo ), exc );
			}
		}

		public virtual async Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, string markForLog = "" )
		{
			try
			{
				const string currentMenthodName = "PutStockItemsAsync";
				var jsonSoapInfo = this.ToJsonSoapInfo();
				var productsBriefInfo = stockItems.ToJson();

				stockItems.ForEach( x =>
				{
					if( x.UpdateEntity.qty.ToDecimalOrDefault() > 0 )
					{
						x.UpdateEntity.is_in_stock = 1;
						x.UpdateEntity.is_in_stockSpecified = true;
					}
					else
					{
						x.UpdateEntity.is_in_stock = 0;
						x.UpdateEntity.is_in_stockSpecified = false;
					}
				} );

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;
				var statusChecker = new StatusChecker( maxCheckCount );
				TimerCallback tcb = statusChecker.CheckStatus;

				var res = false;
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					if( privateClient.State != CommunicationState.Opened )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						MagentoLogger.LogTraceStarted( string.Format( "{{MethodName:{0}, Called From:{1}, SoapInfo:{2}, MethodParameters:{3}}}", currentMenthodName, markForLog, jsonSoapInfo, productsBriefInfo ) );

						var temp = await privateClient.catalogInventoryStockItemMultiUpdateAsync( sessionId, stockItems.Select( x => x.Id ).ToArray(), stockItems.Select( x => x.UpdateEntity ).ToArray() ).ConfigureAwait( false );

						res = temp.result;

						var updateBriefInfo = string.Format( "{{Success:{0}}}", res );
						MagentoLogger.LogTraceEnded( string.Format( "{{MethodName:{0}, Called From:{1}, SoapInfo:{2}, MethodParameters:{3}, MethodResult:{4}}}", currentMenthodName, markForLog, jsonSoapInfo, productsBriefInfo, updateBriefInfo ) );
					}
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				var productsBriefInfo = string.Join( "|", stockItems.Select( x => string.Format( "Id:{0}, Qty:{1}", x.Id, x.UpdateEntity.qty ) ) );
				throw new MagentoSoapException( string.Format( "An error occured during PutStockItemsAsync({0})", productsBriefInfo ), exc );
			}
		}

		public virtual async Task< salesOrderInfoResponse > GetOrderAsync( string incrementId )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 300000;
				var statusChecker = new StatusChecker( maxCheckCount );
				TimerCallback tcb = statusChecker.CheckStatus;

				var res = new salesOrderInfoResponse();

				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					if( privateClient.State != CommunicationState.Opened )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.salesOrderInfoAsync( sessionId, incrementId ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrderAsync(incrementId:{0})", incrementId ), exc );
			}
		}

		public virtual async Task< magentoInfoResponse > GetMagentoInfoAsync()
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;
				var statusChecker = new StatusChecker( maxCheckCount );
				TimerCallback tcb = statusChecker.CheckStatus;

				var res = new magentoInfoResponse();
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					if( privateClient.State != CommunicationState.Opened )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.magentoInfoAsync( sessionId ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
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

		#region JustForTesting
		public async Task< int > CreateCart( string storeid )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var res = await this._magentoSoapService.shoppingCartCreateAsync( sessionId, storeid ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync({0})", storeid ), exc );
			}
		}

		public async Task< string > CreateOrder( int shoppingcartid, string store )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var res = await this._magentoSoapService.shoppingCartOrderAsync( sessionId, shoppingcartid, store, null ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< int > CreateCustomer(
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
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

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
				var res = await this._magentoSoapService.customerCustomerCreateAsync( sessionId, customerCustomerEntityToCreate ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartCustomerSet( int shoppingCart, int customerId, string customerPass, string store )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var cutomers = await this._magentoSoapService.customerCustomerListAsync( sessionId, new filters() ).ConfigureAwait( false );

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
				var res = await this._magentoSoapService.shoppingCartCustomerSetAsync( sessionId, shoppingCart, customerShoppingCart, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var customer = new shoppingCartCustomerEntity
				{
					email = customerMail,
					firstname = customerfirstname,
					lastname = customerlastname,
					website_id = 0,
					store_id = 0,
					mode = "guest",
				};

				var res = await this._magentoSoapService.shoppingCartCustomerSetAsync( sessionId, shoppingCart, customer, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartAddressSet( int shoppingCart, string store )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

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

				var res = await this._magentoSoapService.shoppingCartCustomerAddressesAsync( sessionId, shoppingCart, customerAddressEntities, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
			}
		}

		public async Task< bool > DeleteCustomer( int customerId )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var res = await this._magentoSoapService.customerCustomerDeleteAsync( sessionId, customerId ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during DeleteCustomer()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var shoppingCartProductEntities = new shoppingCartProductEntity[ 1 ];

				shoppingCartProductEntities[ 0 ] = new shoppingCartProductEntity { product_id = productId, qty = 3 };

				var res = await this._magentoSoapService.shoppingCartProductAddAsync( sessionId, shoppingCartId, shoppingCartProductEntities, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during ShoppingCartAddProduct()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

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

				var res = await this._magentoSoapService.shoppingCartPaymentMethodAsync( sessionId, shoppingCartId, cartPaymentMethodEntity, store ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during ShoppingCartAddProduct()" ), exc );
			}
		}

		public async Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var res = await this._magentoSoapService.shoppingCartShippingListAsync( sessionId, shoppingCartId, store ).ConfigureAwait( false );

				var shippings = res.result;
				var shipping = shippings.First();

				var shippingMethodResponse = await this._magentoSoapService.shoppingCartShippingMethodAsync( sessionId, shoppingCartId, shipping.code, store ).ConfigureAwait( false );

				return shippingMethodResponse.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during ShoppingCartAddProduct()" ), exc );
			}
		}

		public async Task< int > CreateProduct( string storeId, string name, string sku, int isInStock )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

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
					stock_data = new catalogInventoryStockItemUpdateEntity { qty = "100", is_in_stockSpecified = true, is_in_stock = isInStock, manage_stock = 1, use_config_manage_stock = 0, use_config_min_qty = 0, use_config_min_sale_qty = 0, is_qty_decimal = 0 }
				};

				var res = await this._magentoSoapService.catalogProductCreateAsync( sessionId, "simple", "4", sku, catalogProductCreateEntity, storeId ).ConfigureAwait( false );

				//product id
				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during CreateProduct({0})", storeId ), exc );
			}
		}
		#endregion

	}

	internal class StatusChecker
	{
		private int invokeCount;
		private readonly int maxCount;

		public StatusChecker( int count )
		{
			this.invokeCount = 0;
			this.maxCount = count;
		}

		public void CheckStatus( Object stateInfo )
		{
			var serviceClient = ( Mage_Api_Model_Server_Wsi_HandlerPortTypeClient )stateInfo;
			Interlocked.Increment( ref this.invokeCount );
			if( this.invokeCount == this.maxCount )
			{
				Interlocked.Exchange( ref this.invokeCount, 0 );
				serviceClient.Abort();
			}
		}
	}

	internal class PutStockItem
	{
		public PutStockItem( string id, catalogInventoryStockItemUpdateEntity updateEntity )
		{
			this.Id = id;
			this.UpdateEntity = updateEntity;
		}

		public catalogInventoryStockItemUpdateEntity UpdateEntity { get; set; }

		public string Id { get; set; }
	}
}