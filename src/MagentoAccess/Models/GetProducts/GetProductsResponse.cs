using System.Collections.Generic;

namespace MagentoAccess.Models.GetProducts
{
	public class GetProductsResponse
	{
		public IEnumerable< Product > Products { get; set; }
	}
}