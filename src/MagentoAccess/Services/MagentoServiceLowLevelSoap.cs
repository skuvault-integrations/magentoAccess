using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

		protected const string SoapApiUrl = "index.php/api/v2_soap/index/";

		protected readonly Mage_Api_Model_Server_Wsi_HandlerPortTypeClient _magentoSoapService;

		protected string _sessionId;

		protected DateTime _sessionIdCreatedAt;

		protected const int SessionIdLifeTime = 3590;

		private void LogTraceGetResponseException( FaultException exception )
		{
			var faultName = PredefinedValues.NotAvailable;
			if( exception != null )
			{
				if( exception.Code != null )
					faultName = string.IsNullOrWhiteSpace( exception.Code.Name ) ? PredefinedValues.NotAvailable : exception.Code.Name;
			}

			var actionInfo = PredefinedValues.NotAvailable;
			if( exception != null )
				actionInfo = string.IsNullOrWhiteSpace( exception.Action ) ? PredefinedValues.NotAvailable : exception.Action;

			MagentoLogger.Log().Trace( exception, "[magento] SOAP action:{0}, fault code:{1}, throw an fault exception.", actionInfo, faultName );
		}

		private void LogTraceGetResponseException( ProtocolException exception )
		{
			MagentoLogger.Log().Trace( exception, "[magento] SOAP helplink:{0}, throw a protocol exception.", string.IsNullOrWhiteSpace( exception.HelpLink ) ? PredefinedValues.NotAvailable : exception.HelpLink );
		}

		private void LogTraceGetResponseException( Exception exception )
		{
			MagentoLogger.Log().Trace( exception, "[magento] SOAP throw an exception." );
		}

		private void LogTraceGetResponseAsyncStarted( string info )
		{
			MagentoLogger.Log().Trace( "[magento] SOAP Call:{0}, started.", info );
		}

		private void LogTraceGetResponseAsyncEnded( string info )
		{
			MagentoLogger.Log().Trace( "[magento] SOAP Call:{0}, ended.", info );
		}

		internal async Task< string > GetSessionId( bool throwException = false )
		{
			try
			{
				this.LogTraceGetResponseAsyncStarted( string.Format( "GetSessionId" ) );
				if( !string.IsNullOrWhiteSpace( this._sessionId ) && DateTime.UtcNow.Subtract( this._sessionIdCreatedAt ).TotalSeconds < SessionIdLifeTime )
					return this._sessionId;
				loginResponse getSessionIdTask;

				getSessionIdTask = await this._magentoSoapService.loginAsync( this.ApiUser, this.ApiKey ).ConfigureAwait( false );

				this._sessionIdCreatedAt = DateTime.UtcNow;
				this.LogTraceGetResponseAsyncEnded( string.Format( "GetSessionId" ) );
				return this._sessionId = getSessionIdTask.result;
			}
			catch( FaultException faultException )
			{
				if( throwException )
					throw;

				this.LogTraceGetResponseException( faultException );
				return null;
			}
			catch( ProtocolException protocolException )
			{
				if( throwException )
					throw;

				this.LogTraceGetResponseException( protocolException );
				return null;
			}
			catch( Exception exception )
			{
				if( throwException )
					throw;

				this.LogTraceGetResponseException( exception );
				return null;
			}
		}

		public MagentoServiceLowLevelSoap( string apiUser, string apiKey, string baseMagentoUrl, string store )
		{
			this.ApiUser = apiUser;
			this.ApiKey = apiKey;
			this.Store = store;
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl }.BuildUrl();
			this._magentoSoapService = new Mage_Api_Model_Server_Wsi_HandlerPortTypeClient( new BasicHttpBinding() { MaxReceivedMessageSize = Int32.MaxValue }, new EndpointAddress( endPoint ) );
		}

		public async Task< salesOrderListResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			try
			{
				this.LogTraceGetResponseAsyncStarted( string.Format( "GetOrdersAsync({0},{1})", modifiedFrom, modifiedTo ) );

				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return new salesOrderListResponse();

				filters filters;

				if( string.IsNullOrWhiteSpace( this.Store ) )
					filters = new filters { complex_filter = new complexFilter[ 2 ] };
				else
				{
					filters = new filters { complex_filter = new complexFilter[ 3 ] };
					filters.complex_filter[ 2 ] = new complexFilter() { key = "store_id", value = new associativeEntity() { key = "in", value = this.Store } };
				}

				filters.complex_filter[ 0 ] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "from", value = modifiedFrom.ToSoapParameterString() } };
				filters.complex_filter[ 1 ] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "to", value = modifiedTo.ToSoapParameterString() } };

				var res = await this._magentoSoapService.salesOrderListAsync( sessionId, filters ).ConfigureAwait( false );

				this.LogTraceGetResponseAsyncEnded( string.Format( "GetOrdersAsync({0},{1})", modifiedFrom, modifiedTo ) );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new salesOrderListResponse();
			}
			catch( ProtocolException protocolException )
			{
				this.LogTraceGetResponseException( protocolException );
				return null;
			}
			catch( Exception exception )
			{
				this.LogTraceGetResponseException( exception );
				return null;
			}
		}

		public async Task< salesOrderListResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			try
			{
				var ordersIdsAgregated = ordersIds.Aggregate( ( ac, x ) => ac += "," + x );

				this.LogTraceGetResponseAsyncStarted( string.Format( "GetOrdersAsync({0})", ordersIdsAgregated ) );

				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return new salesOrderListResponse();

				filters filters;
				if( string.IsNullOrWhiteSpace( this.Store ) )
					filters = new filters { complex_filter = new complexFilter[ 1 ] };
				else
				{
					filters = new filters { complex_filter = new complexFilter[ 2 ] };
					filters.complex_filter[ 1 ] = new complexFilter() { key = "store_id", value = new associativeEntity() { key = "in", value = this.Store } };
				}

				filters.complex_filter[ 0 ] = new complexFilter() { key = "increment_id", value = new associativeEntity() { key = "in", value = ordersIdsAgregated } };

				var res = await this._magentoSoapService.salesOrderListAsync( sessionId, filters ).ConfigureAwait( false );

				this.LogTraceGetResponseAsyncEnded( string.Format( "GetOrdersAsync({0})", ordersIdsAgregated ) );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new salesOrderListResponse();
			}
			catch( ProtocolException protocolException )
			{
				this.LogTraceGetResponseException( protocolException );
				return new salesOrderListResponse();
			}
			catch( Exception exception )
			{
				this.LogTraceGetResponseException( exception );
				return new salesOrderListResponse();
			}
		}

		public async Task< catalogProductListResponse > GetProductsAsync()
		{
			try
			{
				this.LogTraceGetResponseAsyncStarted( string.Format( "GetProductsAsync()" ) );

				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return new catalogProductListResponse();

				var filters = new filters { filter = new associativeEntity[ 0 ] };

				var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;

				var res = await this._magentoSoapService.catalogProductListAsync( sessionId, filters, store ).ConfigureAwait( false );

				this.LogTraceGetResponseAsyncEnded( string.Format( "GetProductsAsync()" ) );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new catalogProductListResponse();
			}
			catch( ProtocolException protocolException )
			{
				this.LogTraceGetResponseException( protocolException );
				return new catalogProductListResponse();
			}
			catch( Exception exception )
			{
				this.LogTraceGetResponseException( exception );
				return new catalogProductListResponse();
			}
		}

		public async Task< catalogInventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			try
			{
				this.LogTraceGetResponseAsyncStarted( string.Format( "GetStockItemsAsync({0})", skusOrIds.Aggregate( ( ac, x ) => ac += "," + x ) ) );

				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return new catalogInventoryStockItemListResponse();

				var skusArray = skusOrIds.ToArray();

				var res = await this._magentoSoapService.catalogInventoryStockItemListAsync( sessionId, skusArray ).ConfigureAwait( false );

				this.LogTraceGetResponseAsyncEnded( string.Format( "GetStockItemsAsync({0})", skusOrIds.Aggregate( ( ac, x ) => ac += "," + x ) ) );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new catalogInventoryStockItemListResponse();
			}
			catch( ProtocolException protocolException )
			{
				this.LogTraceGetResponseException( protocolException );
				return new catalogInventoryStockItemListResponse();
			}
			catch( Exception exception )
			{
				this.LogTraceGetResponseException( exception );
				return new catalogInventoryStockItemListResponse();
			}
		}

		public async Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems )
		{
			try
			{
				this.LogTraceGetResponseAsyncStarted( string.Format( "PutStockItemsAsync({0})", string.Join( ",", stockItems.Select( x => string.Format( "[id:{0},qty{1}]", x.Id, x.UpdateEntity.qty ) ) ) ) );

				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return false;

				var res = await this._magentoSoapService.catalogInventoryStockItemMultiUpdateAsync( sessionId, stockItems.Select( x => x.Id ).ToArray(), stockItems.Select( x => x.UpdateEntity ).ToArray() ).ConfigureAwait( false );

				this.LogTraceGetResponseAsyncEnded( string.Format( "GetStockItemsAsync({0})", res.result ) );

				return res.result;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return false;
			}
			catch( ProtocolException protocolException )
			{
				this.LogTraceGetResponseException( protocolException );
				return false;
			}
			catch( Exception exception )
			{
				this.LogTraceGetResponseException( exception );
				return false;
			}
		}

		public async Task< salesOrderInfoResponse > GetOrderAsync( string incrementId )
		{
			try
			{
				this.LogTraceGetResponseAsyncStarted( string.Format( "GetOrderAsync({0})", incrementId ) );

				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return new salesOrderInfoResponse();

				var res = await this._magentoSoapService.salesOrderInfoAsync( sessionId, incrementId ).ConfigureAwait( false );

				this.LogTraceGetResponseAsyncEnded( string.Format( "GetOrderAsync({0})", incrementId ) );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new salesOrderInfoResponse();
			}
			catch( ProtocolException protocolException )
			{
				this.LogTraceGetResponseException( protocolException );
				return new salesOrderInfoResponse();
			}
			catch( Exception exception )
			{
				this.LogTraceGetResponseException( exception );
				return new salesOrderInfoResponse();
			}
		}

		public async Task< magentoInfoResponse > GetMagentoInfoAsync()
		{
			this.LogTraceGetResponseAsyncStarted( "GetMagentoVersion()" );

			var sessionId = await this.GetSessionId( true ).ConfigureAwait( false );

			var res = await this._magentoSoapService.magentoInfoAsync( sessionId ).ConfigureAwait( false );

			this.LogTraceGetResponseAsyncEnded( "GetMagentoVersion()" );

			return res;
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