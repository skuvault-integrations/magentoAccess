using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;

namespace MagentoAccess.Services.Soap._1_9_2_1_ce
{
	internal class MagentoServiceLowLevelSoap_v_1_9_2_1_ce: IMagentoServiceLowLevelSoap
	{
		public string ApiUser{ get; private set; }

		public string ApiKey{ get; private set; }

		public string Store{ get; private set; }

		public string BaseMagentoUrl{ get; set; }

		protected IMagento1XxxHelper Magento1xxxHelper{ get; set; }

		protected const string SoapApiUrl = "index.php/api/v2_soap/index/";

		protected Mage_Api_Model_Server_Wsi_HandlerPortTypeClient _magentoSoapService;

		protected string _sessionId;

		protected DateTime _sessionIdCreatedAt;

		private readonly CustomBinding _customBinding;

		protected const int SessionIdLifeTime = 3590;

		private void LogTraceGetResponseException( Exception exception )
		{
			MagentoLogger.Log().Trace( exception, "[magento] SOAP throw an exception." );
		}

		public async Task< string > GetSessionId( bool throwException = true )
		{
			try
			{
				if( !string.IsNullOrWhiteSpace( this._sessionId ) && DateTime.UtcNow.Subtract( this._sessionIdCreatedAt ).TotalSeconds < SessionIdLifeTime )
					return this._sessionId;

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 120000;

				var res = string.Empty;

				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				//await ActionPolicies.GetAsync.Do( async () =>
				//{
				//	var statusChecker = new StatusChecker(maxCheckCount);
				//	TimerCallback tcb = statusChecker.CheckStatus;

				if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
					privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				//	using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
				{
					var loginResponse = await privateClient.loginAsync( this.ApiUser, this.ApiKey ).ConfigureAwait( false );
					this._sessionIdCreatedAt = DateTime.UtcNow;
					this._sessionId = loginResponse.result;
					res = this._sessionId;
				}
				//} ).ConfigureAwait( false );

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

		public MagentoServiceLowLevelSoap_v_1_9_2_1_ce( string apiUser, string apiKey, string baseMagentoUrl, string store )
		{
			this.ApiUser = apiUser;
			this.ApiKey = apiKey;
			this.Store = store;
			this.BaseMagentoUrl = baseMagentoUrl;

			_customBinding = CustomBinding( baseMagentoUrl );
			this._magentoSoapService = this.CreateMagentoServiceClient( baseMagentoUrl );
			this.Magento1xxxHelper = new Magento1xxxHelper( this );
		}

		private Mage_Api_Model_Server_Wsi_HandlerPortTypeClient CreateMagentoServiceClient( string baseMagentoUrl )
		{
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl }.BuildUrl();
			var magentoSoapService = new Mage_Api_Model_Server_Wsi_HandlerPortTypeClient( _customBinding, new EndpointAddress( endPoint ) );

			magentoSoapService.Endpoint.Behaviors.Add( new CustomBehavior() );

			return magentoSoapService;
		}

		private async Task< Mage_Api_Model_Server_Wsi_HandlerPortTypeClient > CreateMagentoServiceClientAsync( string baseMagentoUrl )
		{
			var task = Task.Factory.StartNew( () => CreateMagentoServiceClient( baseMagentoUrl ) );
			await Task.WhenAll( task ).ConfigureAwait( false );
			return task.Result;
		}

		private static CustomBinding CustomBinding( string baseMagentoUrl )
		{
			var textMessageEncodingBindingElement = new TextMessageEncodingBindingElement
			{
				MessageVersion = MessageVersion.Soap11,
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
				MessageVersion = MessageVersion.Soap11,
			};

			ICollection< BindingElement > bindingElements = new List< BindingElement >();
			var httpBindingElement = httpTransportBindingElement;
			var textBindingElement = myTextMessageEncodingBindingElement;
			bindingElements.Add( textBindingElement );
			bindingElements.Add( httpBindingElement );

			var customBinding = new CustomBinding( bindingElements ) { ReceiveTimeout = new TimeSpan( 0, 2, 30, 0 ), SendTimeout = new TimeSpan( 0, 2, 30, 0 ), OpenTimeout = new TimeSpan( 0, 2, 30, 0 ), CloseTimeout = new TimeSpan( 0, 2, 30, 0 ), Name = "CustomHttpBinding" };
			return customBinding;
		}

		public virtual async Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
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

				var res = new salesOrderListResponse();

				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.salesOrderListAsync( sessionId, filters ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				//crutch for magento 1.7 
				res.result = res.result.Where( x => Extensions.ToDateTimeOrDefault( x.updated_at ) >= modifiedFrom && Extensions.ToDateTimeOrDefault( x.updated_at ) <= modifiedTo ).ToArray();

				return new GetOrdersResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrdersAsync(modifiedFrom:{0},modifiedTo{1})", modifiedFrom, modifiedTo ), exc );
			}
		}

		public virtual async Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			var ordersIdsAgregated = string.Empty;
			try
			{
				ordersIdsAgregated = string.Join( ",", ordersIds );

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

				var res = new salesOrderListResponse();

				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.salesOrderListAsync( sessionId, filters ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new GetOrdersResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrdersAsync({0})", ordersIdsAgregated ), exc );
			}
		}

