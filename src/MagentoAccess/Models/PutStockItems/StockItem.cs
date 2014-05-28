namespace MagentoAccess.Models.PutStockItems
{
	public class StockItem
	{
		public string ItemId { get; set; }
		public string ProductId { get; set; }
		public string StockId { get; set; }
		public long Qty { get; set; }
		public long MinQty { get; set; }
	}
}