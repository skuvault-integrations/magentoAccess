namespace MagentoAccess.Models.DeleteProducts
{
	public class DeleteProductModel
	{
		public DeleteProductModel( string store, int category, string productId, string identifertype )
		{
			StoreId = store;
			CategoryId = category;
			ProductId = productId;
			IdentiferType = identifertype;
		}

		public string StoreId { get; set; }
		public int CategoryId { get; set; }
		public string ProductId { get; set; }
		public string IdentiferType { get; set; }
	}
}