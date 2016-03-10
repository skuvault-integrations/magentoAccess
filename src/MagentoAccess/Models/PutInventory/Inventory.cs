namespace MagentoAccess.Models.PutInventory
{
	public class Inventory
	{
		public string ItemId{ get; set; }
		public string ProductId{ get; set; }
		public string StockId{ get; set; }
		public long Qty{ get; set; }
		public long MinQty{ get; set; }

		//used in Magento2
		public string Sku{ get; set; }
	}

	public class InventoryBySku
	{
		public string Sku{ get; set; }
		public string StockId{ get; set; }
		public long Qty{ get; set; }
		public long MinQty{ get; set; }
	}
}