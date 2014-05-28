using MagentoAccess.Models.Services.GetStockItems;

namespace MagentoAccess.Models.GetProducts
{
	public class Product
	{
		public string EntityId { get; set; }
		public string Sku { get; set; }
		public decimal Price { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public Product( Services.GetProducts.Product product )
		{
			this.EntityId = product.EntityId;
			this.Sku = product.Sku;
			this.Price = product.Price;
			this.Name = product.Name;
			this.Description = product.Description;
			this.EntityId = product.EntityId;
		}

		public Product( StockItem stockItem )
		{
			this.EntityId = stockItem.ItemId;
		}
	}
}