using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MagentoAccess.Magento2catalogProductRepositoryV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProducts
{
	internal class SoapGetProductsResponse
	{
		public SoapGetProductsResponse( catalogProductListResponse res )
		{
			this.Products = res.result.Select( x => new SoapProduct( x ) );
		}

		public SoapGetProductsResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductListResponse res )
		{
			this.Products = res.result.Select( x => new SoapProduct( x ) );
		}

		public SoapGetProductsResponse( List< CatalogDataProductInterface > res )
		{
			this.Products = res.Select( x => new SoapProduct( x ) );
		}

		public SoapGetProductsResponse( List< Magento2catalogProductRepositoryV1_v_2_1_0_0_CE.CatalogDataProductInterface > res )
		{
			this.Products = res.Select(x => new SoapProduct(x));
		}

		public IEnumerable< SoapProduct > Products{ get; set; }
	}

	internal class SoapProduct
	{
		public SoapProduct( catalogProductEntity catalogProductEntity )
		{
			this.CategoryIds = catalogProductEntity.category_ids.ToList();
			this.Name = catalogProductEntity.name;
			this.ProductId = catalogProductEntity.product_id;
			this.Set = catalogProductEntity.set;
			this.Sku = catalogProductEntity.sku;
			this.Type = catalogProductEntity.type;
			this.WebsiteIds = catalogProductEntity.website_ids.ToList();
		}

		public SoapProduct( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductEntity catalogProductEntity )
		{
			this.CategoryIds = catalogProductEntity.category_ids.ToList();
			this.Name = catalogProductEntity.name;
			this.ProductId = catalogProductEntity.product_id;
			this.Set = catalogProductEntity.set;
			this.Sku = catalogProductEntity.sku;
			this.Type = catalogProductEntity.type;
			this.WebsiteIds = catalogProductEntity.website_ids.ToList();
		}

		public SoapProduct( CatalogDataProductInterface catalogProductEntity )
		{
			this.Name = catalogProductEntity.name;
			this.ProductId = catalogProductEntity.id.ToString( CultureInfo.InvariantCulture );
			this.Sku = catalogProductEntity.sku;
			this.Type = catalogProductEntity.typeId;
		}

		public SoapProduct( Magento2catalogProductRepositoryV1_v_2_1_0_0_CE.CatalogDataProductInterface catalogProductEntity )
		{
			this.Name = catalogProductEntity.name;
			this.ProductId = catalogProductEntity.id.ToString( CultureInfo.InvariantCulture );
			this.Sku = catalogProductEntity.sku;
			this.Type = catalogProductEntity.typeId;
		}

		public string Type{ get; set; }

		public List< string > WebsiteIds{ get; set; }

		public string Sku{ get; set; }

		public string Set{ get; set; }

		public string ProductId{ get; set; }

		public string Name{ get; set; }

		public List< string > CategoryIds{ get; set; }
	}
}