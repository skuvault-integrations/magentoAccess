using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using Netco.Logging;

namespace MagentoAccess.Services
{
	internal class MagentoServiceLowLevelSoap : IMagentoServiceLowLevelSoap
	{
		public string UserName { get; set; }

		public string Password { get; set; }

		public string Store { get; set; }

		protected const string SoapApiUrl = "index.php/api/v2_soap/index/";

		protected readonly Mage_Api_Model_Server_Wsi_HandlerPortTypeClient _magentoSoapService;

		protected string _sessionId;

		protected DateTime _sessionIdCreatedAt;

		protected const int SessionIdLifeTime = 3590;

		private void LogTraceGetResponseException( FaultException exception )
		{
			this.Log().Trace( "[magento] SOAP action:{0}, fault code:{1}, throw an exception.", exception.Action, exception.Code, exception );
		}

		internal async Task< string > GetSessionId()
		{
			try
			{
				if( string.IsNullOrWhiteSpace( this._sessionId ) && DateTime.UtcNow.Subtract( this._sessionIdCreatedAt ).TotalSeconds < SessionIdLifeTime )
					return this._sessionId;

				var getSessionIdTask = await this._magentoSoapService.loginAsync( this.UserName, this.Password ).ConfigureAwait( false );
				this._sessionIdCreatedAt = DateTime.UtcNow;
				return this._sessionId = getSessionIdTask.result;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return null;
			}
		}

		public MagentoServiceLowLevelSoap( string userName, string password, string baseMagentoUrl, string store )
		{
			this.UserName = userName;
			this.Password = password;
			this.Store = store;
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl }.BuildUrl();
			this._magentoSoapService = new Mage_Api_Model_Server_Wsi_HandlerPortTypeClient( new BasicHttpBinding() { MaxReceivedMessageSize = Int32.MaxValue }, new EndpointAddress( endPoint ) );
		}

		public async Task< salesOrderListResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return new salesOrderListResponse();

				filters filters;

				if( string.IsNullOrWhiteSpace( this.Store ) )
					filters = new filters { complex_filter = new complexFilter[ 2 ] };
				else
				{
					filters = new filters { complex_filter = new complexFilter[ 3 ] };
					filters.complex_filter[ 1 ] = new complexFilter() { key = "store_id", value = new associativeEntity() { key = "in", value = this.Store } };
				}

				filters.complex_filter[ 0 ] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "from", value = modifiedFrom.ToString() } };
				filters.complex_filter[ 1 ] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "to", value = modifiedTo.ToString() } };

				var res = await this._magentoSoapService.salesOrderListAsync( sessionId, filters ).ConfigureAwait( false );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new salesOrderListResponse();
			}
		}

		public async Task< salesOrderListResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			try
			{
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

				filters.complex_filter[ 0 ] = new complexFilter() { key = "increment_id", value = new associativeEntity() { key = "in", value = ordersIds.Aggregate( ( ac, x ) => ac += "," + x ) } };

				var res = await this._magentoSoapService.salesOrderListAsync( sessionId, filters ).ConfigureAwait( false );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new salesOrderListResponse();
			}
		}

		public async Task< catalogProductListResponse > GetProductsAsync()
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return new catalogProductListResponse();

				var filters = new filters { filter = new associativeEntity[ 0 ] };

				var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;

				var res = await this._magentoSoapService.catalogProductListAsync( sessionId, filters, store ).ConfigureAwait( false );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new catalogProductListResponse();
			}
		}

		public async Task< catalogInventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return new catalogInventoryStockItemListResponse();

				var skusArray = skusOrIds.ToArray();

				var res = await this._magentoSoapService.catalogInventoryStockItemListAsync( sessionId, skusArray ).ConfigureAwait( false );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new catalogInventoryStockItemListResponse();
			}
		}

		public async Task< salesOrderInfoResponse > GetOrderAsync( string incrementId )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				if( sessionId == null )
					return new salesOrderInfoResponse();

				var res = await this._magentoSoapService.salesOrderInfoAsync( sessionId, incrementId ).ConfigureAwait( false );

				return res;
			}
			catch( FaultException exception )
			{
				this.LogTraceGetResponseException( exception );
				return new salesOrderInfoResponse();
			}
		}
	}
}