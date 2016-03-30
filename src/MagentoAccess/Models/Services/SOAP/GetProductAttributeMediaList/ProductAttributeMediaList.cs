using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Magento2catalogProductAttributeMediaGalleryManagementV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList
{
	internal class ProductAttributeMediaListResponse
	{
		public ProductAttributeMediaListResponse( catalogProductAttributeMediaListResponse res, string productId )
		{
			ProductId = productId;
			MagentoImages = res.result.Select( x => new MagentoImage( x ) ).ToList();
		}

		public ProductAttributeMediaListResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductAttributeMediaListResponse res, string productId )
		{
			ProductId = productId;
			MagentoImages = res.result.Select( x => new MagentoImage( x ) ).ToList();
		}

		public ProductAttributeMediaListResponse( catalogProductAttributeMediaGalleryManagementV1GetListResponse1 res, string productId )
		{
			this.ProductId = productId;
			this.MagentoImages = res.catalogProductAttributeMediaGalleryManagementV1GetListResponse.result.Select( x => new MagentoImage( x ) ).ToList();
		}

		public string ProductId { get; set; }

		public List< MagentoImage > MagentoImages { get; set; }
	}
}