using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	internal interface IRest2XMagentoServiceLowLevel
	{
		Task<IEnumerable<Product>> GetProductsByRest( bool includeDetails, string productType, bool productTypeShouldBeExcluded, IEnumerable<int> scopes, DateTime? updatedFrom, IEnumerable<string> skus, bool stockItemsOnly, Mark mark = null );
	}
}
