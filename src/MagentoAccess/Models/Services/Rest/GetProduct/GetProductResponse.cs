namespace MagentoAccess.Models.Services.Rest.GetProduct
{
	public class GetProductResponse
	{
		public string EntityId { get; set; }
		public StockData StockData { get; set; }
		public string Sku { get; set; }
		public string Name { get; set; }
		public string Price { get; set; }
		public string Description { get; set; }
	}
}