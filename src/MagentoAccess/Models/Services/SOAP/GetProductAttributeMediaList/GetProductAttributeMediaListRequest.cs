namespace MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList
{
	internal class GetProductAttributeMediaListRequest
	{
		public string ProductId{ get; private set; }
		public string Sku{ get; private set; }

		public GetProductAttributeMediaListRequest( string productId, string sku )
		{
			this.ProductId = productId;
			this.Sku = sku;
		}
	}
}