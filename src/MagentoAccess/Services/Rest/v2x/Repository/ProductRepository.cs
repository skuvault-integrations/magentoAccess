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
using SearchCriteria = MagentoAccess.Models.Services.Rest.v2x.SearchCriteria;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public class ProductRepository
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
			var parameters = new SearchCriteria( new List< SearchCriteria.FilterGroup >()
			{
				new SearchCriteria.FilterGroup( new List< SearchCriteria.FilterGroup.Filter >()
				{
				} )
			} )
			{ CurrentPage = page.CurrentPage, PageSize = page.ItemsPerPage };

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
				.Parameters( parameters )
				.AuthToken( this.Token )
				.Url( this.Url );

			RootObject response;
			using( var v = await webRequest.RunAsync().ConfigureAwait( false ) )
			{
				response = JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd(), new JsonSerializerSettings() { } );
			}
			return response;
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
			var parameters = new SearchCriteria( new List< SearchCriteria.FilterGroup >()
			{
				new SearchCriteria.FilterGroup( new List< SearchCriteria.FilterGroup.Filter >()
				{
					new SearchCriteria.FilterGroup.Filter( @"updated_at", updatedAt.ToRestParameterString(), SearchCriteria.FilterGroup.Filter.ConditionType.GreaterThan ),
				} )
			} )
			{ CurrentPage = page.CurrentPage, PageSize = page.ItemsPerPage };

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
				.Parameters( parameters )
				.AuthToken( this.Token )
				.Url( this.Url );

			RootObject response;
			using( var v = await webRequest.RunAsync().ConfigureAwait( false ) )
			{
				response = JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
			}
			return response;
		}

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, PagingModel page )
		{
			var parameters = new SearchCriteria( new List< SearchCriteria.FilterGroup >()
			{
				new SearchCriteria.FilterGroup( new List< SearchCriteria.FilterGroup.Filter >()
				{
					new SearchCriteria.FilterGroup.Filter( @"updated_at", updatedAt.ToRestParameterString(), SearchCriteria.FilterGroup.Filter.ConditionType.GreaterThan ),
				} ),
				new SearchCriteria.FilterGroup( new List< SearchCriteria.FilterGroup.Filter >()
				{
					new SearchCriteria.FilterGroup.Filter( @"type_id", type, SearchCriteria.FilterGroup.Filter.ConditionType.Equals ),
				} )
			} )
			{ CurrentPage = page.CurrentPage, PageSize = page.ItemsPerPage };

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
				.Parameters( parameters )
				.AuthToken( this.Token )
				.Url( this.Url );

			RootObject response;
			using( var v = await webRequest.RunAsync().ConfigureAwait( false ) )
			{
				response = JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
			}
			return response;
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
	}
}