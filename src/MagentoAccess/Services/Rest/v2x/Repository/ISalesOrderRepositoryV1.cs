using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface ISalesOrderRepositoryV1
	{
		Task< StockItem > GetStockItemAsync( string productSku );
		Task< IEnumerable< StockItem > > GetOrdersAsync( IEnumerable< string > productSku );
	}
}