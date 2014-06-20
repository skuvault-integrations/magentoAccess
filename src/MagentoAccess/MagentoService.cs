using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetMagentoCoreInfo;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.Credentials;
using MagentoAccess.Models.Services.PutStockItems;
using MagentoAccess.Services;
using Netco.Extensions;

namespace MagentoAccess
{
	public class MagentoService : IMagentoService
	{
		public bool UseSoapOnly { get; set; }

		internal virtual IMagentoServiceLowLevel MagentoServiceLowLevel { get; set; }

		internal virtual IMagentoServiceLowLevelSoap MagentoServiceLowLevelSoap { get; set; }

		private void LogTraceException( Exception exception )
		{
			MagentoLogger.Log().Trace( exception, "[magento] An exception occured." );
		}

		public delegate void SaveAccessToken( string token, string secret );

		public SaveAccessToken AfterGettingToken { get; set; }

		public TransmitVerificationCodeDelegate TransmitVerificationCode { get; set; }

		public async Task< MagentoCoreInfo > GetMagentoInfoAsync()
		{
			try
			{
				var magentoInfo = await this.MagentoServiceLowLevelSoap.GetMagentoInfoAsync().ConfigureAwait( false );
				var magentoOrders = await this.MagentoServiceLowLevel.GetProductsAsync( 1, 1, true );

				var soapWorks = !string.IsNullOrWhiteSpace( magentoInfo.result.magento_version ) || !string.IsNullOrWhiteSpace( magentoInfo.result.magento_edition );
				var restWorks = magentoOrders.Products != null;

				return new MagentoCoreInfo( magentoInfo.result.magento_version, magentoInfo.result.magento_edition, soapWorks, restWorks );
			}
			catch( Exception exception )
			{
				this.LogTraceException( exception );
				throw;
			}
		}

		public MagentoService( MagentoAuthenticatedUserCredentials magentoAuthenticatedUserCredentials )
		{
			this.MagentoServiceLowLevel = new MagentoServiceLowLevel(
				magentoAuthenticatedUserCredentials.ConsumerKey,
				magentoAuthenticatedUserCredentials.ConsumerSckretKey,
				magentoAuthenticatedUserCredentials.BaseMagentoUrl,
				magentoAuthenticatedUserCredentials.AccessToken,
				magentoAuthenticatedUserCredentials.AccessTokenSecret
				);

			this.MagentoServiceLowLevelSoap = new MagentoServiceLowLevelSoap(
				magentoAuthenticatedUserCredentials.SoapApiUser,
				magentoAuthenticatedUserCredentials.SoapApiKey,
				magentoAuthenticatedUserCredentials.BaseMagentoUrl,
				null
				);
		}

