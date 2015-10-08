namespace MagentoAccess.Models.Services.Rest.GetProduct
{
	public class StockData
	{
		public decimal Qty { get; set; }
		public decimal MinQty { get; set; }
		public decimal MinSaleQty { get; set; }
		public decimal MaxSaleQty { get; set; }
		public decimal IsInStock { get; set; }
	}
}