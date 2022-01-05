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
	public class ProductRepository : BaseRepository, IProductRepository
	{
		private AuthorizationToken Token { get; }	//TODO GUARD-2311 If need to update (Option 2), then change to { get; private set }
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

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{
				return TrackNetworkActivityTime( async () =>
				{
					using( var v = await webRequest.RunAsync( cancellationToken, mark ).ConfigureAwait( false ) )
					{
						var response = JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					
						if ( response.items != null )
							response.items = response.items.Where( i => !string.IsNullOrWhiteSpace( i.sku ) ).ToList();

						return response;
					}
				} );
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

			//TODO GUARD-2311 REST Products Sync (FillProductDetails) calls this
			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{
				return TrackNetworkActivityTime( async () =>
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
						//TODO GUARD-2311 Option 2: Get new token if 401/Unauthorized response
						if ( GetNewTokenIfUnauthorized( exception, this.Token ) )
						{
							//Question: If Unauthorized, should we return here or just let it rethrow?
						}
						throw;
					}
				} );
			} );
		}

		/// <param name="exception">Response exception details</param>
		/// <param name="token">If Unauthorized, will get the new token and save here</param>
		/// <returns>True if 401/Unauthorized response, False otherwise</returns>
		private bool GetNewTokenIfUnauthorized( MagentoWebException exception, AuthorizationToken token )
		{
			//TODO GUARD-2311 Might also need to check inner exception as done in
			//MagentoAccess\Services\Rest\v2x\MagentoServiceLowLevel.csReauthorizeAsync > this.RepeatOnAuthProblemAsync = .....RetryAsync
			if ( exception.StatusCode == System.Net.HttpStatusCode.Unauthorized )
			{
				//TODO GUARD-2311 Option 2
				//	1. Get new token (see two alternatives below)
				//		Similar to MagentoAccess.Services.Rest.v2x > MagentoServiceLowLevel > ReauthorizeAsync() but would create a new method in that would call the below method and return the new token
				//		IntegrationAdminTokenRepository.GetTokenAsync( MagentoLogin.Create( this.ApiUser ), MagentoPass.Create( this.ApiKey ), CancellationToken.None )
				//	2. Then save the new token into this.Token
				//Pros:	1. Retrying at the lowest level, in a repository
				//		2. Would then be able to eliminate RepeatOnAuthProblemAsync, once this code is called from all places RepeatOnAuthProblemAsync is used
				//			Therefore, wouldn't have multiple retry policies used on the same call
				//Downside: 1. To just get the new token we would need pass in here the following - IntegrationAdminTokenRepository, ApiUser, ApiKey
				//		Or just a delegate to IntegrationAdminTokenRepository.GetTokenAsync( MagentoLogin.Create( this.ApiUser ), MagentoPass.Create( this.ApiKey ), CancellationToken.None )
				//		Which seems to be a reversal of control, since normally MagentoServiceLowLevel calls the repository, not vice versa
				//		2. There are a lot of places this logic would need to be called from, in this repository and others
				return true;
			}
			return false;
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

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{
				return TrackNetworkActivityTime( async () => 
				{
					using( var v = await webRequest.RunAsync( cancellationToken, mark ).ConfigureAwait( false ) )
					{
						var response = JsonConvert.DeserializeObject< RootObject >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					
						if ( response.items != null )
							response.items = response.items.Where( i => !string.IsNullOrWhiteSpace( i.sku ) ).ToList();

						return response;
					}
				} );
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

			//TODO GUARD-2311 REST Products Sync (FillProductDetails) calls this
			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{
				return TrackNetworkActivityTime( async () => 
				{
					using( var v = await webRequest.RunAsync( cancellationToken, Mark.CreateNew() ).ConfigureAwait( false ) )
					{
						return JsonConvert.DeserializeObject< CategoryNode >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					}
				} );
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

			//TODO GUARD-2311 REST Products Sync (FillProductDetails) calls this
			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{
				return TrackNetworkActivityTime( async () => 
				{
					using( var v = await webRequest.RunAsync( cancellationToken, Mark.CreateNew() ).ConfigureAwait( false ) )
					{
						return JsonConvert.DeserializeObject< ProductAttribute >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					}
				} );
			} );
		}
	}
}