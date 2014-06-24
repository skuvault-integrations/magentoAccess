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
				loginResponse getSessionIdTask;

				getSessionIdTask = await this._magentoSoapService.loginAsync( this.ApiUser, this.ApiKey ).ConfigureAwait( false );

				this._sessionIdCreatedAt = DateTime.UtcNow;
				return this._sessionId = getSessionIdTask.result;
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
			var endPoint = new List< string > { baseMagentoUrl, SoapApiUrl }.BuildUrl();
			this._magentoSoapService = new Mage_Api_Model_Server_Wsi_HandlerPortTypeClient( new BasicHttpBinding() { MaxReceivedMessageSize = Int32.MaxValue }, new EndpointAddress( endPoint ) );
		}

		public async Task< salesOrderListResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				filters filters;

				//if (string.IsNullOrWhiteSpace(this.Store))
				//	filters = new filters { complex_filter = new complexFilter[2] };
				//else
				//{
				//	filters = new filters { complex_filter = new complexFilter[3] };
				//	filters.complex_filter[2] = new complexFilter() { key = "store_id", value = new associativeEntity() { key = "in", value = this.Store } };
				//}

				//filters.complex_filter[0] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "from", value = modifiedFrom.ToSoapParameterString() } };
				//filters.complex_filter[1] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "to", value = modifiedTo.ToSoapParameterString() } };

				////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				/// 
				//if (string.IsNullOrWhiteSpace(this.Store))
				//	filters = new filters { filter = new associativeEntity[1] };
				//else
				//{
				//	filters = new filters { filter = new associativeEntity[2] };
				//	filters.filter[1] = new associativeEntity() { key = "store_id", value = this.Store };
				//}

				//filters.filter[0] = new associativeEntity() { key = "updated_at", value = modifiedFrom.ToSoapParameterString() };

				////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

				//if (string.IsNullOrWhiteSpace(this.Store))
				//	filters = new filters { complex_filter = new complexFilter[2] };
				//else
				//{
				//	filters = new filters { complex_filter = new complexFilter[3] };
				//	filters.complex_filter[2] = new complexFilter() { key = "store_id", value = new associativeEntity() { key = "in", value = this.Store } };
				//}

				//filters.complex_filter[0] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "qteq", value = modifiedFrom.ToSoapParameterString() } };
				//filters.complex_filter[1] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "lteq", value = modifiedTo.ToSoapParameterString() } };

				////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

				if (string.IsNullOrWhiteSpace(this.Store))
					filters = new filters { complex_filter = new complexFilter[1] };
				else
				{
					filters = new filters { complex_filter = new complexFilter[2] };
					filters.complex_filter[1] = new complexFilter() { key = "store_id", value = new associativeEntity() { key = "in", value = this.Store } };
				}

				filters.complex_filter[0] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "from", value = modifiedFrom.ToSoapParameterString() } };

				var res = await this._magentoSoapService.salesOrderListAsync( sessionId, filters ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrdersAsync(modifiedFrom:{0},modifiedTo{1})", modifiedFrom, modifiedTo ), exc );
			}
		}

		public async Task< salesOrderListResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			try
			{
				var ordersIdsAgregated = ordersIds.Aggregate( ( ac, x ) => ac += "," + x );

				var sessionId = await this.GetSessionId().ConfigureAwait( false );

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

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrdersAsync(...)" ), exc );
			}
		}

		public async Task< catalogProductListResponse > GetProductsAsync()
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var filters = new filters { filter = new associativeEntity[ 0 ] };

				var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;

				var res = await this._magentoSoapService.catalogProductListAsync( sessionId, filters, store ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		public async Task< catalogInventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var skusArray = skusOrIds.ToArray();

				var res = await this._magentoSoapService.catalogInventoryStockItemListAsync( sessionId, skusArray ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetStockItemsAsync(...)" ), exc );
			}
		}

		public async Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var res = await this._magentoSoapService.catalogInventoryStockItemMultiUpdateAsync( sessionId, stockItems.Select( x => x.Id ).ToArray(), stockItems.Select( x => x.UpdateEntity ).ToArray() ).ConfigureAwait( false );

				return res.result;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during PutStockItemsAsync(...)" ), exc );
			}
		}

		public async Task< salesOrderInfoResponse > GetOrderAsync( string incrementId )
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var res = await this._magentoSoapService.salesOrderInfoAsync( sessionId, incrementId ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrderAsync(incrementId:{0})", incrementId ), exc );
			}
		}

		public async Task< magentoInfoResponse > GetMagentoInfoAsync()
		{
			try
			{
				var sessionId = await this.GetSessionId().ConfigureAwait( false );

				var res = await this._magentoSoapService.magentoInfoAsync( sessionId ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetMagentoInfoAsync()" ), exc );
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