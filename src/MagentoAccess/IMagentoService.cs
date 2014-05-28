using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutStockItems;

namespace MagentoAccess
{
	public interface IMagentoService
	{
		Task< IEnumerable< Order > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo );

		Task< IEnumerable< Order > > GetOrdersAsync();

		Task UpdateProductsAsync( IEnumerable< StockItem > products );

		Task< IEnumerable< Product > > GetProductsSimpleAsync();

		Task< IEnumerable< Models.GetStockItems.StockItem > > GetProductsAsync();
	}
}