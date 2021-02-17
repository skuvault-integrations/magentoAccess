using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.Products;
using Netco.Logging;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface IProductRepository
	{
		Task< RootObject > GetProductsAsync( PagingModel page );
		Task< List< RootObject > > GetProductsAsync();
		Task< RootObject > GetProductsAsync( DateTime updatedAt, PagingModel page, Mark mark = null );
		Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, PagingModel page );
		Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, PagingModel page, Mark mark = null );
		Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, Mark mark = null );
		Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type );
		Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, Mark mark = null );
		Task< IEnumerable< RootObject > > GetProductsBySkusAsync( IEnumerable< string > skus, Mark mark );
		Task< Item > GetProductAsync( string sku );
		Task< CategoryNode > GetCategoriesTreeAsync();
		Task< ProductAttribute > GetManufacturersAsync();
	}
}