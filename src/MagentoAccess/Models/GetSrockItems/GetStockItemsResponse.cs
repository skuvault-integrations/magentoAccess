using System.Collections.Generic;

namespace MagentoAccess.Models.GetSrockItems
{
	public class GetStockItemsResponse
	{
		public IEnumerable< StockItem > Items { get; set; }
	}
}