using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository;
using Netco.Logging;
using RootObject = MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository.RootObject;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface ISalesOrderRepositoryV1
	{
		Task< RootObject > GetOrdersAsync( IEnumerable< string > ids, PagingModel pagingModel, CancellationToken cancellationToken, string searchField = "increment_id" );
		Task< RootObject > GetOrdersAsync( DateTime updatedFrom, DateTime updatedTo, PagingModel pagingModel, CancellationToken cancellationToken, Mark mark = null );
		DateTime LastNetworkActivityTime { get; }
		Task< ShipmentsResponse > GetOrdersShipmentsAsync( DateTime updatedFrom, DateTime updatedTo, PagingModel page, CancellationToken cancellationToken, Mark mark = null );
	}
}