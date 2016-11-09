using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Newtonsoft.Json;
using Filter = MagentoAccess.Models.Services.Rest.v2x.Filter;
using FilterGroup = MagentoAccess.Models.Services.Rest.v2x.FilterGroup;
using SearchCriteria = MagentoAccess.Models.Services.Rest.v2x.SearchCriteria;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public class SalesOrderRepositoryV1 : ISalesOrderRepositoryV1
	{
		private AuthorizationToken Token { get; }
		private MagentoUrl Url { get; }

		public SalesOrderRepositoryV1( AuthorizationToken token, MagentoUrl url )
		{
			this.Url = url;
			this.Token = token;
		}

		public async Task< RootObject > GetOrdersAsync( IEnumerable< string > productSku, PagingModel page )
		{
			if( productSku == null || !productSku.Any() )
				return default(RootObject);

			var parameters = new SearchCriteria()
			{
				filter_groups = new List< FilterGroup >()
				{
					new FilterGroup()
					{
						filters = new List< Filter >()
						{
							new Filter( "increment_id", string.Join( ",", productSku ), Filter.ConditionType.In )
						}
					}
				},
				current_page = page.CurrentPage,
				page_size = page.ItemsPerPage,
			};

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.SalesOrder )
				.Parameters( parameters )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync().ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} );
		}

		public async Task< RootObject > GetOrdersAsync( DateTime updatedFrom, DateTime updatedTo, PagingModel page )
		{
			var parameters = new SearchCriteria()
			{
				filter_groups = new List< FilterGroup >()
				{
					new FilterGroup()
					{
						filters = new List< Filter >()
						{
							new Filter( "updated_at", updatedFrom.ToRestParameterString(), Filter.ConditionType.From )
						}
					},
					new FilterGroup()
					{
						filters = new List< Filter >()
						{
							new Filter( "updated_at", updatedTo.ToRestParameterString(), Filter.ConditionType.To )
						}
					}
				},
				current_page = page.CurrentPage,
				page_size = page.ItemsPerPage,
			};

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.SalesOrder )
				.Parameters( parameters )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync().ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} );
		}
	}
}