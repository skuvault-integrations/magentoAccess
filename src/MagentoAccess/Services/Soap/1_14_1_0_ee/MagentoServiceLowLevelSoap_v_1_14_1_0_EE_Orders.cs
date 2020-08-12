using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference_v_1_14_1_EE;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetOrders;
using Netco.Logging;
using MagentoAccess.Models.GetShipments;

namespace MagentoAccess.Services.Soap._1_14_1_0_ee
{
	internal partial class MagentoServiceLowLevelSoap_v_1_14_1_0_EE : IMagentoServiceLowLevelSoap
	{
		public virtual async Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo, Mark mark = null )
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

				var privateClient = this._clientFactory.GetClient();

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.salesOrderListAsync( sessionId.SessionId, filters ).ConfigureAwait( false );
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

				var privateClient = this._clientFactory.GetClient();

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.salesOrderListAsync( sessionId.SessionId, filters ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new GetOrdersResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrdersAsync({0})", ordersIdsAgregated ), exc );
			}
		}

		public virtual async Task< OrderInfoResponse > GetOrderAsync( string incrementId, Mark childMark )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 300000;

				var res = new salesOrderInfoResponse();

				var privateClient = this._clientFactory.GetClient();

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.salesOrderInfoAsync( sessionId.SessionId, incrementId ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new OrderInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrderAsync(incrementId:{0})", incrementId ), exc );
			}
		}

		public virtual Task< OrderInfoResponse > GetOrderAsync( Order order, Mark childMark )
		{
			return this.GetOrderAsync( this.GetOrdersUsesEntityInsteadOfIncrementId ? order.OrderId : order.incrementId, childMark );
		}

		public Task< Dictionary< string, IEnumerable< Shipment > > > GetOrdersShipmentsAsync( DateTime modifiedFrom, DateTime modifiedTo, Mark mark = null )
		{
			return Task.FromResult( new Dictionary< string, IEnumerable< Shipment > >() );
		}
	}
}