		public virtual async Task< SoapGetProductsResponse > GetProductsAsync()
		{
			try
			{
				var filters = new filters { filter = new associativeEntity[ 0 ] };

				var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductListResponse();
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductListAsync( sessionId, filters, store ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new SoapGetProductsResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		public virtual async Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			try
			{
				var skusArray = skusOrIds.ToArray();

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogInventoryStockItemListResponse();
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogInventoryStockItemListAsync( sessionId, skusArray ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new InventoryStockItemListResponse( res );
			}
			catch( Exception exc )
			{
				var productsBriefInfo = string.Join( "|", skusOrIds );
				throw new MagentoSoapException( string.Format( "An error occured during GetStockItemsAsync({0})", productsBriefInfo ), exc );
			}
		}

		public virtual async Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				var res = new catalogProductAttributeMediaListResponse();
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductAttributeMediaListAsync( sessionId, getProductAttributeMediaListRequest.ProductId, "0", "1" ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new ProductAttributeMediaListResponse( res, getProductAttributeMediaListRequest.ProductId, getProductAttributeMediaListRequest.Sku );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductAttributeMediaListAsync({0})", getProductAttributeMediaListRequest ), exc );
			}
		}

		public virtual async Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( string rootCategory = "1" )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				var res = new catalogCategoryTreeResponse();
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogCategoryTreeAsync( sessionId, rootCategory, "0" ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new GetCategoryTreeResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetCategoriesTree({0})", rootCategory ), exc );
			}
		}

		public virtual async Task< CatalogProductInfoResponse > GetProductInfoAsync( CatalogProductInfoRequest request )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductInfoResponse();
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );
					var attributes = new catalogProductRequestAttributes { additional_attributes = request.custAttributes ?? new string[ 0 ] };

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductInfoAsync( sessionId, request.ProductId, "0", attributes, "1" ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new CatalogProductInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductInfoAsync({0})", request.ToJson() ), exc );
			}
		}

		public virtual async Task< CatalogProductAttributeInfoResponse > GetManufacturersInfoAsync( string attribute )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductAttributeInfoResponse();
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductAttributeInfoAsync( sessionId, attribute ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new CatalogProductAttributeInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetManufacturerAsync()" ), exc );
			}
		}

		public virtual async Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts )
		{
			return await this.Magento1xxxHelper.FillProductDetails( resultProducts ).ConfigureAwait( false );
		}

		public virtual async Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, Mark markForLog = null )
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

				var res = false;
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						MagentoLogger.LogTraceStarted( CreateMethodCallInfo( methodParameters, mark : markForLog ) );

						var temp = await privateClient.catalogInventoryStockItemMultiUpdateAsync( sessionId, stockItemsProcessed.Select( x => x.Item1.ProductId ).ToArray(), stockItemsProcessed.Select( x => x.Item2 ).ToArray() ).ConfigureAwait( false );

						res = temp.result;

						var updateBriefInfo = string.Format( "{{Success:{0}}}", res );
						MagentoLogger.LogTraceEnded( CreateMethodCallInfo( methodParameters, mark : markForLog, methodResult : updateBriefInfo ) );
					}
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during PutStockItemsAsync({0})", methodParameters ), exc );
			}
		}

		public virtual async Task< bool > PutStockItemAsync( PutStockItem putStockItem, Mark markForLog )
		{
			var productsBriefInfo = new List< PutStockItem > { putStockItem }.ToJson();

			try
			{
				var catalogInventoryStockItemUpdateEntity = ( putStockItem.Qty > 0 ) ?
					new catalogInventoryStockItemUpdateEntity() { is_in_stock = 1, is_in_stockSpecified = true, qty = putStockItem.Qty.ToString() } :
					new catalogInventoryStockItemUpdateEntity() { is_in_stock = 0, is_in_stockSpecified = false, qty = putStockItem.Qty.ToString() };

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 120000;

				var res = false;
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						MagentoLogger.LogTraceStarted( CreateMethodCallInfo( productsBriefInfo, markForLog ) );

						var temp = await privateClient.catalogInventoryStockItemUpdateAsync( sessionId, putStockItem.ProductId, catalogInventoryStockItemUpdateEntity ).ConfigureAwait( false );

						res = temp.result > 0;

						var updateBriefInfo = string.Format( "{{Success:{0}}}", res );
						MagentoLogger.LogTraceEnded( CreateMethodCallInfo( productsBriefInfo, markForLog, methodResult : updateBriefInfo ) );
					}
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during PutStockItemsAsync({0})", productsBriefInfo ), exc );
			}
		}

		public virtual async Task< OrderInfoResponse > GetOrderAsync( string incrementId )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 300000;

				var res = new salesOrderInfoResponse();

				var privateClient = await this.CreateMagentoServiceClientAsync( this.BaseMagentoUrl ).ConfigureAwait( false );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.salesOrderInfoAsync( sessionId, incrementId ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new OrderInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrderAsync(incrementId:{0})", incrementId ), exc );
			}
		}

		public virtual async Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new magentoInfoResponse();
				var privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.magentoInfoAsync( sessionId ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new GetMagentoInfoResponse( res );
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
				var res0 = await this._magentoSoapService.catalogCategoryAttributeCurrentStoreAsync( sessionId, storeId ).ConfigureAwait( false );

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

				var res = await this._magentoSoapService.catalogProductCreateAsync( sessionId, "simple", "4", sku, catalogProductCreateEntity, storeId ).ConfigureAwait( false );

				//product id
				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during CreateProduct({0})", storeId ), exc );
			}
		}

		public async Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );
				var res = await this._magentoSoapService.catalogCategoryRemoveProductAsync( sessionId, categoryId, productId, identiferType ).ConfigureAwait( false );

				//product id
				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during DeleteProduct({0})", storeId ), exc );
			}
		}
		#endregion
	}
}