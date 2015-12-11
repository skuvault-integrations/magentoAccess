namespace MagentoAccess.Models.CreateProducts
{
	public class CreateProductModelResult
	{
		public int Result { get; set; }

		public string StoreId { get; set; }

		public string Sku { get; set; }

		public string Name { get; set; }

		public int IsInStock { get; set; }

		public CreateProductModelResult( CreateProductModel createProductModel )
		{
			IsInStock = createProductModel.IsInStock;
			Name = createProductModel.Name;
			Sku = createProductModel.Sku;
			StoreId = createProductModel.StoreId;
		}
	}
}