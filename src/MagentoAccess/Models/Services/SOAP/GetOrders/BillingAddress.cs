using AutoMapper;
using MagentoAccess.TsZoey_v_1_9_0_1_CE;

namespace MagentoAccess.Models.Services.Soap.GetOrders
{
	internal class BillingAddress
	{
		public BillingAddress()
		{
		}

		public BillingAddress( salesOrderAddressEntity resultBillingAddress )
		{
			Mapper.Map< TsZoey_v_1_9_0_1_CE.salesOrderAddressEntity, BillingAddress >( resultBillingAddress, this );
		}

		public string AddressId { get; set; }
		public string AddressType { get; set; }
		public string City { get; set; }
		public string Company { get; set; }
		public string CountryId { get; set; }
		public string CreatedAt { get; set; }
		public string Fax { get; set; }
		public string Firstname { get; set; }
		public string IncrementId { get; set; }
		public string IsActive { get; set; }
		public string Lastname { get; set; }
		public string ParentId { get; set; }
		public string Postcode { get; set; }
		public string Region { get; set; }
		public string RegionId { get; set; }
		public string Street { get; set; }
		public string Telephone { get; set; }
		public string UpdatedAt { get; set; }
	}
}