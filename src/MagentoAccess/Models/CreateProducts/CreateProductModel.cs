namespace MagentoAccess.Models.CreateProducts
{
	public class CreateProductModel
	{
		public CreateProductModel( string storeId, string sku, string name, int isInStock )
		{
			StoreId = storeId;
			Name = name;
			Sku = sku;
			IsInStock = isInStock;
		}

		public string StoreId { get; set; }
		public string Name { get; set; }

		public string Sku { get; set; }

		public int IsInStock { get; set; }
	}
}