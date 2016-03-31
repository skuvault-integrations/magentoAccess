using System;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;

namespace MagentoAccess.Models.Services.Soap.GetProducts
{
	[ Serializable ]
	internal class MagentoUrl
	{
		internal MagentoUrl( MagentoImage magentoImage )
		{
			this.Url = magentoImage.ImageUrl;
			this.Type = ( MagentoUrlType )magentoImage.ImageType;
		}

		public MagentoUrl( Models.GetProducts.MagentoUrl magentoImage )
		{
			this.Url = magentoImage.Url;
			this.Type = ( MagentoUrlType )magentoImage.Type;
		}

		public string Url{ get; set; }
		public MagentoUrlType Type{ get; set; }

		public Models.GetProducts.MagentoUrl ToMagentoUrl()
		{
			var res = new Models.GetProducts.MagentoUrl( this.Url, ( Models.GetProducts.MagentoUrlType )this.Type );
			return res;
		}
	}

	[ Flags ]
	public enum MagentoUrlType
	{
		Thumbnail = 0x1,
		SmallImage = 0x2,
		Image = 0x4,
	}
}