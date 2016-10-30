using System.Collections.Generic;

namespace MagentoAccess.Models.Services.Rest.v1x.GetStockItems
{
	public class GetStockItemsResponse
	{
		public IEnumerable< StockItem > Items { get; set; }
	}
}