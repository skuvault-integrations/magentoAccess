using System.Collections.Generic;

namespace MagentoAccess.Models.PutInventory
{
	public class PutInventoryResponse
	{
		public List< PutStockResponseItem > Items { get; set; }
	}
}