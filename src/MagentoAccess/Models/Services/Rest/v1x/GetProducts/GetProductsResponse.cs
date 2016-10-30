using System.Collections.Generic;

namespace MagentoAccess.Models.Services.Rest.v1x.GetProducts
{
	public class GetProductsResponse
	{
		public IEnumerable< Product > Products { get; set; }
	}
}