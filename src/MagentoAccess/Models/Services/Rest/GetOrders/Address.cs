using System;

namespace MagentoAccess.Models.Services.Rest.GetOrders
{
	[ Serializable ]
	public sealed class Address
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
	}
}