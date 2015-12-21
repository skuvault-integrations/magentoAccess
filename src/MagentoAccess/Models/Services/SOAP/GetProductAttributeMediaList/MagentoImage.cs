using System;
using System.Linq;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList
{
	internal class MagentoImage
	{
		public string ImageUrl { get; set; }

		public MagentoUrlType ImageType { get; set; }

		public MagentoImage( catalogProductImageEntity catalogProductImageEntity )
		{
			ImageUrl = catalogProductImageEntity.url;
			this.ImageType = catalogProductImageEntity.types.Select( x =>
			{
				MagentoUrlType magentoUrlType;
				if( Enum.TryParse( x, true, out magentoUrlType ) )
					return magentoUrlType;
				return ( MagentoUrlType )0;
			} ).Aggregate( ( MagentoUrlType )0, ( x, y ) => x | y );
		}

		public MagentoImage( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductImageEntity catalogProductImageEntity )
		{
			ImageUrl = catalogProductImageEntity.url;
			this.ImageType = catalogProductImageEntity.types.Select( x =>
			{
				MagentoUrlType magentoUrlType;
				if( Enum.TryParse( x, true, out magentoUrlType ) )
					return magentoUrlType;
				return ( MagentoUrlType )0;
			} ).Aggregate( ( MagentoUrlType )0, ( x, y ) => x | y );
		}
	}

	[ Flags ]
	public enum MagentoUrlType
	{
		Thumbnail = 0x1,
		Small_Image = 0x2,
		Image = 0x4,
	}
}