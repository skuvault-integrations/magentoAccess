using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Magento2catalogProductAttributeMediaGalleryManagementV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Models.Services.Soap.GetProductInfo;

namespace MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList
{
	internal class ProductAttributeMediaListResponse: ResponseWithExceptions
	{
		public ProductAttributeMediaListResponse( catalogProductAttributeMediaListResponse res, string productId, string sku )
		{
			this.ProductId = productId;
			this.Sku = sku;
			this.MagentoImages = res.result.Select( x => new MagentoImage( x ) ).ToList();
		}

		public ProductAttributeMediaListResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductAttributeMediaListResponse res, string productId, string sku )
		{
			this.ProductId = productId;
			this.Sku = sku;
			this.MagentoImages = res.result.Select( x => new MagentoImage( x ) ).ToList();
		}

		public ProductAttributeMediaListResponse( catalogProductAttributeMediaGalleryManagementV1GetListResponse1 res, string productId, string sku )
		{
			this.ProductId = productId;
			this.Sku = sku;
			this.MagentoImages = res.catalogProductAttributeMediaGalleryManagementV1GetListResponse.result.Select( x => new MagentoImage( x ) ).ToList();
		}

		public ProductAttributeMediaListResponse( Exception exc )
		{
			this.Exc = exc;
		}

		public string ProductId{ get; private set; }
		public string Sku{ get; private set; }

		public List< MagentoImage > MagentoImages{ get; private set; }
	}
}