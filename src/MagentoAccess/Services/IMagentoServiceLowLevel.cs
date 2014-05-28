using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.GetOrders;
using MagentoAccess.Models.Services.GetProduct;
using MagentoAccess.Models.Services.GetProducts;
using MagentoAccess.Models.Services.GetStockItems;
using MagentoAccess.Models.Services.PutStockItems;
using StockItem = MagentoAccess.Models.Services.PutStockItems.StockItem;

namespace MagentoAccess.Services
{
	public interface IMagentoServiceLowLevel
	{
		string AccessToken { get; }

		string AccessTokenSecret { get; }

		Task PopulateAccessToken();

		Task< GetProductResponse > GetProductAsync( string id );

		Task< GetProductsResponse > GetProductsAsync( int page, int limit );

		Task< GetStockItemsResponse > GetStockItemsAsync( int page, int limit );

		Task< PutStockItemsResponse > PutStockItemsAsync( IEnumerable< StockItem > inventoryItems );

		Task< GetOrdersResponse > GetOrdersAsync();

		Task< GetOrdersResponse > GetOrdersAsync( DateTime dateFrom, DateTime dateTo );
	}
}