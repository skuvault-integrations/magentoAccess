using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Netco.Extensions;
using Newtonsoft.Json;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public class SalesOrderRepositoryV1 :  ISalesOrderRepositoryV1
	{
		private AuthorizationToken Token { get; }
		private MagentoUrl Url { get; }

		public SalesOrderRepositoryV1( AuthorizationToken token, MagentoUrl url )
		{
			this.Url = url;
			this.Token = token;
		}
		
		public async Task< StockItem > GetStockItemAsync( string productSku )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Create(
					MagentoServicePath.CatalogStockItems.RepositoryPath +
					$"/{productSku}" ) )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync().ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< StockItem >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} );
		}

		public async Task< IEnumerable< StockItem > > GetOrdersAs6ync( DateTime from, DateTime to, PagingModel page )
		{

			var parameters2 = new SearchCriteria()
			{
				current_page = page.CurrentPage,
				page_size = page.ItemsPerPage,
				filter_groups = new List<FilterGroup>() {new FilterGroup() {filters = new List< Filter>()
				{

					new Filter("updated_at",from.ToRestParameterString(),Filter.ConditionType.GreaterThan)
				} } }
			};




			var parameters = new SearchCriteria(new List<SearchCriteria.FilterGroup>()
			{
				new SearchCriteria.FilterGroup( new List< SearchCriteria.FilterGroup.Filter >()
				{
				} )
			})
			{ CurrentPage = page.CurrentPage, PageSize = page.ItemsPerPage };


			var webRequest = (WebRequest)WebRequest.Create()
				.Method(MagentoWebRequestMethod.Get)
				.Path(MagentoServicePath.Create(MagentoServicePath.SalesOrder.RepositoryPath ))
				.AuthToken(this.Token)
				.Url(this.Url);

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get(async () =>
			{
				using (var v = await webRequest.RunAsync().ConfigureAwait(false))
				{
					return JsonConvert.DeserializeObject<StockItem>(new StreamReader(v, Encoding.UTF8).ReadToEnd());
				}
			});
			var tailProducts = await productSku.ProcessInBatchAsync( 10, async x => await this.GetStockItemAsync( x ).ConfigureAwait( false ) ).ConfigureAwait( false );

			return tailProducts;
		}
	}
}