		public MagentoService( MagentoNonAuthenticatedUserCredentials magentoUserCredentials )
		{
			this.MagentoServiceLowLevel = new MagentoServiceLowLevel(
				magentoUserCredentials.ConsumerKey,
				magentoUserCredentials.ConsumerSckretKey,
				magentoUserCredentials.BaseMagentoUrl,
				magentoUserCredentials.RequestTokenUrl,
				magentoUserCredentials.AuthorizeUrl,
				magentoUserCredentials.AccessTokenUrl
				);
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			try
			{
				var ordersBriefInfo = await this.MagentoServiceLowLevelSoap.GetOrdersAsync( dateFrom, dateTo ).ConfigureAwait( false );

				if( ordersBriefInfo == null )
					return Enumerable.Empty< Order >();

				if( ordersBriefInfo.result == null )
					return Enumerable.Empty< Order >();

				var ordersDetailsTasks = ordersBriefInfo.result.Select( x => this.MagentoServiceLowLevelSoap.GetOrderAsync( x.increment_id ) );

				var commontask = await Task.WhenAll( ordersDetailsTasks ).ConfigureAwait( false );

				var resultOrders = commontask.Select( x => new Order( x.result ) );

				return resultOrders;
			}
			catch( Exception exception )
			{
				this.LogTraceException( exception );
				return Enumerable.Empty< Order >();
			}
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync()
		{
			try
			{
				var res = await this.MagentoServiceLowLevel.GetOrdersAsync().ConfigureAwait( false );
				return res.Orders.Select( x => new Order( x ) );
			}
			catch( Exception exception )
			{
				this.LogTraceException( exception );
				return Enumerable.Empty< Order >();
			}
		}

		public async Task< IEnumerable< Product > > GetProductsSimpleAsync()
		{
			try
			{
				return await this.GetRestProductsAsync();
			}
			catch( Exception exception )
			{
				this.LogTraceException( exception );
				return Enumerable.Empty< Product >();
			}
		}

		public async Task< IEnumerable< Product > > GetProductsAsync()
		{
			try
			{
				const int stockItemsListMaxChunkSize = 500;
				if( this.UseSoapOnly )
				{
					var products = await this.MagentoServiceLowLevelSoap.GetProductsAsync().ConfigureAwait( false );

					if( products == null || products.result == null )
						return Enumerable.Empty< Product >();

					var productsDevidedByChunks = products.result.ToList().Batch( stockItemsListMaxChunkSize );

					var getStockItemsAsyncTasks = productsDevidedByChunks.Select( stockItemsChunk => this.MagentoServiceLowLevelSoap.GetStockItemsAsync( stockItemsChunk.Select( x => x.sku ).ToList() ) );

					var stockItemsResponses = await Task.WhenAll( getStockItemsAsyncTasks ).ConfigureAwait( false );

					if( stockItemsResponses == null || !stockItemsResponses.Any() )
						return Enumerable.Empty< Product >();

					var stockItems = stockItemsResponses.Where( x => x != null && x.result != null ).SelectMany( x => x.result );

					var res = from stockItemEntity in stockItems join productEntity in products.result on stockItemEntity.product_id equals productEntity.product_id select new Product { ProductId = stockItemEntity.product_id, EntityId = productEntity.product_id, Name = productEntity.name, Sku = productEntity.sku, Qty = stockItemEntity.qty };

					return res;
				}
				else
				{
					var stockItems = await this.GetRestStockItemsAsync().ConfigureAwait( false );

					var products = await this.GetRestProductsAsync().ConfigureAwait( false );

					var res = from stockItem in stockItems join product in products on stockItem.EntityId equals product.EntityId select new Product { ProductId = stockItem.ProductId, EntityId = stockItem.EntityId, Description = product.Description, Name = product.Name, Sku = product.Sku, Price = product.Price, Qty = stockItem.Qty };

					return res;
				}
			}
			catch( Exception exception )
			{
				this.LogTraceException( exception );
				return Enumerable.Empty< Product >();
			}
		}

		public async Task UpdateInventoryAsync( IEnumerable< Inventory > products )
		{
			try
			{
				const int productsUpdateMaxChunkSize = 500;
				var inventories = products as IList< Inventory > ?? products.ToList();
				if( !inventories.Any() )
					return;

				if( this.UseSoapOnly )
				{
					var productToUpdate = inventories.Select( x => new PutStockItem( x.ProductId, new catalogInventoryStockItemUpdateEntity { qty = x.Qty.ToString() } ) ).ToList();

					var productsDevidedToChunks = productToUpdate.SplitToChunks( productsUpdateMaxChunkSize );

					var updateProductsChunbksTasks = productsDevidedToChunks.Select( x => this.MagentoServiceLowLevelSoap.PutStockItemsAsync( x ) );

					await Task.WhenAll( updateProductsChunbksTasks ).ConfigureAwait( false );
				}
				else
				{
					var inventoryItems = inventories.Select( x => new StockItem
					{
						ItemId = x.ItemId,
						MinQty = x.MinQty,
						ProductId = x.ProductId,
						Qty = x.Qty,
						StockId = x.StockId,
					} ).ToList();
					await this.MagentoServiceLowLevel.PutStockItemsAsync( inventoryItems ).ConfigureAwait( false );
				}
			}
			catch( Exception exception )
			{
				this.LogTraceException( exception );
			}
		}

		private async Task< IEnumerable< Product > > GetRestProductsAsync()
		{
			var page = 1;
			const int itemsPerPage = 100;

			var getProductsResponse = await this.MagentoServiceLowLevel.GetProductsAsync( page, itemsPerPage ).ConfigureAwait( false );

			var productsChunk = getProductsResponse.Products;
			if( productsChunk.Count() < itemsPerPage )
				return productsChunk.Select( x => new Product { Sku = x.Sku, Description = x.Description, EntityId = x.EntityId, Name = x.Name, Price = x.Price } );

			var receivedProducts = new List< Models.Services.GetProducts.Product >();

			var lastReceiveProducts = productsChunk;

			bool isLastAndCurrentResponsesHaveTheSameProducts;

			do
			{
				receivedProducts.AddRange( productsChunk );

				var getProductsTask = this.MagentoServiceLowLevel.GetProductsAsync( ++page, itemsPerPage );
				getProductsTask.Wait();
				productsChunk = getProductsTask.Result.Products;

				//var repeatedItems = from l in lastReceiveProducts join c in productsChunk on l.EntityId equals c.EntityId select l;
				var repeatedItems = from c in productsChunk join l in lastReceiveProducts on c.EntityId equals l.EntityId select l;

				lastReceiveProducts = productsChunk;

				isLastAndCurrentResponsesHaveTheSameProducts = repeatedItems.Any();

				// try to get items that was added before last iteration
				if( isLastAndCurrentResponsesHaveTheSameProducts )
				{
					var notRrepeatedItems = productsChunk.Where( x => !repeatedItems.Exists( r => r.EntityId == x.EntityId ) );
					receivedProducts.AddRange( notRrepeatedItems );
				}
			} while( !isLastAndCurrentResponsesHaveTheSameProducts );

			return receivedProducts.Select( x => new Product { Sku = x.Sku, Description = x.Description, EntityId = x.EntityId, Name = x.Name, Price = x.Price } );
		}

		private async Task< IEnumerable< Product > > GetRestStockItemsAsync()
		{
			var page = 1;
			const int itemsPerPage = 100;

			var getProductsResponse = await this.MagentoServiceLowLevel.GetStockItemsAsync( page, itemsPerPage ).ConfigureAwait( false );

			var productsChunk = getProductsResponse.Items;
			if( productsChunk.Count() < itemsPerPage )
				return productsChunk.Select( x => new Product { EntityId = x.ItemId } );

			var receivedProducts = new List< Models.Services.GetStockItems.StockItem >();

			var lastReceiveProducts = productsChunk;

			bool isLastAndCurrentResponsesHaveTheSameProducts;

			do
			{
				receivedProducts.AddRange( productsChunk );

				var getProductsTask = this.MagentoServiceLowLevel.GetStockItemsAsync( ++page, itemsPerPage );
				getProductsTask.Wait();

				productsChunk = getProductsTask.Result.Items;

				var repeatedItems = from c in productsChunk join l in lastReceiveProducts on new { ItemId = c.ItemId, BackOrders = c.BackOrders, Qty = c.Qty } equals new { ItemId = l.ItemId, BackOrders = l.BackOrders, Qty = l.Qty } select l;

				lastReceiveProducts = productsChunk;

				isLastAndCurrentResponsesHaveTheSameProducts = repeatedItems.Any();

				// try to get items that was added before last iteration
				if( isLastAndCurrentResponsesHaveTheSameProducts )
				{
					var notRrepeatedItems = productsChunk.Where( x => !repeatedItems.Exists( r => new { ItemId = r.ItemId, BackOrders = r.BackOrders, Qty = r.Qty } != new { ItemId = x.ItemId, BackOrders = x.BackOrders, Qty = x.Qty } ) );
					receivedProducts.AddRange( notRrepeatedItems );
				}
			} while( !isLastAndCurrentResponsesHaveTheSameProducts );

			return receivedProducts.Select( x => new Product { EntityId = x.ItemId, Qty = x.Qty, ProductId = x.ProductId } );
		}

		public void InitiateDesktopAuthentication()
		{
			try
			{
				this.MagentoServiceLowLevel.TransmitVerificationCode = this.TransmitVerificationCode;
				var authorizeTask = this.MagentoServiceLowLevel.InitiateDescktopAuthenticationProcess();
				authorizeTask.Wait();

				if( this.AfterGettingToken != null )
					this.AfterGettingToken.Invoke( this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret );
			}
			catch( Exception exception )
			{
				this.LogTraceException( exception );
			}
		}

		public VerificationData RequestVerificationUri()
		{
			try
			{
				return this.MagentoServiceLowLevel.RequestVerificationUri();
			}
			catch( Exception ex )
			{
				MagentoLogger.Log().Trace( ex, "An exception occured while attempting to get to get 'Verification URI'" );
				throw;
			}
		}

		public void PopulateAccessTokenAndAccessTokenSecret( string verificationCode, string requestToken, string requestTokenSecret )
		{
			try
			{
				this.MagentoServiceLowLevel.PopulateAccessTokenAndAccessTokenSecret( verificationCode, requestToken, requestTokenSecret );

				if( this.AfterGettingToken != null )
					this.AfterGettingToken.Invoke( this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret );
			}
			catch( MagentoAuthException ex )
			{
				MagentoLogger.Log().Trace( ex, "An exception occured while attempting to  populate access token and access token secret" );
				throw;
			}
			catch( Exception ex )
			{
				MagentoLogger.Log().Trace( ex, "An exception occured while attempting to invoke 'after getting token' action" );
				throw;
			}
		}
	}
}