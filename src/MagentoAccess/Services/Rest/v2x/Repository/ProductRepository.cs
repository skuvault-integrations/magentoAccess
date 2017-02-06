using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.Products;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Netco.Extensions;
using Newtonsoft.Json;
using Filter = MagentoAccess.Models.Services.Rest.v2x.Filter;
using FilterGroup = MagentoAccess.Models.Services.Rest.v2x.FilterGroup;
using SearchCriteria = MagentoAccess.Models.Services.Rest.v2x.SearchCriteria;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public class ProductRepository : IProductRepository
	{
		private AuthorizationToken Token { get; }
		private MagentoUrl Url { get; }

		public ProductRepository( AuthorizationToken token, MagentoUrl url )
		{
			this.Url = url;
			this.Token = token;
		}

		public async Task< RootObject > GetProductsAsync( PagingModel page )
		{
			var parameters = new SearchCriteria()
			{
				filter_groups = new List< FilterGroup >()
				{
					new FilterGroup()
					{
						filters = new List< Filter > { }
					}
				},
				current_page = page.CurrentPage,
				page_size = page.ItemsPerPage
			};

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
				.Parameters( parameters )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var responseStream = await webRequest.RunAsync().ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< RootObject >( new StreamReader( responseStream, Encoding.UTF8 ).ReadToEnd(), new JsonSerializerSettings() { } );
				}
			} );
		}

		public async Task< List< RootObject > > GetProductsAsync()
		{
			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( pagingModel ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 10, async x => await this.GetProductsAsync( new PagingModel( pagingModel.ItemsPerPage, x ) ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, PagingModel page )
		{
			var parameters = new SearchCriteria()
			{
				filter_groups = new List< FilterGroup >()
				{
					new FilterGroup()
					{
						filters = new List< Filter > { new Filter( @"updated_at", updatedAt.ToRestParameterString(), Filter.ConditionType.GreaterThan ) }
					}
				},
				current_page = page.CurrentPage,
				page_size = page.ItemsPerPage
			};

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
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

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, PagingModel page )
		{
			var parameters = new SearchCriteria()
			{
				filter_groups = new List< FilterGroup >()
				{
					new FilterGroup()
					{
						filters = new List< Filter > { new Filter( @"updated_at", updatedAt.ToRestParameterString(), Filter.ConditionType.GreaterThan ) }
					},
					new FilterGroup()
					{
						filters = new List< Filter > { new Filter( @"type_id", type, Filter.ConditionType.Equals ) }
					},
				},
				current_page = page.CurrentPage,
				page_size = page.ItemsPerPage
			};

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
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

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, PagingModel page )
		{
			var parameters = new SearchCriteria()
			{
				filter_groups = new List< FilterGroup >()
				{
					new FilterGroup()
					{
						filters = new List< Filter > { new Filter( @"updated_at", updatedAt.ToRestParameterString(), Filter.ConditionType.GreaterThan ) }
					},
					new FilterGroup()
					{
						filters = new List< Filter > { new Filter( @"type_id", type, Filter.ConditionType.NotEqual ) }
					},
				},
				current_page = page.CurrentPage,
				page_size = page.ItemsPerPage
			};
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
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

		public async Task< List< RootObject > > GetProductsAsync( DateTime updatedAt )
		{
			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( updatedAt, pagingModel ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 10, async x => await this.GetProductsAsync( updatedAt, new PagingModel( pagingModel.ItemsPerPage, x ) ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type )
		{
			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( updatedAt, type, pagingModel ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 10, async x => await this.GetProductsAsync( updatedAt, type, new PagingModel( pagingModel.ItemsPerPage, x ) ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type, bool excludeType )
		{
			if( string.IsNullOrWhiteSpace( type ) )
				return await this.GetProductsAsync( updatedAt ).ConfigureAwait( false );

			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( updatedAt, type, excludeType, pagingModel ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 10, async x => await this.GetProductsAsync( updatedAt, type, excludeType, new PagingModel( pagingModel.ItemsPerPage, x ) ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< Item > GetProductAsync( string sku )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products.AddCatalog( Uri.EscapeDataString( sku ) ) )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync().ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< Item >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} );
		}
	}
}