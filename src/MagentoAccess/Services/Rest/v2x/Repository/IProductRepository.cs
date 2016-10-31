using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.Rest.v2x.Products;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface IProductRepository
	{
		Task< RootObject > GetProductsAsync( PagingModel page );
		Task< List< RootObject > > GetProductsAsync();
		Task< RootObject > GetProductsAsync( DateTime updatedAt, PagingModel page );
		Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, PagingModel page );
		Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, PagingModel page );
		Task< List< RootObject > > GetProductsAsync( DateTime updatedAt );
		Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type );
		Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type, bool excludeType );
	}
}