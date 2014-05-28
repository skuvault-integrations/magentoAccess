using System.Collections.Generic;

namespace MagentoAccess.Models.GetStockItems
{
	public class GetStockItemsResponse
	{
		public IEnumerable< StockItem > Items { get; set; }
	}
}