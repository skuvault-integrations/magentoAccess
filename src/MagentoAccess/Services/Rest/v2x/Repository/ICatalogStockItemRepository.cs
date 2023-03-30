using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using Netco.Logging;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface ICatalogStockItemRepository
	{
		Task< bool > PutStockItemAsync( string productSku, string itemId, RootObject stockItem, CancellationToken cancellationToken, Mark mark = null );
		Task< IEnumerable< bool > > PutStockItemsAsync( IEnumerable< Tuple< string, string, RootObject > > items, CancellationToken cancellationToken, Mark mark = null );
		Task< StockItem > GetStockItemAsync( string productSku, CancellationToken cancellationToken, Mark mark = null );
		Task< IEnumerable< StockItem > > GetStockItemsAsync( IEnumerable< string > productSku, CancellationToken cancellationToken, Mark mark = null );
		DateTime LastNetworkActivityTime { get; }
	}
}