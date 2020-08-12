using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Services.Soap;
using Netco.Extensions;
using Netco.Logging;
using MagentoAccess.Models.GetShipments;

namespace MagentoAccess.Services.Rest.v2x
{
	internal partial class MagentoServiceLowLevel : IMagentoServiceLowLevelSoap
	{
		private const int ShipmentsPerPage = 100;

		public async Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo, Mark mark = null )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				const int itemsPerPage = 100;
				var page = new PagingModel( itemsPerPage, 1 );
				var sale = await this.SalesOrderRepository.GetOrdersAsync( modifiedFrom, modifiedTo, page, mark ).ConfigureAwait( false );
				var pages = page.GetPages( sale.total_count ).Select( x => new PagingModel( itemsPerPage, x ) );
				var sales = await pages.ProcessInBatchAsync( 4, async x => await this.SalesOrderRepository.GetOrdersAsync( modifiedFrom, modifiedTo, x, mark ).ConfigureAwait( false ) ).ConfigureAwait( false );

				var result = sales.ToList();
				result.Add( sale );

				return new GetOrdersResponse( result.ToArray() );
			} );
		}

		public async Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				const int itemsPerPage = 100;
				var page = new PagingModel( itemsPerPage, 1 );
				var pagesData = page.GetPages( ordersIds );
				var pages = pagesData.Select( ( x, i ) => Tuple.Create( new PagingModel( itemsPerPage, i ), x ) );
				var sales = await pages.ProcessInBatchAsync( 4, async x => await this.SalesOrderRepository.GetOrdersAsync( x.Item2, x.Item1 ).ConfigureAwait( false ) ).ConfigureAwait( false );
				var result = Enumerable.ToList( sales );

				return new GetOrdersResponse( result.ToArray() );
			} );
		}

		public async Task< OrderInfoResponse > GetOrderAsync( string incrementId, Mark childMark )
		{
			var orderAsync = await this.SalesOrderRepository.GetOrdersAsync( new List< string > { incrementId }, new PagingModel( 1, 1 ) ).ConfigureAwait( false );
			return new OrderInfoResponse( orderAsync.items.FirstOrDefault() );
		}

		public Task< OrderInfoResponse > GetOrderAsync( Order order, Mark childMark )
		{
			return this.GetOrderAsync( this.GetOrdersUsesEntityInsteadOfIncrementId ? order.OrderId : order.incrementId, childMark );
		}

		public async Task< Dictionary< string, IEnumerable< Shipment > > > GetOrdersShipmentsAsync( DateTime modifiedFrom, DateTime modifiedTo, Mark mark = null )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				var result = new Dictionary< string, IEnumerable< Shipment > >();
				var page = new PagingModel( ShipmentsPerPage, 1 );
				var shipmentsFirstPage = await this.SalesOrderRepository.GetOrdersShipmentsAsync( modifiedFrom, modifiedTo, page, mark ).ConfigureAwait( false );

				var pages = page.GetPages( shipmentsFirstPage.TotalCount ).Select( pageIndex => new PagingModel( ShipmentsPerPage, pageIndex ) );
				var shipmentsPages = await pages.ProcessInBatchAsync( 4, async pageIndex => await this.SalesOrderRepository.GetOrdersShipmentsAsync( modifiedFrom, modifiedTo, pageIndex, mark ).ConfigureAwait( false ) ).ConfigureAwait( false );

				var shipments = shipmentsPages.Where( sp => sp.Items != null ).SelectMany( sp => sp.Items ).ToList();
				shipments.AddRange( shipmentsFirstPage.Items );

				return shipments.Select( s => new Shipment( s ) ).GroupBy( shipment => shipment.OrderId.ToString() ).ToDictionary( gr => gr.Key, gr => gr.AsEnumerable() );
			} );
		}

		public Task< string > CreateOrder( int shoppingcartid, string store )
		{
			return null;
		}

		public Task< int > CreateCart( string storeid )
		{
			return null;
		}
	}
}