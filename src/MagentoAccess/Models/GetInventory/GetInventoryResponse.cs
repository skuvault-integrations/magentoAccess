using System.Collections.Generic;
using MagentoAccess.Models.GetOrders;

namespace MagentoAccess.Models.GetInventory
{
	public class GetInventoryResponse
	{
		public List< InventoryItem > Items { get; set; }
	}
}