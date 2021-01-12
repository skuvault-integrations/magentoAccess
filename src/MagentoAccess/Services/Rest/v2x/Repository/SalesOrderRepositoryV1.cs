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
using Netco.Extensions;
using Netco.Logging;
using System.Threading;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public class SalesOrderRepositoryV1 : BaseRepository, ISalesOrderRepositoryV1
	{
		private AuthorizationToken Token { get; }
		private MagentoUrl Url { get; }
		private MagentoTimeouts OperationsTimeouts { get; }

		public SalesOrderRepositoryV1( AuthorizationToken token, MagentoUrl url, MagentoTimeouts operationsTimeouts )
		{
			this.Url = url;
			this.Token = token;
			this.OperationsTimeouts = operationsTimeouts;
		}

		public async Task< RootObject > GetOrdersAsync( IEnumerable< string > ids, PagingModel page, CancellationToken cancellationToken, string searchField = "increment_id" )
		{
			var idsList = ids as IList< string > ?? ids.ToList();
			if( ids == null || !idsList.Any() )
				return default(RootObject);

			var parameters = new SearchCriteria()
			{
				filter_groups = new List< FilterGroup >()
				{
					new FilterGroup()
					{
						filters = new List< Filter >()
						{
							new Filter( searchField, string.Join( ",", idsList ), Filter.ConditionType.In )
						}
					}
				},
				current_page = page.CurrentPage,
				page_size = page.ItemsPerPage,
			};

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.CreateOrdersServicePath() )
				.Parameters( parameters )
				.Timeout( OperationsTimeouts[ MagentoOperationEnum.GetOrdersByIds ] )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{
				return TrackNetworkActivityTime( async () =>
				{
					using( var v = await webRequest.RunAsync( cancellationToken, Mark.CreateNew() ) )
					{
						return JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					}
				} );
			} );
		}

		public async Task< RootObject > GetOrdersAsync( DateTime updatedFrom, DateTime updatedTo, PagingModel page, CancellationToken cancellationToken, Mark mark = null )
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
				.Path( MagentoServicePath.CreateOrdersServicePath() )
				.Parameters( parameters )
				.Timeout( OperationsTimeouts[ MagentoOperationEnum.GetModifiedOrders ] )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{
				return TrackNetworkActivityTime( async () =>
				{
					using( var v = await webRequest.RunAsync( cancellationToken, mark ).ConfigureAwait( false ) )
					{
						return JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					}
				} );
			} ).ConfigureAwait( false );
		}

		public async Task< IEnumerable< RootObject > > GetOrdersAsync( IEnumerable< string > productSku, CancellationToken cancellationToken )
		{
			var chunks = productSku.ToList().SplitToChunks( 20 );
			if( !chunks.Any() )
				return new List< RootObject >();

			var resultItems = await chunks.ProcessInBatchAsync( 5, async ch =>
			{
				var pagingModel = new PagingModel( 10, 1 );
				var itemsFirstPage = await this.GetOrdersAsync( ch, pagingModel, cancellationToken ).ConfigureAwait( false );
				var pagesToProcess = pagingModel.GetPages( itemsFirstPage.total_count );
				var tailItems = await pagesToProcess.ProcessInBatchAsync( 5, async x => await this.GetOrdersAsync( ch, new PagingModel( pagingModel.ItemsPerPage, x ), cancellationToken ).ConfigureAwait( false ) ).ConfigureAwait( false );

				var resultItemsInChunk = new List< RootObject >() { itemsFirstPage };
				resultItemsInChunk.AddRange( tailItems );
				return resultItemsInChunk;
			} ).ConfigureAwait( false );

			return resultItems.SelectMany( x => x );
		}

		public async Task< List< RootObject > > GetOrdersAsync( DateTime updatedFrom, DateTime updatedTo, CancellationToken cancellationToken )
		{
			var pagingModel = new PagingModel( 100, 1 );
			var itemsFirstPage = await this.GetOrdersAsync( updatedFrom, updatedTo, pagingModel, cancellationToken ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( itemsFirstPage.total_count );
			var tailItems = await pagesToProcess.ProcessInBatchAsync( 5, async x => await this.GetOrdersAsync( updatedFrom, updatedTo, new PagingModel( pagingModel.ItemsPerPage, x ), cancellationToken ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultItems = new List< RootObject >() { itemsFirstPage };
			resultItems.AddRange( tailItems );
			return resultItems;
		}

		public Task< ShipmentsResponse > GetOrdersShipmentsAsync( DateTime updatedFrom, DateTime updatedTo, PagingModel page, CancellationToken cancellationToken, Mark mark = null )
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
				.Path( MagentoServicePath.CreateShipmentsServicePath() )
				.Parameters( parameters )
				.AuthToken( this.Token )
				.Url( this.Url );

			return ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync( cancellationToken, mark ).ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< ShipmentsResponse >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} );
		}
	}
}