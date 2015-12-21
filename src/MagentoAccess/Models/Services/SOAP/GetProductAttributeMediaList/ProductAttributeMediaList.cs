using System.Collections.Generic;
using System.Linq;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList
{
	internal class ProductAttributeMediaListResponse
	{
		public ProductAttributeMediaListResponse( catalogProductAttributeMediaListResponse res )
		{
			MagentoImages = res.result.Select( x => new MagentoImage( x ) ).ToList();
		}

		public ProductAttributeMediaListResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductAttributeMediaListResponse res )
		{
			MagentoImages = res.result.Select( x => new MagentoImage( x ) ).ToList();
		}

		public List< MagentoImage > MagentoImages { get; set; }
	}
}