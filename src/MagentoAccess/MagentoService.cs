using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetMagentoCoreInfo;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PingRest;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.Credentials;
using MagentoAccess.Models.Services.GetStockItems;
using MagentoAccess.Services;
using Netco.Extensions;

namespace MagentoAccess
{
	public class MagentoService : IMagentoService
	{
		public bool UseSoapOnly { get; set; }

		internal virtual IMagentoServiceLowLevel MagentoServiceLowLevel { get; set; }

		internal virtual IMagentoServiceLowLevelSoap MagentoServiceLowLevelSoap { get; set; }

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
				MagentoLogger.LogTraceStarted( string.Format( "{{MethodName:{0}, SoapInfo{1}}}", currentMenthodName, soapInfo ) );
				var magentoInfo = await this.MagentoServiceLowLevelSoap.GetMagentoInfoAsync().ConfigureAwait( false );
				var soapWorks = !string.IsNullOrWhiteSpace( magentoInfo.result.magento_version ) || !string.IsNullOrWhiteSpace( magentoInfo.result.magento_edition );

				var magentoCoreInfo = new PingSoapInfo( magentoInfo.result.magento_version, magentoInfo.result.magento_edition, soapWorks );
				MagentoLogger.LogTraceEnded( string.Format( "{{MethodName:{0}, SoapInfo{1}, MethodResult:{2}}}", currentMenthodName, soapInfo, magentoCoreInfo.ToJson() ) );

				return magentoCoreInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, SoapInfo:{1}", currentMenthodName, soapInfo ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< PingRestInfo > PingRestAsync()
		{
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			const string currentMenthodName = "PingRestAsync";
			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "{{MethodName:{0}, RestInfo:{1}}}", currentMenthodName, restInfo ) );

				var magentoOrders = await this.MagentoServiceLowLevel.GetProductsAsync( 1, 1, true ).ConfigureAwait( false );
				var restWorks = magentoOrders.Products != null;
				var magentoCoreInfo = new PingRestInfo( restWorks );

				MagentoLogger.LogTraceEnded( string.Format( "{{MethodName:{0}, RestInfo:{1}, MethodResult:{2}}}", currentMenthodName, restInfo, magentoCoreInfo.ToJson() ) );

