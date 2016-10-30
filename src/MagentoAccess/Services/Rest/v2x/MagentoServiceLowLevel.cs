using MagentoAccess.Models.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x.WebRequester;

namespace MagentoAccess.Services.Rest.v2x
{
	public class MagentoServiceLowLevel
	{
		public void GetProducts()
		{
			WebRequest.Create().Method(MagentoWebRequestMethod.Get).Path(MagentoServicePath.Products);
		}
	}
}