using System.Collections.Generic;

namespace MagentoAccess.Models.Services.GetStockItems
{
	public class GetStockItemsResponse
	{
		public IEnumerable< StockItem > Items { get; set; }
	}
}