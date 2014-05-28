using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutInventory;

namespace MagentoAccess
{
	public interface IMagentoService
	{
		Task< IEnumerable< Order2 > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo );

		Task< IEnumerable< Order2 > > GetOrdersAsync();

		Task UpdateInventoryAsync( IEnumerable< Inventory2 > products );

		Task< IEnumerable< Product2 > > GetProductsSimpleAsync();

		Task< IEnumerable< Product2 > > GetProductsAsync();
	}
}