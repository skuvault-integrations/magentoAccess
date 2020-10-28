using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.Products;
using Netco.Logging;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface IProductRepository
	{
		Task< RootObject > GetProductsAsync( PagingModel page, CancellationToken cancellationToken );
		Task< List< RootObject > > GetProductsAsync( CancellationToken cancellationToken );
		Task< RootObject > GetProductsAsync( DateTime updatedAt, PagingModel page, CancellationToken cancellationToken, Mark mark = null );
		Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, PagingModel page, CancellationToken cancellationToken );
		Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, PagingModel page, CancellationToken cancellationToken, Mark mark = null );
		Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, CancellationToken cancellationToken, Mark mark = null );
		Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type, CancellationToken cancellationToken );
		Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, CancellationToken cancellationToken, Mark mark = null );
		Task< IEnumerable< RootObject > > GetProductsBySkusAsync( IEnumerable< string > skus, CancellationToken cancellationToken, Mark mark );
		Task< Item > GetProductAsync( string sku, CancellationToken cancellationToken );
		Task< CategoryNode > GetCategoriesTreeAsync( CancellationToken cancellationToken );
		Task< ProductAttribute > GetManufacturersAsync( CancellationToken cancellationToken );
		DateTime LastNetworkActivityTime { get; }
	}
}