using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using RootObject = MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository.RootObject;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface ISalesOrderRepositoryV1
	{
		Task< RootObject > GetOrdersAsync( IEnumerable< string > productSku, PagingModel pagingModel );
		Task< RootObject > GetOrdersAsync( DateTime updatedFrom, DateTime updatedTo, PagingModel pagingModel );
	}
}