using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface ICatalogStockItemRepository
	{
		Task< bool > PutStockItemAsync( string productSku, string itemId, RootObject stockItem );
		Task< IEnumerable< bool > > PutStockItemsAsync( IEnumerable< Tuple< string, string, RootObject > > items );
		Task< StockItem > GetStockItemAsync( string productSku );
		Task< IEnumerable< StockItem > > GetStockItemsAsync( IEnumerable< string > productSku );
	}
}