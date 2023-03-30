using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services.Soap;
using Netco.Logging;

namespace MagentoAccess.Services.Rest.v2x
{
	internal sealed partial class MagentoServiceLowLevel : IMagentoServiceLowLevelSoap
	{
		public async Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds, IEnumerable< int > scopes, CancellationToken cancellationToken, Mark mark = null )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				var products = await this.CatalogStockItemRepository.GetStockItemsAsync( skusOrIds, cancellationToken, mark ).ConfigureAwait( false );
				var inventoryStockItems = products.Where( p => p.qty != null ).Select( Mapper.Map< InventoryStockItem > ).ToList();
				return new InventoryStockItemListResponse( inventoryStockItems );
			} );
		}

		public async Task< IEnumerable< RpcInvoker.RpcRequestResponse< PutStockItem, object > > > PutStockItemsAsync( List< PutStockItem > stockItems, CancellationToken cancellationToken, Mark mark = null )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				var products = await this.CatalogStockItemRepository.PutStockItemsAsync(
					stockItems.Select( x => Tuple.Create( x.Sku, x.ItemId, new RootObject() { stockItem = new StockItem { qty = x.Qty, minQty = x.MinQty, isInStock = x.Qty > 0 } } ) ),
					cancellationToken,
					mark ).ConfigureAwait( false );
				return products.Select( x => new RpcInvoker.RpcRequestResponse< PutStockItem, object >( ( PutStockItem )null, new RpcInvoker.RpcResponse< object >( RpcInvoker.SoapErrorCode.Success, x, null ) ) );
			} );
		}

		public Task< bool > PutStockItemAsync( PutStockItem putStockItem, CancellationToken cancellationToken, Mark markForLog )
		{
			return null;
		}

		public Task< InventoryStockItemListResponse > GetStockItemsWithoutSkuAsync( IEnumerable< string > skusOrIds, IEnumerable< int > scopes, CancellationToken cancellationToken, Mark mark = null )
		{
			throw new NotImplementedException();
		}
	}
}