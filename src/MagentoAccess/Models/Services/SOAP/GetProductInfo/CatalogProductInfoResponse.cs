using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProductInfo
{
	internal class CatalogProductInfoResponse
	{
		public CatalogProductInfoResponse( catalogProductInfoResponse catalogProductInfoResponse )
		{
			Description = catalogProductInfoResponse.result.description;
			ShortDescription = catalogProductInfoResponse.result.short_description;
			Price = catalogProductInfoResponse.result.price;
			Weight = catalogProductInfoResponse.result.weight;
			ProductId = catalogProductInfoResponse.result.product_id;
		}

		public CatalogProductInfoResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductInfoResponse catalogProductInfoResponse )
		{
			Description = catalogProductInfoResponse.result.description;
			ShortDescription = catalogProductInfoResponse.result.short_description;
			Price = catalogProductInfoResponse.result.price;
			Weight = catalogProductInfoResponse.result.weight;
			ProductId = catalogProductInfoResponse.result.product_id;
		}

		public string ProductId { get; set; }

		public string Weight { get; set; }

		public string Price { get; set; }

		public string ShortDescription { get; set; }

		public string Description { get; set; }
	}
}