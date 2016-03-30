using System;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;

namespace MagentoAccess.Models.GetProducts
{
	public class MagentoUrl
	{
		internal MagentoUrl( MagentoImage magentoImage )
		{
			Url = magentoImage.ImageUrl;
			Type = ( MagentoUrlType )magentoImage.ImageType;
		}

		public string Url{ get; set; }

		public MagentoUrlType Type{ get; set; }

		internal MagentoUrl( string magentoImageUrl, MagentoUrlType magentoUrlType )
		{
			this.Url = magentoImageUrl;
			this.Type = magentoUrlType;
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