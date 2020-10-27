using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
		private MagentoTimeouts OperationsTimeouts { get; }

		public ProductRepository( AuthorizationToken token, MagentoUrl url, MagentoTimeouts operationsTimeouts )
		{
			this.Url = url;
			this.Token = token;
			this.OperationsTimeouts = operationsTimeouts;
		}

		public async Task< RootObject > GetProductsAsync( PagingModel page, CancellationToken cancellationToken )
		{
			return await this.GetProductsAsync( DateTime.MinValue, page, cancellationToken ).ConfigureAwait( false );
		}

		public async Task< List< RootObject > > GetProductsAsync( CancellationToken cancellationToken )
		{
			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( pagingModel, cancellationToken ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 5, async x => await this.GetProductsAsync( new PagingModel( pagingModel.ItemsPerPage, x ), cancellationToken ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, PagingModel page, CancellationToken cancellationToken, Mark mark = null )
		{
			return await this.GetProductsAsync( updatedAt, null, false, page, cancellationToken, mark ).ConfigureAwait( false );
		}

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, PagingModel page, CancellationToken cancellationToken )
		{
			return await this.GetProductsAsync( updatedAt, type, false, page, cancellationToken ).ConfigureAwait( false );
		}

		public async Task< RootObject > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, PagingModel page, CancellationToken cancellationToken, Mark mark = null )
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
				.Timeout( OperationsTimeouts[ MagentoOperationEnum.GetFilteredProducts ] )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync( cancellationToken, mark ).ConfigureAwait( false ) )
				{
					var response = JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					
					if ( response.items != null )
						response.items = response.items.Where( i => !string.IsNullOrWhiteSpace( i.sku ) ).ToList();

					return response;
				}
			} ).ConfigureAwait( false );
		}

		public async Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, CancellationToken cancellationToken, Mark mark = null )
		{
			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( updatedAt, pagingModel, cancellationToken, mark.CreateChildOrNull() ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 5, async x => await this.GetProductsAsync( updatedAt, new PagingModel( pagingModel.ItemsPerPage, x ), cancellationToken, mark.CreateChildOrNull() ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< List< RootObject > > GetProductsAsync( string type, bool excludeType, CancellationToken cancellationToken )
		{
			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( DateTime.MinValue, type, excludeType, pagingModel, cancellationToken ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 5, async x => await this.GetProductsAsync( DateTime.MinValue, new PagingModel( pagingModel.ItemsPerPage, x ), cancellationToken ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type, CancellationToken cancellationToken )
		{
			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( updatedAt, type, pagingModel, cancellationToken ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 5, async x => await this.GetProductsAsync( updatedAt, type, new PagingModel( pagingModel.ItemsPerPage, x ), cancellationToken ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< List< RootObject > > GetProductsAsync( DateTime updatedAt, string type, bool excludeType, CancellationToken cancellationToken, Mark mark = null )
		{
			if( string.IsNullOrWhiteSpace( type ) )
				return await this.GetProductsAsync( updatedAt, cancellationToken, mark.CreateChildOrNull() ).ConfigureAwait( false );

			var pagingModel = new PagingModel( 100, 1 );
			var products = await this.GetProductsAsync( updatedAt, type, excludeType, pagingModel, cancellationToken, mark.CreateChildOrNull() ).ConfigureAwait( false );
			var pagesToProcess = pagingModel.GetPages( products.totalCount );
			var tailProducts = await pagesToProcess.ProcessInBatchAsync( 5, async x => await this.GetProductsAsync( updatedAt, type, excludeType, new PagingModel( pagingModel.ItemsPerPage, x ), cancellationToken, mark.CreateChildOrNull() ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var resultProducts = new List< RootObject >() { products };
			resultProducts.AddRange( tailProducts );
			return resultProducts;
		}

		public async Task< Item > GetProductAsync( string sku, CancellationToken cancellationToken )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.CreateProductsServicePath().AddCatalog( Uri.EscapeDataString( sku ) ) )
				.Timeout( OperationsTimeouts[ MagentoOperationEnum.GetProductBySku ] )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				try
				{
					using( var v = await webRequest.RunAsync( cancellationToken, Mark.CreateNew() ).ConfigureAwait( false ) )
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

		public async Task< IEnumerable< RootObject > > GetProductsBySkusAsync( IEnumerable< string > skus, CancellationToken cancellationToken, Mark mark )
		{
			if( skus == null || !skus.Any() )
				return new List< RootObject >();

			const int productSkusPerBatch = 20;
			const int batchesInParallel = 5;
			var skusBatches = skus.Where( x => !string.IsNullOrWhiteSpace( x ) ).Slice( productSkusPerBatch );
			var resultBatches = await skusBatches.ProcessInBatchAsync( batchesInParallel, async skusInBatch => 
			{ 
				return await this.GetProductsBySkusBatchAsync( skusInBatch, cancellationToken, mark ).ConfigureAwait( false );
			} ).ConfigureAwait( false );
			return resultBatches.Where( x => x != null );
		}

		private async Task< RootObject > GetProductsBySkusBatchAsync( IEnumerable< string > skus, CancellationToken cancellationToken, Mark mark )
		{
			if( skus == null )
				return null;

			var filterGroups = new List< FilterGroup >
			{
				new FilterGroup
				{
					filters = new List< Filter > { new Filter( "sku", string.Join( ",", skus ), Filter.ConditionType.In ) }
				}
			};

			var parameters = new SearchCriteria
			{
				filter_groups = filterGroups
			};

			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.CreateProductsServicePath() )
				.Parameters( parameters )
				.Timeout( OperationsTimeouts[ MagentoOperationEnum.GetProductsBySkus ] )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync( cancellationToken, mark ).ConfigureAwait( false ) )
				{
					var response = JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					
					if ( response.items != null )
						response.items = response.items.Where( i => !string.IsNullOrWhiteSpace( i.sku ) ).ToList();

					return response;
				}
			} ).ConfigureAwait( false );
		}

		public async Task< CategoryNode > GetCategoriesTreeAsync( CancellationToken cancellationToken )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.CreateCategoriesPath() )
				.Timeout( OperationsTimeouts[ MagentoOperationEnum.GetProductsCategories ] )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync( cancellationToken, Mark.CreateNew() ).ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< CategoryNode >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} );
		}

		public async Task< ProductAttribute > GetManufacturersAsync( CancellationToken cancellationToken )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.CreateManufacturersServicePath() )
				.Timeout( OperationsTimeouts[ MagentoOperationEnum.GetProductsManufacturers ] )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync( cancellationToken, Mark.CreateNew() ).ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< ProductAttribute >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} );
		}
	}
}