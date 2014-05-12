using System.Collections.Generic;
using MagentoAccess.Models.BaseResponse;

namespace MagentoAccess.Models.GetOrders
{
	public class GetOrdersResponse
	{
		public List< Order > Orders { get; set; }
	}
}