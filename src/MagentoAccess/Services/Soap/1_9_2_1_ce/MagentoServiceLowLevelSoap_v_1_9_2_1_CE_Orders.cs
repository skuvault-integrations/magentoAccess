using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetOrders;

namespace MagentoAccess.Services.Soap._1_9_2_1_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_1_9_2_1_ce : IMagentoServiceLowLevelSoap
	{
		public virtual async Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			var filters = new filters();
			AddFilter( filters, modifiedFrom.ToSoapParameterString(), "updated_at", "from" );
			AddFilter( filters, modifiedTo.ToSoapParameterString(), "updated_at", "to" );
			if( !string.IsNullOrWhiteSpace( this.Store ) )
				AddFilter( filters, this.Store, "store_id", "in" );

			return await this.GetWithAsync(
				res =>
				{
					//crutch for magento 1.7 
					res.result = res.result.Where( x => Extensions.ToDateTimeOrDefault( x.updated_at ) >= modifiedFrom && Extensions.ToDateTimeOrDefault( x.updated_at ) <= modifiedTo ).ToArray();
					return new GetOrdersResponse( res );
				},
				async ( client, session ) => await client.salesOrderListAsync( session, filters ).ConfigureAwait( false ), 600000 ).ConfigureAwait( false );
		}

		public virtual async Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			var ordersIdsAgregated = string.Join( ",", ordersIds );

			var filters = new filters();
			AddFilter( filters, ordersIdsAgregated, "increment_id", "in" );

			if( !string.IsNullOrWhiteSpace( this.Store ) )
				AddFilter( filters, this.Store, "store_id", "in" );

			return await this.GetWithAsync(
				res => new GetOrdersResponse( res ),
				async ( client, session ) => await client.salesOrderListAsync( session, filters ).ConfigureAwait( false ), 600000 ).ConfigureAwait( false );
		}

		public virtual async Task< OrderInfoResponse > GetOrderAsync( string incrementId )
		{
			return await this.GetWithAsync(
				res => new OrderInfoResponse( res ),
				async ( client, session ) => await client.salesOrderInfoAsync( session, incrementId ).ConfigureAwait( false ), 600000 ).ConfigureAwait( false );
		}
	}
}