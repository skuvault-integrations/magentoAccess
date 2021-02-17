using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using Netco.Logging;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface ICatalogStockItemRepository
	{
		Task< bool > PutStockItemAsync( string productSku, string itemId, RootObject stockItem, Mark mark = null );
		Task< IEnumerable< bool > > PutStockItemsAsync( IEnumerable< Tuple< string, string, RootObject > > items, Mark mark = null );
		Task< StockItem > GetStockItemAsync( string productSku, Mark mark = null );
		Task< IEnumerable< StockItem > > GetStockItemsAsync( IEnumerable< string > productSku, Mark mark = null );
	}
}