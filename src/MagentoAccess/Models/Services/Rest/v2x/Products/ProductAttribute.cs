using System.Collections.Generic;

namespace MagentoAccess.Models.Services.Rest.v2x.Products
{
	public class ProductAttribute
	{
		public string attributeCode { get; set; }
		public List< ProductAttributeOption > options { get; set; }
	}

	public class ProductAttributeOption
	{
		public string label { get; set; }
		public string value { get; set; }
	}
}
