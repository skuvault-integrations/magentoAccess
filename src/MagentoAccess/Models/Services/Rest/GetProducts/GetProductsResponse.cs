using System.Collections.Generic;

namespace MagentoAccess.Models.Services.Rest.GetProducts
{
	public class GetProductsResponse
	{
		public IEnumerable< Product > Products { get; set; }
	}
}