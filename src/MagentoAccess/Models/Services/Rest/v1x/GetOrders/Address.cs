using System;
using MagentoAccess.Misc;

namespace MagentoAccess.Models.Services.Rest.v1x.GetOrders
{
	[ Serializable ]
	public sealed class Address : IEquatable< Address >
	{
		public string Region { get; set; }
		public string Postcode { get; set; }
		public string Lastname { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string Email { get; set; }
		public string Telephone { get; set; }
		public string CountryId { get; set; }
		public string Firstname { get; set; }
		public string AddressType { get; set; }
		public string Prefix { get; set; }
		public string Middlename { get; set; }
		public string Suffix { get; set; }
		public string Company { get; set; }

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( this, obj ) )
				return true;

			var order = obj as Address;
			if( order == null )
				return false;
			else
				return this.Equals( order );
		}

		public override string ToString()
		{
			var result = this.ToJson();
			return result;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = this.Region.GetHashCode();
				result = ( result * 381 ) ^ this.Postcode.GetHashCode();
				result = ( result * 381 ) ^ this.Lastname.GetHashCode();
				result = ( result * 381 ) ^ this.Street.GetHashCode();
				result = ( result * 381 ) ^ this.City.GetHashCode();
				result = ( result * 381 ) ^ this.Email.GetHashCode();
				result = ( result * 381 ) ^ this.Telephone.GetHashCode();
				result = ( result * 381 ) ^ this.CountryId.GetHashCode();
				result = ( result * 381 ) ^ this.Firstname.GetHashCode();
				result = ( result * 381 ) ^ this.AddressType.GetHashCode();
				result = ( result * 381 ) ^ this.Prefix.GetHashCode();
				result = ( result * 381 ) ^ this.Middlename.GetHashCode();
				result = ( result * 381 ) ^ this.Suffix.GetHashCode();
				result = ( result * 381 ) ^ this.Company.GetHashCode();
				return result;
			}
		}

		public bool Equals( Address other )
		{
			if( other == null )
				return false;

			var res = Equals( this.Region, other.Region )
			          && Equals( this.Postcode, other.Postcode )
			          && Equals( this.Lastname, other.Lastname )
			          && Equals( this.Street, other.Street )
			          && Equals( this.City, other.City )
			          && Equals( this.Email, other.Email )
			          && Equals( this.Telephone, other.Telephone )
			          && Equals( this.CountryId, other.CountryId )
			          && Equals( this.Firstname, other.Firstname )
			          && Equals( this.AddressType, other.AddressType )
			          && Equals( this.Prefix, other.Prefix )
			          && Equals( this.Middlename, other.Middlename )
			          && Equals( this.Suffix, other.Suffix )
			          && Equals( this.Company, other.Company );
			return res;
		}
	}
}