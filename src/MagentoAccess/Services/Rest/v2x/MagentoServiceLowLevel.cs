using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.Rest.v2x.Products;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Newtonsoft.Json;
using SearchCriteria = MagentoAccess.Models.Services.Rest.v2x.SearchCriteria;

namespace MagentoAccess.Services.Rest.v2x
{
	public class MagentoServiceLowLevel
	{
		public async Task< RootObject > GetProductsAsync()
		{
			var parameters = new SearchCriteria( new List< SearchCriteria.FilterGroup >()
			{
				new SearchCriteria.FilterGroup( new List< SearchCriteria.FilterGroup.Filter >()
				{
					new SearchCriteria.FilterGroup.Filter( @"updated_at", @"2016-07-01 00:00:00", SearchCriteria.FilterGroup.Filter.ConditionType.GreaterThan ),
				} )
			} )
			{ CurrentPage = 1, PageSize = 100 };

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
				.Parameters( parameters )
				.Url( MagentoUrl.Create( "http://xxx/magento-2-0-2-0-ce" ) );

			RootObject response;
			using( var v = await webRequest.RunAsync().ConfigureAwait( false ) )
			{
				response = JsonConvert.DeserializeObject< RootObject >( v.ToString() );
			}
			return response;
		}
	}
}