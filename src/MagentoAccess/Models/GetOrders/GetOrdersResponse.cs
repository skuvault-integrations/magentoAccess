using System.Collections.Generic;
using MagentoAccess.Services.Parsers;

namespace MagentoAccess.Models.GetOrders
{
	public class GetOrdersResponse : Order
	{
		public List< Order > Orders { get; set; }
		public ResponseError Error { get; set; }
	}
}