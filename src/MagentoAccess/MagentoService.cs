using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetMagentoCoreInfo;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PingRest;
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

		private void LogTraceStarted( string info )
		{
			MagentoLogger.Log().Trace( "[magento] Start call:{0}.", info );
		}

		private void LogTraceEnded( string info )
		{
			MagentoLogger.Log().Trace( "[magento] End call:{0}.", info );
		}

		public delegate void SaveAccessToken( string token, string secret );

		public SaveAccessToken AfterGettingToken { get; set; }

		public TransmitVerificationCodeDelegate TransmitVerificationCode { get; set; }

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

		public async Task< PingSoapInfo > PingSoapAsync()
		{
			var soapInfo = this.MagentoServiceLowLevelSoap.ToJsonSoapInfo();
			const string currentMenthodName = "PingSoapAsync";
			try
			{
				this.LogTraceStarted( string.Format( "{{MethodName:{0}, SoapInfo{1}}}", currentMenthodName, soapInfo ) );
				var magentoInfo = await this.MagentoServiceLowLevelSoap.GetMagentoInfoAsync().ConfigureAwait( false );
				var soapWorks = !string.IsNullOrWhiteSpace( magentoInfo.result.magento_version ) || !string.IsNullOrWhiteSpace( magentoInfo.result.magento_edition );

				var magentoCoreInfo = new PingSoapInfo( magentoInfo.result.magento_version, magentoInfo.result.magento_edition, soapWorks );
				this.LogTraceEnded( string.Format( "{{MethodName:{0}, SoapInfo{1}, MethodResult:{2}}}", currentMenthodName, soapInfo, magentoCoreInfo.ToJson() ) );

				return magentoCoreInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, SoapInfo:{1}", currentMenthodName, soapInfo ), exception );
				this.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< PingRestInfo > PingRestAsync()
		{
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			const string currentMenthodName = "PingRestAsync";
			try
			{
				this.LogTraceStarted( string.Format( "{{MethodName:{0}, RestInfo:{1}}}", currentMenthodName, restInfo ) );

				var magentoOrders = await this.MagentoServiceLowLevel.GetProductsAsync( 1, 1, true ).ConfigureAwait( false );
				var restWorks = magentoOrders.Products != null;
				var magentoCoreInfo = new PingRestInfo( restWorks );

				this.LogTraceEnded( string.Format( "{{MethodName:{0}, RestInfo:{1}, MethodResult:{2}}}", currentMenthodName, restInfo, magentoCoreInfo.ToJson() ) );

				return magentoCoreInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, RestInfo:{1}", currentMenthodName, restInfo ), exception );
				this.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			var dateFromUtc = TimeZoneInfo.ConvertTimeToUtc( dateFrom );
			var dateToUtc = TimeZoneInfo.ConvertTimeToUtc( dateTo );
			var methodParameters = string.Format( "{{dateFrom:{0},dateTo:{1}}}", dateFromUtc, dateToUtc );
			var soapInfo = this.MagentoServiceLowLevelSoap.ToJsonSoapInfo();
			const string currentMenthodName = "GetOrdersAsync";

			try
			{
				this.LogTraceStarted( string.Format( "{{MethodName:{0}, SoapInfo{1}, MethodParameters:{2}}}", currentMenthodName, soapInfo, methodParameters ) );

				var ordersBriefInfo = await this.MagentoServiceLowLevelSoap.GetOrdersAsync( dateFromUtc, dateToUtc ).ConfigureAwait( false );

				if( ordersBriefInfo == null )
					return Enumerable.Empty< Order >();

				if( ordersBriefInfo.result == null )
					return Enumerable.Empty< Order >();

				var ordersDetailsTasks = ordersBriefInfo.result.Select( x => this.MagentoServiceLowLevelSoap.GetOrderAsync( x.increment_id ) );

				var commontask = await Task.WhenAll( ordersDetailsTasks ).ConfigureAwait( false );

				var resultOrders = commontask.Select( x => new Order( x.result ) );

				var resultOrdersBriefInfo = resultOrders.ToJson();

				this.LogTraceEnded( string.Format( "MethodName:{0}, SoapInfo{1}, MethodParameters:{2}, MethodResult:{3}", currentMenthodName, soapInfo, methodParameters, resultOrdersBriefInfo ) );

				return resultOrders;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, SoapInfo:{1}, MethodParameters:{2}", currentMenthodName, soapInfo, methodParameters ), exception );
				this.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync()
		{
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			const string currentMenthodName = "GetOrdersAsync";
			try
			{
				this.LogTraceStarted( string.Format( "{{MethodName:{0}, RestInfo:{1}}}", currentMenthodName, restInfo ) );
				var res = await this.MagentoServiceLowLevel.GetOrdersAsync().ConfigureAwait( false );
				var resHandled = res.Orders.Select( x => new Order( x ) );
				var orderBriefInfo = resHandled.ToJson();
				this.LogTraceEnded( string.Format( "{{MethodName:{0}, RestInfo:{1}, MethodResult:{2}}}", currentMenthodName, restInfo, orderBriefInfo ) );
				return resHandled;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, RestInfo:{1}", currentMenthodName, restInfo ), exception );
				this.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Product > > GetProductsSimpleAsync()
		{
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			const string currentMenthodName = "GetProductsSimpleAsync";
			try
			{
				this.LogTraceStarted( string.Format( "{{MethodName:{0}, RestInfo:{1}}}", currentMenthodName, restInfo ) );
				var res = await this.GetRestProductsAsync().ConfigureAwait( false );

				var productBriefInfo = res.ToJson();
				this.LogTraceEnded( string.Format( "{{MethodName:{0}, RestInfo:{1}, MethodResult:{2}}}", currentMenthodName, restInfo, productBriefInfo ) );

				return res;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, RestInfo:{1}", currentMenthodName, restInfo ), exception );
				this.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Product > > GetProductsAsync()
		{
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			var soapInfo = this.MagentoServiceLowLevelSoap.ToJsonSoapInfo();
			const string currentMenthodName = "GetProductsAsync";
			try
			{
				this.LogTraceStarted( string.Format( "{{MethodName:{0}, SoapInfo:{1}, RestInfo:{2}}}", currentMenthodName, soapInfo, restInfo ) );
				const int stockItemsListMaxChunkSize = 500;
				IEnumerable< Product > resultProducts;
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

					resultProducts = from stockItemEntity in stockItems join productEntity in products.result on stockItemEntity.product_id equals productEntity.product_id select new Product { ProductId = stockItemEntity.product_id, EntityId = productEntity.product_id, Name = productEntity.name, Sku = productEntity.sku, Qty = stockItemEntity.qty };
				}
				else
				{
					var stockItems = await this.GetRestStockItemsAsync().ConfigureAwait( false );

					var products = await this.GetRestProductsAsync().ConfigureAwait( false );

					resultProducts = from stockItem in stockItems join product in products on stockItem.EntityId equals product.EntityId select new Product { ProductId = stockItem.ProductId, EntityId = stockItem.EntityId, Description = product.Description, Name = product.Name, Sku = product.Sku, Price = product.Price, Qty = stockItem.Qty };
				}

				var resultProductsBriefInfo = resultProducts.ToJson();

				this.LogTraceEnded( string.Format( "{{MethodName:{0}, SoapInfo:{1}, RestInfo:{2}, MethodResult:{3}}}", currentMenthodName, soapInfo, restInfo, resultProductsBriefInfo ) );

				return resultProducts;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, SoapInfo:{1}, RestInfo:{2}", currentMenthodName, soapInfo, restInfo ), exception );
				this.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task UpdateInventoryAsync( IEnumerable< Inventory > products )
		{
			var productsBriefInfo = products.ToJson();
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			var soapInfo = this.MagentoServiceLowLevelSoap.ToJsonSoapInfo();
			const string currentMenthodName = "UpdateInventoryAsync";
			try
			{
				this.LogTraceStarted( string.Format( "{{MethodName:{0}, SoapInfo:{1},RestInfo:{2}, MathodParameters:{3}}}", currentMenthodName, soapInfo, restInfo, productsBriefInfo ) );

				var inventories = products as IList< Inventory > ?? products.ToList();
				var updateBriefInfo = PredefinedValues.NotAvailable;
				if( inventories.Any() )
				{
					if( this.UseSoapOnly )
					{
						const int productsUpdateMaxChunkSize = 500;
						var productToUpdate = inventories.Select( x => new PutStockItem( x.ProductId, new catalogInventoryStockItemUpdateEntity { qty = x.Qty.ToString() } ) ).ToList();

						var productsDevidedToChunks = productToUpdate.SplitToChunks( productsUpdateMaxChunkSize );

						var updateProductsChunksTasks = productsDevidedToChunks.Select( x => this.MagentoServiceLowLevelSoap.PutStockItemsAsync( x ) );

						await Task.WhenAll( updateProductsChunksTasks ).ConfigureAwait( false );
					}
					else
					{
						const int productsUpdateMaxChunkSize = 200;
						var inventoryItems = inventories.Select( x => new StockItem
						{
							ItemId = x.ItemId,
							MinQty = x.MinQty,
							ProductId = x.ProductId,
							Qty = x.Qty,
							StockId = x.StockId,
						} ).ToList();

						var productsDevidedToChunks = inventoryItems.SplitToChunks( productsUpdateMaxChunkSize );

						var updateProductsChunksTasks = new List< Task< PutStockItemsResponse > >();

						//updateProductsChunksTasks = productsDevidedToChunks.Select(x => this.MagentoServiceLowLevel.PutStockItemsAsync(x)).ToList();

						foreach( var productsDevidedToChunk in productsDevidedToChunks )
						{
							var stockItemsAsync = await this.MagentoServiceLowLevel.PutStockItemsAsync( productsDevidedToChunk ).ConfigureAwait( false );
							updateProductsChunksTasks.Add( Task.FromResult( stockItemsAsync ) );
						}

						var whenAll = Task.WhenAll( updateProductsChunksTasks );

						await whenAll.ConfigureAwait( false );

						var updateResult = whenAll.Result.SelectMany( x => x.Items ).ToList();

						updateBriefInfo = updateResult.ToJson();

						if( whenAll.IsFaulted )
							throw new Exception( string.Format( "Returned only {0}", updateBriefInfo ) );
					}
				}

				this.LogTraceEnded( string.Format( "{{MethodName:{0}, SoapInfo:{1},RestInfo:{2}, MathodParameters:{3}, MethodResult:{4}}}", currentMenthodName, soapInfo, restInfo, productsBriefInfo, updateBriefInfo ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, SoapInfo:{1},RestInfo:{2}, MethodParameters:{3}", currentMenthodName, soapInfo, restInfo, productsBriefInfo ), exception );
				this.LogTraceException( mexc );
				throw mexc;
			}
		}

		public void InitiateDesktopAuthentication()
		{
			try
			{
				this.LogTraceStarted( string.Format( "InitiateDesktopAuthentication()" ) );
				this.MagentoServiceLowLevel.TransmitVerificationCode = this.TransmitVerificationCode;
				var authorizeTask = this.MagentoServiceLowLevel.InitiateDescktopAuthenticationProcess();
				authorizeTask.Wait();

				if( this.AfterGettingToken != null )
					this.AfterGettingToken.Invoke( this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret );

				this.LogTraceEnded( string.Format( "InitiateDesktopAuthentication()" ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( "Error.", exception );
				this.LogTraceException( mexc );
				throw mexc;
			}
		}

		public VerificationData RequestVerificationUri()
		{
			try
			{
				this.LogTraceStarted( string.Format( "RequestVerificationUri()" ) );
				var res = this.MagentoServiceLowLevel.RequestVerificationUri();
				this.LogTraceEnded( string.Format( "RequestVerificationUri()" ) );

				return res;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( "Error.", exception );
				this.LogTraceException( mexc );
				throw mexc;
			}
		}

		public void PopulateAccessTokenAndAccessTokenSecret( string verificationCode, string requestToken, string requestTokenSecret )
		{
			try
			{
				this.LogTraceStarted( string.Format( "PopulateAccessTokenAndAccessTokenSecret(...)" ) );
				this.MagentoServiceLowLevel.PopulateAccessTokenAndAccessTokenSecret( verificationCode, requestToken, requestTokenSecret );

				if( this.AfterGettingToken != null )
					this.AfterGettingToken.Invoke( this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret );

				this.LogTraceEnded( string.Format( "PopulateAccessTokenAndAccessTokenSecret(...)" ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( "Error.", exception );
				this.LogTraceException( mexc );
				throw mexc;
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
	}
}