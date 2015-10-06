using System.Collections.Generic;
using System.Linq;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.SOAP.GetProducts
{
	internal class SoapGetProductsResponse
	{
		public SoapGetProductsResponse( catalogProductListResponse res )
		{
			this.Products = res.result.Select( x => new SoapProduct( x ) );
		}

		public IEnumerable<SoapProduct> Products { get; set; }
	}

	internal class SoapProduct
	{
		public SoapProduct( catalogProductEntity catalogProductEntity )
		{
			CategoryIds = catalogProductEntity.category_ids.ToList();
			Name = catalogProductEntity.name;
			ProductId = catalogProductEntity.product_id;
			Set = catalogProductEntity.set;
			Sku = catalogProductEntity.sku;
			this.Type = catalogProductEntity.type;
			WebsiteIds = catalogProductEntity.website_ids.ToList();
		}

		public string Type { get; set; }

		public List< string > WebsiteIds { get; set; }

		public string Sku { get; set; }

		public string Set { get; set; }

		public string ProductId { get; set; }

		public string Name { get; set; }

		public List< string > CategoryIds { get; set; }
	}
}