				return magentoCoreInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, RestInfo:{1}", currentMenthodName, restInfo ), exception );
				MagentoLogger.LogTraceException( mexc );
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
			var mark = Guid.NewGuid().ToString();

			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "{{MethodName:\"{0}\", SoapInfo:\"{1}\", MethodParameters:\"{2}\", Mark:\"{3}\"}}", currentMenthodName, soapInfo, methodParameters, mark ) );

				var interval = new TimeSpan( 7, 0, 0, 0 );
				var intervalOverlapping = new TimeSpan( 0, 0, 0, 1 );

				var dates = SplitToDates( dateFromUtc, dateToUtc, interval, intervalOverlapping );

				var ordersBriefInfos = await dates.ProcessInBatchAsync( 30, async x =>
				{
					MagentoLogger.LogTrace( string.Format( "OrdersRequested: {{MethodName:\"{0}\", SoapInfo:\"{1}\", MethodParameters:\"{2}\", called from:\"{3}\"}}", "GetOrderAsync", soapInfo, String.Format( "{0},{1}", x.Item1, x.Item2 ), mark ) );
					var res = await this.MagentoServiceLowLevelSoap.GetOrdersAsync( x.Item1, x.Item2 ).ConfigureAwait( false );
					MagentoLogger.LogTrace( string.Format( "OrdersReceived: {{MethodName:\"{0}\", SoapInfo:\"{1}\", MethodParameters:\"{2}\", called from:\"{3}\"}}", "GetOrderAsync", soapInfo, String.Format( "{0},{1}", x.Item1, x.Item2 ), mark ) );
					return res;
				} ).ConfigureAwait( false );

				var ordersBriefInfo = ordersBriefInfos.Where( x => x != null && x.Orders != null ).SelectMany( x => x.Orders ).ToList();

				ordersBriefInfo = ordersBriefInfo.Distinct( new SalesOrderByOrderIdComparer() ).ToList();

				var ordersBriefInfoString = ordersBriefInfo.ToJson();

				MagentoLogger.LogTrace( string.Format( "{{MethodName:\"{0}\", SoapInfo:\"{1}\", MethodParameters:\"{2}\", Mark:\"{3}\", BriefOrdersReceived:\"{4}\"}}", currentMenthodName, soapInfo, methodParameters, mark, ordersBriefInfoString ) );

				var salesOrderInfoResponses = await ordersBriefInfo.ProcessInBatchAsync( 16, async x =>
				{
					MagentoLogger.LogTrace( string.Format( "OrderRequested: {{MethodName:\"{0}\", SoapInfo:\"{1}\", MethodParameters:\"{2}\", called from:\"{3}\"}}", "GetOrderAsync", soapInfo, x.incrementId, mark ) );
					var res = await this.MagentoServiceLowLevelSoap.GetOrderAsync( x.incrementId ).ConfigureAwait( false );
					MagentoLogger.LogTrace( string.Format( "OrderReceived: {{MethodName:\"{0}\", SoapInfo:\"{1}\", MethodParameters:\"{2}\", called from:\"{3}\"}}", "GetOrderAsync", soapInfo, x.incrementId, mark ) );
					return res;
				} ).ConfigureAwait( false );

				var salesOrderInfoResponsesList = salesOrderInfoResponses.ToList();

				var resultOrders = new List< Order >();

				const int batchSize = 500;
				for( var i = 0; i < salesOrderInfoResponsesList.Count; i += batchSize )
				{
					var orderInfoResponses = salesOrderInfoResponsesList.Skip( i ).Take( batchSize );
					var resultOrderPart = orderInfoResponses.AsParallel().Select( x => new Order( x.result ) ).ToList();
					resultOrders.AddRange( resultOrderPart );
					var resultOrdersBriefInfo = resultOrderPart.ToJsonAsParallel( 0, batchSize );
					var partDescription = "From: " + i.ToString() + "," + ( ( i + batchSize < salesOrderInfoResponsesList.Count ) ? batchSize : salesOrderInfoResponsesList.Count % batchSize ).ToString() + " items(or few)";
					MagentoLogger.LogTraceEnded( string.Format( "MethodName:\"{0}\",LogPart:\"{5}\", SoapInfo:\"{1}\", MethodParameters:\"{2}\", Mark:\"{3}\", MethodResult:\"{4}\"", currentMenthodName, soapInfo, methodParameters, mark, resultOrdersBriefInfo, partDescription ) );
				}

				return resultOrders;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:\"{0}\", SoapInfo:\"{1}\", MethodParameters:\"{2}\", Mark:\"{3}\"", currentMenthodName, soapInfo, methodParameters, mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		private static List< Tuple< DateTime, DateTime > > SplitToDates( DateTime dateFromUtc, DateTime dateToUtc, TimeSpan interval, TimeSpan intervalOverlapping )
		{
			var dates = new List< Tuple< DateTime, DateTime > >();
			var dateFromUtcCopy = dateFromUtc;
			var dateToUtcCopy = dateToUtc;
			while( dateFromUtcCopy < dateToUtcCopy )
			{
				dates.Add( Tuple.Create( dateFromUtcCopy, dateFromUtcCopy.Add( interval ).Add( intervalOverlapping ) ) );
				dateFromUtcCopy = dateFromUtcCopy.Add( interval );
			}
			var lastInterval = dates.Last();
			dates.Remove( lastInterval );
			dates.Add( Tuple.Create( lastInterval.Item1, dateToUtc ) );
			return dates;
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync()
		{
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			const string currentMenthodName = "GetOrdersAsync";
			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "{{MethodName:{0}, RestInfo:{1}}}", currentMenthodName, restInfo ) );
				var res = await this.MagentoServiceLowLevel.GetOrdersAsync().ConfigureAwait( false );
				var resHandled = res.Orders.Select( x => new Order( x ) );
				var orderBriefInfo = resHandled.ToJson();
				MagentoLogger.LogTraceEnded( string.Format( "{{MethodName:{0}, RestInfo:{1}, MethodResult:{2}}}", currentMenthodName, restInfo, orderBriefInfo ) );
				return resHandled;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, RestInfo:{1}", currentMenthodName, restInfo ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Product > > GetProductsSimpleAsync()
		{
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			const string currentMenthodName = "GetProductsSimpleAsync";
			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "{{MethodName:{0}, RestInfo:{1}}}", currentMenthodName, restInfo ) );
				var res = await this.GetRestProductsAsync().ConfigureAwait( false );

				var productBriefInfo = res.ToJson();
				MagentoLogger.LogTraceEnded( string.Format( "{{MethodName:{0}, RestInfo:{1}, MethodResult:{2}}}", currentMenthodName, restInfo, productBriefInfo ) );

				return res;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, RestInfo:{1}", currentMenthodName, restInfo ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Product > > GetProductsAsync()
		{
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			var soapInfo = this.MagentoServiceLowLevelSoap.ToJsonSoapInfo();
			const string currentMenthodName = "GetProductsAsync";
			var mark = Guid.NewGuid().ToString();
			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "{{MethodName:{0}, SoapInfo:{1}, RestInfo:{2}, Mark:{3}}}", currentMenthodName, soapInfo, restInfo, mark ) );

				IEnumerable< Product > resultProducts;

				var pingres = await this.PingSoapAsync().ConfigureAwait( false );

				switch( pingres.Version )
				{
					case MagentoVersions.M1901:
						resultProducts = await this.GetProductsBySoap().ConfigureAwait( false );
						break;
					default:
						resultProducts = await this.GetProductsBySoap().ConfigureAwait( false );
						break;
				}

				var resultProductsBriefInfo = resultProducts.ToJson();

				MagentoLogger.LogTraceEnded( string.Format( "{{MethodName:{0}, SoapInfo:{1}, RestInfo:{2}, Mark:{3}, MethodResult:{4}}}", currentMenthodName, soapInfo, restInfo, mark, resultProductsBriefInfo ) );

				return resultProducts;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, SoapInfo:{1}, RestInfo:{2}", currentMenthodName, soapInfo, restInfo ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		private async Task< IEnumerable< Product > > GetProductsBySoap()
		{
			const int stockItemsListMaxChunkSize = 1000;
			IEnumerable< Product > resultProducts = new List< Product >();
			var catalogProductListResponse = await this.MagentoServiceLowLevelSoap.GetProductsAsync().ConfigureAwait( false );

			if( catalogProductListResponse == null || catalogProductListResponse.result == null )
				return resultProducts;

			var products = catalogProductListResponse.result.ToList();

			var productsDevidedByChunks = products.Batch( stockItemsListMaxChunkSize );

			// this code works to solw on 1 core server (but seems faster on multicore)
			//var getStockItemsAsyncTasks = productsDevidedByChunks.Select( stockItemsChunk => this.MagentoServiceLowLevelSoap.GetStockItemsAsync( stockItemsChunk.Select( x => x.sku ).ToList() ) );
			//var stockItemsResponses = await Task.WhenAll(getStockItemsAsyncTasks).ConfigureAwait(false);
			//if (stockItemsResponses == null || !stockItemsResponses.Any())
			//	return Enumerable.Empty<Product>();
			//var stockItems = stockItemsResponses.Where(x => x != null && x.result != null).SelectMany(x => x.result).ToList();

			// this code works faster on 1 core machine 
			var getStockItemsAsync = new List< catalogInventoryStockItemEntity >();
			foreach( var productsDevidedByChunk in productsDevidedByChunks )
			{
				var catalogInventoryStockItemListResponse = await this.MagentoServiceLowLevelSoap.GetStockItemsAsync( productsDevidedByChunk.Select( x => x.sku ).ToList() ).ConfigureAwait( false );
				getStockItemsAsync.AddRange( catalogInventoryStockItemListResponse.result.ToList() );
			}
			var stockItems = getStockItemsAsync.ToList();

			resultProducts = ( from stockItemEntity in stockItems join productEntity in products on stockItemEntity.product_id equals productEntity.product_id select new Product { ProductId = stockItemEntity.product_id, EntityId = productEntity.product_id, Name = productEntity.name, Sku = productEntity.sku, Qty = stockItemEntity.qty } ).ToList();
			return resultProducts;
		}

		private async Task< IEnumerable< Product > > GetProductsByRest()
		{
			IEnumerable< Product > resultProducts;

			// this code not works for magento 1.8.0.1 http://www.magentocommerce.com/bug-tracking/issue/index/id/130
			// this code works for magento 1.9.0.1
			var stockItemsAsync = this.GetRestStockItemsAsync();

			var productsAsync = this.GetRestProductsAsyncPparallel();

			await Task.WhenAll( stockItemsAsync, productsAsync ).ConfigureAwait( false );

			var stockItems = stockItemsAsync.Result.ToList();

			var products = productsAsync.Result.ToList();
			//#if DEBUG
			//			var temps = stockItems.Select( x => string.Format( "INSERT INTO [dbo].[StockItems] ([EntityId] ,[ProductId] ,[Qty]) VALUES ('{0}','{1}','{2}');", x.EntityId, x.ProductId, x.Qty ) );
			//			var stockItemsStr = string.Join( "\n", temps );
			//			var tempp = products.Select( x => string.Format( "INSERT INTO [dbo].[Products2]([EntityId] ,[ProductId] ,[Description] ,[Name] ,[Sku] ,[Price]) VALUES ('{0}','{1}','','','{4}','{5}');", x.EntityId, x.ProductId, x.Description, x.Name, x.Sku, x.Price ) );
			//			var productsStr = string.Join( "\n", tempp );
			//#endif

			resultProducts = ( from stockItem in stockItems join product in products on stockItem.ProductId equals product.EntityId select new Product { ProductId = stockItem.ProductId, EntityId = stockItem.EntityId, Description = product.Description, Name = product.Name, Sku = product.Sku, Price = product.Price, Qty = stockItem.Qty } ).ToList();
			return resultProducts;
		}

		public async Task UpdateInventoryAsync( IEnumerable< Inventory > products )
		{
			var productsBriefInfo = products.ToJson();
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			var soapInfo = this.MagentoServiceLowLevelSoap.ToJsonSoapInfo();
			const string currentMenthodName = "UpdateInventoryAsync";
			var mark = Guid.NewGuid().ToString();
			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "{{MethodName:{0}, SoapInfo:{1},RestInfo:{2}, Mark:{3}, MathodParameters:{4}}}", currentMenthodName, soapInfo, restInfo, mark, productsBriefInfo ) );

				var inventories = products as IList< Inventory > ?? products.ToList();
				var updateBriefInfo = PredefinedValues.NotAvailable;
				if( inventories.Any() )
				{
					var pingres = await this.PingSoapAsync().ConfigureAwait( false );

					switch( pingres.Version )
					{
						case MagentoVersions.M1901:
							updateBriefInfo = await this.UpdateStockItemsBySoap( inventories, mark ).ConfigureAwait( false );
							break;
						case MagentoVersions.M1702:
							updateBriefInfo = await this.UpdateStockItemsBySoapByThePiece( inventories, mark ).ConfigureAwait( false );
							break;
						default:
							updateBriefInfo = await this.UpdateStockItemsBySoap( inventories, mark ).ConfigureAwait( false );
							break;
					}
				}

				MagentoLogger.LogTraceEnded( string.Format( "{{MethodName:{0}, SoapInfo:{1},RestInfo:{2},Mark:{3}, MathodParameters:{4}, MethodResult:{5}}}", currentMenthodName, soapInfo, restInfo, mark, productsBriefInfo, updateBriefInfo ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, SoapInfo:{1},RestInfo:{2}, Mark:{3}, MethodParameters:{4}", currentMenthodName, soapInfo, restInfo, mark, productsBriefInfo ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		private async Task< string > UpdateStockItemsBySoapByThePiece( IList< Inventory > inventories, string mark )
		{
			var productToUpdate = inventories.Select( x => new PutStockItem( x.ProductId, new catalogInventoryStockItemUpdateEntity { qty = x.Qty.ToString() } ) ).ToList();

			var batchResponses = await productToUpdate.ProcessInBatchAsync( 5, async x => new Tuple< bool, List< PutStockItem > >( await this.MagentoServiceLowLevelSoap.PutStockItemAsync( x, mark ).ConfigureAwait( false ), new List< PutStockItem > { x } ) );

			var updateBriefInfo = batchResponses.Where( x => x.Item1 ).SelectMany( y => y.Item2 ).ToJson();

			var notUpdatedProducts = batchResponses.Where( x => !x.Item1 ).SelectMany( y => y.Item2 );

			var notUpdatedBriefInfo = notUpdatedProducts.ToJson();

			if( notUpdatedProducts.Any() )
				throw new Exception( string.Format( "Not updated {0}", notUpdatedBriefInfo ) );

			return updateBriefInfo;
		}

		public async Task UpdateInventoryBySkuAsync( IEnumerable< InventoryBySku > inventory )
		{
			var productsBriefInfo = inventory.ToJson();
			var restInfo = this.MagentoServiceLowLevel.ToJsonRestInfo();
			var soapInfo = this.MagentoServiceLowLevelSoap.ToJsonSoapInfo();
			const string currentMenthodName = "UpdateInventoryBySkuAsync";
			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "{{MethodName:{0}, SoapInfo:{1},RestInfo:{2}, MathodParameters:{3}}}", currentMenthodName, soapInfo, restInfo, productsBriefInfo ) );

				var inventories = inventory as IList< InventoryBySku > ?? inventory.ToList();
				var updateBriefInfo = PredefinedValues.NotAvailable;
				if( inventories.Any() )
				{
					if( this.UseSoapOnly )
					{
						var stockitems = await this.MagentoServiceLowLevelSoap.GetStockItemsAsync( inventory.Select( x => x.Sku ).ToList() ).ConfigureAwait( false );
						var productsWithSkuQtyId = from i in inventory join s in stockitems.result on i.Sku equals s.sku select new Inventory() { ItemId = s.product_id, ProductId = s.product_id, Qty = i.Qty };
						await this.UpdateInventoryAsync( productsWithSkuQtyId ).ConfigureAwait( false );
					}
					else
					{
						var productsWithSkuUpdatedQtyId = await this.GetProductsAsync().ConfigureAwait( false );
						var resultProducts = productsWithSkuUpdatedQtyId.Select( x => new Inventory() { ItemId = x.EntityId, ProductId = x.ProductId, Qty = x.Qty.ToLongOrDefault() } );
						await this.UpdateInventoryAsync( resultProducts ).ConfigureAwait( false );
					}
				}

				MagentoLogger.LogTraceEnded( string.Format( "{{MethodName:{0}, SoapInfo:{1},RestInfo:{2}, MathodParameters:{3}, MethodResult:{4}}}", currentMenthodName, soapInfo, restInfo, productsBriefInfo, updateBriefInfo ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( string.Format( "MethodName:{0}, SoapInfo:{1},RestInfo:{2}, MethodParameters:{3}", currentMenthodName, soapInfo, restInfo, productsBriefInfo ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public void InitiateDesktopAuthentication()
		{
			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "InitiateDesktopAuthentication()" ) );
				this.MagentoServiceLowLevel.TransmitVerificationCode = this.TransmitVerificationCode;
				var authorizeTask = this.MagentoServiceLowLevel.InitiateDescktopAuthenticationProcess();
				authorizeTask.Wait();

				if( this.AfterGettingToken != null )
					this.AfterGettingToken.Invoke( this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret );

				MagentoLogger.LogTraceEnded( string.Format( "InitiateDesktopAuthentication()" ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( "Error.", exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public VerificationData RequestVerificationUri()
		{
			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "RequestVerificationUri()" ) );
				var res = this.MagentoServiceLowLevel.RequestVerificationUri();
				MagentoLogger.LogTraceEnded( string.Format( "RequestVerificationUri()" ) );

				return res;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( "Error.", exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public void PopulateAccessTokenAndAccessTokenSecret( string verificationCode, string requestToken, string requestTokenSecret )
		{
			try
			{
				MagentoLogger.LogTraceStarted( string.Format( "PopulateAccessTokenAndAccessTokenSecret(...)" ) );
				this.MagentoServiceLowLevel.PopulateAccessTokenAndAccessTokenSecret( verificationCode, requestToken, requestTokenSecret );

				if( this.AfterGettingToken != null )
					this.AfterGettingToken.Invoke( this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret );

				MagentoLogger.LogTraceEnded( string.Format( "PopulateAccessTokenAndAccessTokenSecret(...)" ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( "Error.", exception );
				MagentoLogger.LogTraceException( mexc );
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

		private async Task< IEnumerable< Product > > GetRestProductsAsyncPparallel()
		{
			var page = 1;
			const int itemsPerPage = 100;

			var getProductsResponse = await this.MagentoServiceLowLevel.GetProductsAsync( page, itemsPerPage ).ConfigureAwait( false );

			var productsChunk = getProductsResponse.Products;
			if( productsChunk.Count() < itemsPerPage )
				return productsChunk.Select( x => new Product { Sku = x.Sku, Description = x.Description, EntityId = x.EntityId, Name = x.Name, Price = x.Price } );

			var receivedProducts = new List< Models.Services.GetProducts.Product >();

			var lastReceiveProducts = productsChunk;

			receivedProducts.AddRange( productsChunk );

			var getProductsTasks = new List< Task< List< Models.Services.GetProducts.Product > > >();

			getProductsTasks.Add( Task.Factory.StartNew( () => this.GetRestProducts( lastReceiveProducts, itemsPerPage, ref page ) ) );
			getProductsTasks.Add( Task.Factory.StartNew( () => this.GetRestProducts( lastReceiveProducts, itemsPerPage, ref page ) ) );
			getProductsTasks.Add( Task.Factory.StartNew( () => this.GetRestProducts( lastReceiveProducts, itemsPerPage, ref page ) ) );
			getProductsTasks.Add( Task.Factory.StartNew( () => this.GetRestProducts( lastReceiveProducts, itemsPerPage, ref page ) ) );

			await Task.WhenAll( getProductsTasks ).ConfigureAwait( false );

			var results = getProductsTasks.SelectMany( x => x.Result ).ToList();
			receivedProducts.AddRange( results );
			receivedProducts = receivedProducts.Distinct( new ProductComparer() ).ToList();

			return receivedProducts.Select( x => new Product { Sku = x.Sku, Description = x.Description, EntityId = x.EntityId, Name = x.Name, Price = x.Price } );
		}

		private List< Models.Services.GetProducts.Product > GetRestProducts( IEnumerable< Models.Services.GetProducts.Product > lastReceiveProducts, int itemsPerPage, ref int page )
		{
			var localIsLastAndCurrentResponsesHaveTheSameProducts = true;
			var localLastReceivedProducts = lastReceiveProducts;
			var localReceivedProducts = new List< Models.Services.GetProducts.Product >();
			do
			{
				Interlocked.Increment( ref page );

				var getProductsTask = this.MagentoServiceLowLevel.GetProductsAsync( page, itemsPerPage );
				getProductsTask.Wait();
				var localProductsChunk = getProductsTask.Result.Products;

				var repeatedItems = from c in localProductsChunk join l in localLastReceivedProducts on c.EntityId equals l.EntityId select l;

				localLastReceivedProducts = localProductsChunk;

				localIsLastAndCurrentResponsesHaveTheSameProducts = repeatedItems.Any();

				// try to get items that was added before last iteration
				if( localIsLastAndCurrentResponsesHaveTheSameProducts )
				{
					var notRrepeatedItems = localProductsChunk.Where( x => !repeatedItems.Exists( r => r.EntityId == x.EntityId ) );
					localReceivedProducts.AddRange( notRrepeatedItems );
				}
				else
					localReceivedProducts.AddRange( localProductsChunk );
			} while( !localIsLastAndCurrentResponsesHaveTheSameProducts );

			return localReceivedProducts;
		}

		private async Task< IEnumerable< Product > > GetRestStockItemsAsync()
		{
			var page = 1;
			const int itemsPerPage = 100;

			var getProductsResponse = await this.MagentoServiceLowLevel.GetStockItemsAsync( page, itemsPerPage ).ConfigureAwait( false );

			var productsChunk = getProductsResponse.Items;
			if( productsChunk.Count() < itemsPerPage )
				return productsChunk.Select( x => new Product { EntityId = x.ItemId } );

			var receivedProducts = new List< StockItem >();

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

		private async Task< string > UpdateStockItemsByRest( IList< Inventory > inventories, string markForLog = "" )
		{
			string updateBriefInfo;
			const int productsUpdateMaxChunkSize = 50;
			var inventoryItems = inventories.Select( x => new Models.Services.PutStockItems.StockItem
			{
				ItemId = x.ItemId,
				MinQty = x.MinQty,
				ProductId = x.ProductId,
				Qty = x.Qty,
				StockId = x.StockId,
			} ).ToList();

			var productsDevidedToChunks = inventoryItems.SplitToChunks( productsUpdateMaxChunkSize );

			var batchResponses = await productsDevidedToChunks.ProcessInBatchAsync( 1, async x => await this.MagentoServiceLowLevel.PutStockItemsAsync( x, markForLog ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var updateResult = batchResponses.Where( y => y.Items != null ).SelectMany( x => x.Items ).ToList();

			var secessefullyUpdated = updateResult.Where( x => x.Code == "200" );

			var unSecessefullyUpdated = updateResult.Where( x => x.Code != "200" );

			updateBriefInfo = updateResult.ToJson();

			if( unSecessefullyUpdated.Any() )
				throw new Exception( string.Format( "Not updated: {0}, Updated: {1}", unSecessefullyUpdated.ToJson(), secessefullyUpdated.ToJson() ) );

			return updateBriefInfo;
		}

		private async Task< string > UpdateStockItemsBySoap( IList< Inventory > inventories, string markForLog = "" )
		{
			const int productsUpdateMaxChunkSize = 50;
			var productToUpdate = inventories.Select( x => new PutStockItem( x.ProductId, new catalogInventoryStockItemUpdateEntity { qty = x.Qty.ToString() } ) ).ToList();

			var productsDevidedToChunks = productToUpdate.SplitToChunks( productsUpdateMaxChunkSize );

			var batchResponses = await productsDevidedToChunks.ProcessInBatchAsync( 1, async x => new Tuple< bool, List< PutStockItem > >( await this.MagentoServiceLowLevelSoap.PutStockItemsAsync( x, markForLog ).ConfigureAwait( false ), x ) );

			var updateBriefInfo = batchResponses.Where( x => x.Item1 ).SelectMany( y => y.Item2 ).ToJson();

			var notUpdatedProducts = batchResponses.Where( x => !x.Item1 ).SelectMany( y => y.Item2 );

			var notUpdatedBriefInfo = notUpdatedProducts.ToJson();

			if( notUpdatedProducts.Any() )
				throw new Exception( string.Format( "Not updated {0}", notUpdatedBriefInfo ) );

			return updateBriefInfo;
		}
	}

	internal class ProductComparer : IEqualityComparer< Models.Services.GetProducts.Product >
	{
		public bool Equals( Models.Services.GetProducts.Product x, Models.Services.GetProducts.Product y )
		{
			return x.EntityId == y.EntityId;
		}

		public int GetHashCode( Models.Services.GetProducts.Product obj )
		{
			return obj.EntityId.GetHashCode();
		}
	}

	internal class SalesOrderByOrderIdComparer : IEqualityComparer< Models.Services.SOAP.GetOrders.Order >
	{
		public bool Equals( Models.Services.SOAP.GetOrders.Order x, Models.Services.SOAP.GetOrders.Order y )
		{
			return x.incrementId == y.incrementId && x.OrderId == y.OrderId;
		}

		public int GetHashCode( Models.Services.SOAP.GetOrders.Order obj )
		{
			return obj.OrderId.GetHashCode() ^ obj.incrementId.GetHashCode();
		}
	}
}