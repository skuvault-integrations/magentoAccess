using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository;
using Netco.Logging;
using RootObject = MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository.RootObject;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface ISalesOrderRepositoryV1
	{
		Task< RootObject > GetOrdersAsync( IEnumerable< string > ids, PagingModel pagingModel );
		Task< RootObject > GetOrdersAsync( DateTime updatedFrom, DateTime updatedTo, PagingModel pagingModel, Mark mark = null );
		Task< ShipmentsResponse > GetOrdersShipmentsAsync( DateTime updatedFrom, DateTime updatedTo, PagingModel page, Mark mark = null );
	}
}