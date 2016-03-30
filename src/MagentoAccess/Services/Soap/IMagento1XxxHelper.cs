using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.Soap.GetProducts;

namespace MagentoAccess.Services.Soap
{
	internal interface IMagento1XxxHelper
	{
		Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts );
	}
}