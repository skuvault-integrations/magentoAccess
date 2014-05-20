using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProduct;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.GetSrockItems;
using MagentoAccess.Models.PutStockItems;

namespace MagentoAccess.Services
{
	public interface IMagentoServiceLowLevel
	{
		string AccessToken { get; }

		string AccessTokenSecret { get; }

		Task PopulateAccessToken();

		GetProductResponse GetProduct( string id );

		GetProductsResponse GetProducts();

		GetStockItemsResponse GetInventory();

		PutStockItemsResponse PutInventory( IEnumerable< InventoryItem > inventoryItems );

		GetOrdersResponse GetOrders();

		GetOrdersResponse GetOrders( DateTime dateFrom, DateTime dateTo );
	}
}