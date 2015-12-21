using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProductInfo
{
	internal class CatalogProductInfoResponse
	{
		public CatalogProductInfoResponse( catalogProductInfoResponse res )
		{
			Description = res.result.description;
			ShortDescription = res.result.short_description;
			Price = res.result.price;
			Weight = res.result.weight;
		}

		public string Weight { get; set; }

		public string Price { get; set; }

		public string ShortDescription { get; set; }

		public string Description { get; set; }
	}
}