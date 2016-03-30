using System;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;

namespace MagentoAccess.Models.Services.Soap.GetProducts
{
	internal class MagentoUrl
	{
		internal MagentoUrl(MagentoImage magentoImage)
		{
			this.Url = magentoImage.ImageUrl;
			this.Type = (MagentoUrlType)magentoImage.ImageType;
		}

		public string Url { get; set; }
		public MagentoUrlType Type { get; set; }
	}

	[Flags]
	public enum MagentoUrlType
	{
		Thumbnail = 0x1,
		SmallImage = 0x2,
		Image = 0x4,
	}
}