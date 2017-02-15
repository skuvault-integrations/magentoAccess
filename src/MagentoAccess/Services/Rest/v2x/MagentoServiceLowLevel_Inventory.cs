using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetSessionId;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccess.Services.Soap;
using Netco.ActionPolicyServices;
using Netco.Extensions;
using MagentoUrl = MagentoAccess.Services.Rest.v2x.WebRequester.MagentoUrl;

namespace MagentoAccess.Services.Rest.v2x
{
	internal partial class MagentoServiceLowLevel : IMagentoServiceLowLevelSoap
	{
		public async Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds, IEnumerable< int > scopes, Mark mark = null )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				var products = await this.CatalogStockItemRepository.GetStockItemsAsync( skusOrIds, mark ).ConfigureAwait( false );
				var inventoryStockItems = Enumerable.ToList( products.Select( Mapper.Map< InventoryStockItem > ) );
				return new InventoryStockItemListResponse( inventoryStockItems );
			} );
		}

		public async Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, Mark mark = null )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				var products = await this.CatalogStockItemRepository.PutStockItemsAsync(
					stockItems.Select( x => Tuple.Create( x.Sku, x.ItemId, new RootObject() { stockItem = new StockItem { qty = x.Qty, minQty = x.MinQty } } ) ),
					mark ).ConfigureAwait( false );
				return products.All( x => x );
			} );
		}

		public Task< bool > PutStockItemAsync( PutStockItem putStockItem, Mark markForLog )
		{
			return null;
		}

		public Task< InventoryStockItemListResponse > GetStockItemsWithoutSkuAsync( IEnumerable< string > skusOrIds, IEnumerable< int > scopes, Mark mark = null )
		{
			throw new NotImplementedException();
		}
	}
}