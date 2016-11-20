using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Magento2backendModuleServiceV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2catalogCategoryManagementV1_v_2_1_0_0_CE;
using MagentoAccess.M2catalogInventoryStockRegistryV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2catalogProductAttributeMediaGalleryManagementV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2catalogProductRepositoryV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2integrationAdminTokenServiceV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2salesOrderRepositoryV1_v_2_1_0_0_CE;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetSessionId;
using Netco.Extensions;

namespace MagentoAccess.Services.Soap._2_1_0_0_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_2_1_0_0_ce : IMagentoServiceLowLevelSoap
	{
		public string ApiUser { get; private set; }

		public string ApiKey { get; private set; }

		public string Store { get; private set; }

		public string BaseMagentoUrl { get; set; }

		public string TokenSecret { get; set; }

		public Func< Task< Tuple< string, DateTime > > > PullSessionId { get; set; }

		//protected const string SoapApiUrl = "soap/default?wsdl&services=";
		protected const string SoapApiUrl = "soap/default?services=";

		protected salesOrderRepositoryV1PortTypeClient _magentoSoapService;

		protected integrationAdminTokenServiceV1PortTypeClient _magentoSoapService2;

		protected string _sessionId;

		protected DateTime _sessionIdCreatedAt;

		private readonly CustomBinding _customBinding;

		private readonly CustomBinding _customBinding12;

		protected SemaphoreSlim getSessionIdSemaphore;

		protected const int SessionIdLifeTime = 3590;

		public bool GetStockItemsWithoutSkuImplementedWithPages
		{
			get { return true; }
		}

		private void LogTraceGetResponseException( Exception exception )
		{
			MagentoLogger.Log().Trace( exception, "[magento] SOAP throw an exception." );
		}

		public async Task< GetSessionIdResponse > GetSessionId( bool throwException = true )
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

		public MagentoServiceLowLevelSoap_v_2_1_0_0_ce( string apiUser, string apiKey, string baseMagentoUrl, string store )
		{
			this.ApiUser = apiUser;
			this.ApiKey = apiKey;
			this.Store = store;
			this.BaseMagentoUrl = baseMagentoUrl;

			this._customBinding = CustomBinding( baseMagentoUrl, MessageVersion.Soap11 );
			this._customBinding12 = CustomBinding( baseMagentoUrl, MessageVersion.Soap12 );
			this.PullSessionId = async () =>
			{
				if( !string.IsNullOrWhiteSpace( apiUser ) )
				{
					var privateClient = this.CreateMagentoServiceAdminClient( this.BaseMagentoUrl );
					var integrationAdminTokenServiceV1CreateAdminAccessTokenRequest = new IntegrationAdminTokenServiceV1CreateAdminAccessTokenRequest() { username = this.ApiUser, password = this.ApiKey };
					var loginResponse = await privateClient.integrationAdminTokenServiceV1CreateAdminAccessTokenAsync( integrationAdminTokenServiceV1CreateAdminAccessTokenRequest ).ConfigureAwait( false );
					this._sessionIdCreatedAt = DateTime.UtcNow;
					this._sessionId = loginResponse.integrationAdminTokenServiceV1CreateAdminAccessTokenResponse.result;
				}
				else
				{
					this._sessionId = this.ApiKey;
				}

				return Tuple.Create( this._sessionId, DateTime.UtcNow );
			};

			this.getSessionIdSemaphore = new SemaphoreSlim( 1, 1 );
		}

		private integrationAdminTokenServiceV1PortTypeClient CreateMagentoServiceAdminClient( string baseMagentoUrl )
		{
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl + "integrationAdminTokenServiceV1" }.BuildUrl( trimTailsSlash : true );
			var customBinding = CustomBinding( baseMagentoUrl, MessageVersion.Soap12 );
			var magentoSoapService = new integrationAdminTokenServiceV1PortTypeClient( customBinding, new EndpointAddress( endPoint ) );

			magentoSoapService.Endpoint.Behaviors.Add( new ChannelBehaviour.CustomBehavior() { AccessToken = this._sessionId } );

			return magentoSoapService;
		}

		private catalogProductAttributeMediaGalleryManagementV1PortTypeClient CreateMagentocatalogProductAttributeMediaGalleryRepositoryServiceClient( string baseMagentoUrl )
		{
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl + "catalogProductAttributeMediaGalleryManagementV1" }.BuildUrl( trimTailsSlash : true );
			var magentoSoapService = new catalogProductAttributeMediaGalleryManagementV1PortTypeClient( this._customBinding, new EndpointAddress( endPoint ) );

			magentoSoapService.Endpoint.Behaviors.Add( new ChannelBehaviour.CustomBehavior() { AccessToken = this._sessionId } );

			return magentoSoapService;
		}

		private salesOrderRepositoryV1PortTypeClient CreateMagentoSalesOrderRepositoryServiceClient( string baseMagentoUrl )
		{
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl + "salesOrderRepositoryV1" }.BuildUrl( trimTailsSlash : true );
			var magentoSoapService = new salesOrderRepositoryV1PortTypeClient( this._customBinding12, new EndpointAddress( endPoint ) );

			magentoSoapService.Endpoint.Behaviors.Add( new ChannelBehaviour.CustomBehavior() { AccessToken = this._sessionId } );

			return magentoSoapService;
		}

		private catalogCategoryManagementV1PortTypeClient CreateMagentoCategoriesRepositoryServiceClient( string baseMagentoUrl )
		{
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl + "catalogCategoryManagementV1" }.BuildUrl( trimTailsSlash : true );
			var magentoSoapService = new catalogCategoryManagementV1PortTypeClient( this._customBinding, new EndpointAddress( endPoint ) );

			magentoSoapService.Endpoint.Behaviors.Add( new ChannelBehaviour.CustomBehavior() { AccessToken = this._sessionId } );

			return magentoSoapService;
		}

		private catalogProductRepositoryV1PortTypeClient CreateMagentoCatalogProductRepositoryServiceClient( string baseMagentoUrl )
		{
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl + "catalogProductRepositoryV1" }.BuildUrl( trimTailsSlash : true );

			var customBinding = CustomBinding( baseMagentoUrl, MessageVersion.Soap12 );
			var magentoSoapService = new catalogProductRepositoryV1PortTypeClient( customBinding, new EndpointAddress( endPoint ) );

			magentoSoapService.Endpoint.Behaviors.Add( new ChannelBehaviour.CustomBehavior() { AccessToken = this._sessionId } );

			return magentoSoapService;
		}

		private backendModuleServiceV1PortTypeClient CreateMagentoBackendModuleServiceV1Client( string baseMagentoUrl )
		{
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl + "backendModuleServiceV1" }.BuildUrl( trimTailsSlash : true );

			var customBinding = CustomBinding( baseMagentoUrl, MessageVersion.Soap12 );
			var magentoSoapService = new backendModuleServiceV1PortTypeClient( customBinding, new EndpointAddress( endPoint ) );

			magentoSoapService.Endpoint.Behaviors.Add( new ChannelBehaviour.CustomBehavior() { AccessToken = this._sessionId } );

			return magentoSoapService;
		}

		private catalogInventoryStockRegistryV1PortTypeClient CreateMagentoCatalogInventoryStockServiceClient( string baseMagentoUrl )
		{
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl + "catalogInventoryStockRegistryV1" }.BuildUrl( trimTailsSlash : true );
			var magentoSoapService = new catalogInventoryStockRegistryV1PortTypeClient( this._customBinding, new EndpointAddress( endPoint ) );

			magentoSoapService.Endpoint.Behaviors.Add( new ChannelBehaviour.CustomBehavior() { AccessToken = this._sessionId } );

			return magentoSoapService;
		}

		private async Task< salesOrderRepositoryV1PortTypeClient > CreateMagentoServiceClientAsync( string baseMagentoUrl )
		{
			var task = Task.Factory.StartNew( () => this.CreateMagentoSalesOrderRepositoryServiceClient( baseMagentoUrl ) );
			await Task.WhenAll( task ).ConfigureAwait( false );
			return task.Result;
		}

		private static CustomBinding CustomBinding( string baseMagentoUrl, MessageVersion messageVersion )
		{
			var textMessageEncodingBindingElement = new TextMessageEncodingBindingElement
			{
				MessageVersion = messageVersion,
				WriteEncoding = new UTF8Encoding()
			};

			BindingElement httpTransportBindingElement;
			if( baseMagentoUrl.StartsWith( "https" ) )
			{
				httpTransportBindingElement = new HttpsTransportBindingElement
				{
					DecompressionEnabled = false,
					MaxReceivedMessageSize = 999999999,
					MaxBufferSize = 999999999,
					MaxBufferPoolSize = 999999999,
					KeepAliveEnabled = true,
					AllowCookies = false,
				};
			}
			else
			{
				httpTransportBindingElement = new HttpTransportBindingElement
				{
					DecompressionEnabled = false,
					MaxReceivedMessageSize = 999999999,
					MaxBufferSize = 999999999,
					MaxBufferPoolSize = 999999999,
					KeepAliveEnabled = true,
					AllowCookies = false,
				};
			}

			var myTextMessageEncodingBindingElement = new CustomMessageEncodingBindingElement( textMessageEncodingBindingElement, "qwe" )
			{
				MessageVersion = messageVersion,
			};

			ICollection< BindingElement > bindingElements = new List< BindingElement >();
			var httpBindingElement = httpTransportBindingElement;
			var textBindingElement = myTextMessageEncodingBindingElement;
			bindingElements.Add( textBindingElement );
			bindingElements.Add( httpBindingElement );

			var customBinding = new CustomBinding( bindingElements ) { ReceiveTimeout = new TimeSpan( 0, 2, 30, 0 ), SendTimeout = new TimeSpan( 0, 2, 30, 0 ), OpenTimeout = new TimeSpan( 0, 2, 30, 0 ), CloseTimeout = new TimeSpan( 0, 2, 30, 0 ), Name = "CustomHttpBinding" };
			return customBinding;
		}

		public virtual async Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException )
		{
			try
			{
				// Magento doesn't provide method to receive magento vesrion, since Magento2.0 thats why we use backEndMoodules API

				var sessionIdRespnse = await this.GetSessionId( !suppressException ).ConfigureAwait( false );
				if( sessionIdRespnse == null )
				{
					MagentoLogger.LogTrace( "Can't get session id. Possible reasons: incorrect credentials, user was blocked." );
					return null;
				}
				var modules = await this.GetBackEndModulesAsync().ConfigureAwait( false );
				var getOrdersResponse = await this.GetOrdersAsync( DateTime.Now, DateTime.Now.AddHours( 1 ) ).ConfigureAwait( false );
				var getProductsRes = await this.GetProductsAsync( 1, null, false, null ).ConfigureAwait( false );

				//var saveMethodResult = await this.SaveOrderMethodExistAsync().ConfigureAwait( false );
				return modules?.Modules != null && modules.Modules.Count > 0 && getOrdersResponse.Orders.Count() >= 0 && getProductsRes.Products.Count() >= 0 ? new GetMagentoInfoResponse( "2.1.0.0", "CE" ) : null;
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
		public async Task< int > CreateCart( string storeid )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var res = await this._magentoSoapService.shoppingCartCreateAsync(sessionId, storeid).ConfigureAwait(false);

			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during GetMagentoInfoAsync({0})", storeid), exc);
			//}
			return await Task.FromResult( 0 ).ConfigureAwait( false );
		}

		public async Task< string > CreateOrder( int shoppingcartid, string store )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var res = await this._magentoSoapService.shoppingCartOrderAsync(sessionId, shoppingcartid, store, null).ConfigureAwait(false);

			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during GetMagentoInfoAsync()"), exc);
			//}
			return await Task.FromResult( "" ).ConfigureAwait( false );
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
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var customerCustomerEntityToCreate = new customerCustomerEntityToCreate
			//	{
			//		email = email,
			//		firstname = firstname,
			//		lastname = lastname,
			//		password = password,
			//		website_id = websiteId,
			//		store_id = storeId,
			//		group_id = groupId
			//	};
			//	var res = await this._magentoSoapService.customerCustomerCreateAsync(sessionId, customerCustomerEntityToCreate).ConfigureAwait(false);

			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during GetMagentoInfoAsync()"), exc);
			//}
			return await Task.FromResult( 0 ).ConfigureAwait( false );
		}

		public async Task< bool > ShoppingCartCustomerSet( int shoppingCart, int customerId, string customerPass, string store )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var cutomers = await this._magentoSoapService.customerCustomerListAsync(sessionId, new filters()).ConfigureAwait(false);

			//	var customer = cutomers.result.First(x => x.customer_id == customerId);

			//	var customerShoppingCart = new shoppingCartCustomerEntity
			//	{
			//		confirmation = (customer.confirmation ? 1 : 0).ToString(CultureInfo.InvariantCulture),
			//		customer_id = customer.customer_id,
			//		customer_idSpecified = customer.customer_idSpecified,
			//		email = customer.email,
			//		firstname = customer.firstname,
			//		group_id = customer.group_id,
			//		group_idSpecified = customer.group_idSpecified,
			//		lastname = customer.lastname,
			//		mode = "customer",
			//		password = customerPass,
			//		store_id = customer.store_id,
			//		store_idSpecified = customer.store_idSpecified,
			//		website_id = customer.website_id,
			//		website_idSpecified = customer.website_idSpecified
			//	};
			//	var res = await this._magentoSoapService.shoppingCartCustomerSetAsync(sessionId, shoppingCart, customerShoppingCart, store).ConfigureAwait(false);

			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during GetMagentoInfoAsync()"), exc);
			//}
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public async Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var customer = new shoppingCartCustomerEntity
			//	{
			//		email = customerMail,
			//		firstname = customerfirstname,
			//		lastname = customerlastname,
			//		website_id = 0,
			//		store_id = 0,
			//		mode = "guest",
			//	};

			//	var res = await this._magentoSoapService.shoppingCartCustomerSetAsync(sessionId, shoppingCart, customer, store).ConfigureAwait(false);

			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during GetMagentoInfoAsync()"), exc);
			//}
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public async Task< bool > ShoppingCartAddressSet( int shoppingCart, string store )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var customerAddressEntities = new shoppingCartCustomerAddressEntity[2];

			//	customerAddressEntities[0] = new shoppingCartCustomerAddressEntity
			//	{
			//		mode = "shipping",
			//		firstname = "testFirstname",
			//		lastname = "testLastname",
			//		company = "testCompany",
			//		street = "testStreet",
			//		city = "testCity",
			//		region = "testRegion",
			//		postcode = "testPostcode",
			//		country_id = "1",
			//		telephone = "0123456789",
			//		fax = "0123456789",
			//		is_default_shipping = 0,
			//		is_default_billing = 0
			//	};
			//	customerAddressEntities[1] = new shoppingCartCustomerAddressEntity
			//	{
			//		mode = "billing",
			//		firstname = "testFirstname",
			//		lastname = "testLastname",
			//		company = "testCompany",
			//		street = "testStreet",
			//		city = "testCity",
			//		region = "testRegion",
			//		postcode = "testPostcode",
			//		country_id = "1",
			//		telephone = "0123456789",
			//		fax = "0123456789",
			//		is_default_shipping = 0,
			//		is_default_billing = 0
			//	};

			//	var res = await this._magentoSoapService.shoppingCartCustomerAddressesAsync(sessionId, shoppingCart, customerAddressEntities, store).ConfigureAwait(false);

			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during GetMagentoInfoAsync()"), exc);
			//}
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public async Task< bool > DeleteCustomer( int customerId )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var res = await this._magentoSoapService.customerCustomerDeleteAsync(sessionId, customerId).ConfigureAwait(false);

			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during DeleteCustomer()"), exc);
			//}
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public async Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var shoppingCartProductEntities = new shoppingCartProductEntity[1];

			//	shoppingCartProductEntities[0] = new shoppingCartProductEntity { product_id = productId, qty = 3 };

			//	var res = await this._magentoSoapService.shoppingCartProductAddAsync(sessionId, shoppingCartId, shoppingCartProductEntities, store).ConfigureAwait(false);

			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during ShoppingCartAddProduct()"), exc);
			//}
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public async Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var cartPaymentMethodEntity = new shoppingCartPaymentMethodEntity
			//	{
			//		po_number = null,
			//		//method = "checkmo",
			//		method = "checkmo",
			//		//method = "'cashondelivery'",
			//		cc_cid = null,
			//		cc_owner = null,
			//		cc_number = null,
			//		cc_type = null,
			//		cc_exp_year = null,
			//		cc_exp_month = null
			//	};

			//	var res = await this._magentoSoapService.shoppingCartPaymentMethodAsync(sessionId, shoppingCartId, cartPaymentMethodEntity, store).ConfigureAwait(false);

			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during ShoppingCartAddProduct()"), exc);
			//}
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public async Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);

			//	var res = await this._magentoSoapService.shoppingCartShippingListAsync(sessionId, shoppingCartId, store).ConfigureAwait(false);

			//	var shippings = res.result;
			//	var shipping = shippings.First();

			//	var shippingMethodResponse = await this._magentoSoapService.shoppingCartShippingMethodAsync(sessionId, shoppingCartId, shipping.code, store).ConfigureAwait(false);

			//	return shippingMethodResponse.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during ShoppingCartAddProduct()"), exc);
			//}
			return await Task.FromResult( false ).ConfigureAwait( false );
		}

		public class CreatteProductModel
		{
			public int IsInStock { get; }
			public string Name { get; }
			public string ProductType { get; }
			public string Sku { get; }

			public CreatteProductModel( string name, string sku, int isInStock, string productType )
			{
				this.Name = name;
				this.Sku = sku;
				this.IsInStock = isInStock;
				this.ProductType = productType;
			}
		}

		public async Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType, Mark markForLog )
		{
			var stockItem = new CreatteProductModel( name, sku, isInStock, productType );
			var methodParameters = stockItem.ToJson();
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoCatalogProductRepositoryServiceClient( this.BaseMagentoUrl );

				var res = new List< UpdateRessult< CreatteProductModel > >();
				var stockItems = new List< CreatteProductModel > { stockItem };

				await stockItems.DoInBatchAsync( 10, async x =>
				{
					await ActionPolicies.GetAsync.Do( async () =>
					{
						var statusChecker = new StatusChecker( maxCheckCount );
						TimerCallback tcb = statusChecker.CheckStatus;

						if( privateClient.State != CommunicationState.Opened
						    && privateClient.State != CommunicationState.Created
						    && privateClient.State != CommunicationState.Opening )
							privateClient = this.CreateMagentoCatalogProductRepositoryServiceClient( this.BaseMagentoUrl );

						var updateResult = new UpdateRessult< CreatteProductModel >( x, 0 );
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

							updateResult.Success = temp.catalogProductRepositoryV1SaveResponse.result.id;
						}
					} ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( methodParameters, mark : markForLog, methodResult : res.ToJson() ) );

				return res.First().Success;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during PutStockItemsAsync({methodParameters})", exc );
			}
		}

		public async Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType )
		{
			//try
			//{
			//	var sessionId = await this.GetSessionId().ConfigureAwait(false);
			//	var res = await this._magentoSoapService.catalogCategoryRemoveProductAsync(sessionId, categoryId, productId, identiferType).ConfigureAwait(false);

			//	//product id
			//	return res.result;
			//}
			//catch (Exception exc)
			//{
			//	throw new MagentoSoapException(string.Format("An error occured during DeleteProduct({0})", storeId), exc);
			//}
			return await Task.FromResult( false ).ConfigureAwait( false );
		}
		#endregion

		private class UpdateRessult< T1 >
		{
			public UpdateRessult( T1 itemToUpdate, int success )
			{
				this.ItemToUpdate = itemToUpdate;
				this.Success = success;
			}

			public int Success { get; set; }
			public T1 ItemToUpdate { get; set; }
		}
	}
}