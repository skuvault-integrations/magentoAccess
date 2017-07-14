using System;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;

namespace MagentoAccess.Models.GetProducts
{
	public class MagentoUrl : IEquatable< MagentoUrl >
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

		public bool Equals( MagentoUrl other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return string.Equals( this.Url, other.Url ) && this.Type == other.Type;
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return Equals( ( MagentoUrl )obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ( ( this.Url != null ? this.Url.GetHashCode() : 0 ) * 397 ) ^ ( int )this.Type;
			}
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