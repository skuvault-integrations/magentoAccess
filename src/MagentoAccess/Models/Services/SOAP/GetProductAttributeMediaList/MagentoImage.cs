using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList
{
	internal class MagentoImage
	{
		public MagentoImage( catalogProductImageEntity catalogProductImageEntity )
		{
			ImageUrl = catalogProductImageEntity.url;
		}

		public MagentoImage( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductImageEntity catalogProductImageEntity )
		{
			ImageUrl = catalogProductImageEntity.url;
		}

		public string ImageUrl { get; set; }
	}
}