using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x;
using MagentoAccess.Models.Services.Rest.v2x.Products;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Netco.Extensions;
using Netco.Logging;
using Newtonsoft.Json;
using RootObject = MagentoAccess.Models.Services.Rest.v2x.Products.RootObject;
using WebRequest = MagentoAccess.Services.Rest.v2x.WebRequester.WebRequest;

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
			return await this.GetProductsAsync( DateTime.MinValue, page ).ConfigureAwait( false );
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

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, PagingModel page, Mark mark = null )
		{
			return await this.GetProductsAsync( updatedAt, null, false, page, mark ).ConfigureAwait( false );
		}

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, PagingModel page )
		{
			return await this.GetProductsAsync( updatedAt, type, false, page ).ConfigureAwait( false );
		}

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, PagingModel page, Mark mark = null )
		{
			var filterGroups = new List< FilterGroup >() { };

			if( !string.IsNullOrWhiteSpace( type ) )
			{
				filterGroups.Add( new FilterGroup()
				{
					filters = new List< Filter > { new Filter( @"type_id", type, excludeType ? Filter.ConditionType.NotEqual : Filter.ConditionType.Equals ) }
				} );
			}
			if( updatedAt > DateTime.MinValue )
			{
				filterGroups.Add( new FilterGroup()
				{
					filters = new List< Filter > { new Filter( @"updated_at", updatedAt.ToRestParameterString(), Filter.ConditionType.GreaterThan ) }
				} );
			}
			if( !filterGroups.Any() )
			{
				filterGroups.Add( new FilterGroup() { filters = new List< Filter >() } );
			}

			var parameters = new SearchCriteria()
			{
				filter_groups = filterGroups,
				current_page = page.CurrentPage,
				page_size = page.ItemsPerPage
			};
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.CreateProductsServicePath() )
				.Parameters( parameters )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync( mark ).ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} ).ConfigureAwait( false );
		}

		public async Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, Mark mark = null )
		{
			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( updatedAt, pagingModel, mark.CreateChildOrNull() ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 10, async x => await this.GetProductsAsync( updatedAt, new PagingModel( pagingModel.ItemsPerPage, x ), mark.CreateChildOrNull() ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< List< RootObject > > GetProductsAsync( string type, bool excludeType )
		{
			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( DateTime.MinValue, type, excludeType, pagingModel ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 10, async x => await this.GetProductsAsync( DateTime.MinValue, new PagingModel( pagingModel.ItemsPerPage, x ) ).ConfigureAwait( false ) ).ConfigureAwait( false );

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

		public async Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, Mark mark = null )
		{
			if( string.IsNullOrWhiteSpace( type ) )
				return await this.GetProductsAsync( updatedAt, mark.CreateChildOrNull() ).ConfigureAwait( false );

			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( updatedAt, type, excludeType, pagingModel, mark.CreateChildOrNull() ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 10, async x => await this.GetProductsAsync( updatedAt, type, excludeType, new PagingModel( pagingModel.ItemsPerPage, x ), mark.CreateChildOrNull() ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< Item > GetProductAsync( string sku )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.CreateProductsServicePath().AddCatalog( Uri.EscapeDataString( sku ) ) )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				try
				{
					using( var v = await webRequest.RunAsync( Mark.CreateNew() ).ConfigureAwait( false ) )
					{
						return JsonConvert.DeserializeObject< Item >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					}
				}
				catch( MagentoWebException exception )
				{
					if( exception.IsNotFoundException() )
					{
						return null;
					}
					throw;
				}
			} );
		}

		public async Task< CategoryNode > GetCategoriesTreeAsync()
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.CreateCategoriesPath() )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync( Mark.CreateNew() ).ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< CategoryNode >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} );
		}
	}
}