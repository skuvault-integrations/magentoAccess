namespace MagentoAccess.Models.GetProducts
{
	internal class CatalogProductInfoRequest
	{
		public CatalogProductInfoRequest( string[] custAttributes, string sku, string productId )
		{
			this.custAttributes = custAttributes;
			this.Sku = sku;
			this.ProductId = productId;
		}

		public string ProductId { get; private set; }
		public string Sku { get; private set; }
		public string[] custAttributes { get; private set; }
	}
}