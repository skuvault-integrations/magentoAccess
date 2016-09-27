using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.CreateOrders;
using MagentoAccess.Models.CreateProducts;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.DeleteProducts;
using MagentoAccess.Models.GetMagentoCoreInfo;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PingRest;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.Rest.GetStockItems;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services.Rest;
using MagentoAccess.Services.Soap;
using MagentoAccess.Services.Soap._1_14_1_0_ee;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;
using MagentoAccess.Services.Soap._1_9_2_1_ce;
using MagentoAccess.Services.Soap._2_0_2_0_ce;
using MagentoAccess.Services.Soap._2_1_0_0_ce;
using Netco.Extensions;

namespace MagentoAccess
{
	public class MagentoService : IMagentoService
	{
		public bool UseSoapOnly { get; set; }
		internal virtual IMagentoServiceLowLevelRest MagentoServiceLowLevelRest { get; set; }
		internal virtual IMagentoServiceLowLevelSoap MagentoServiceLowLevelSoap { get; set; }
		internal MagentoServiceLowLevelSoapFactory MagentoServiceLowLevelSoapFactory { get; set; }

		public delegate void SaveAccessToken( string token, string secret );

		public SaveAccessToken AfterGettingToken { get; set; }
		public TransmitVerificationCodeDelegate TransmitVerificationCode { get; set; }
		public Func< string > AdditionalLogInfo { get; set; }

		public async Task< IEnumerable< CreateProductModelResult > > CreateProductAsync( IEnumerable< CreateProductModel > models )
		{
			var methodParameters = models.ToJson();
			var mark = Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

				var pingres = await this.PingSoapAsync().ConfigureAwait( false );
				//crunch for old versions
				var magentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.Version, true, false );

				var productsCreationInfo = await models.ProcessInBatchAsync( 30, async x =>
				{
					MagentoLogger.LogTrace( $"CreatingProduct: {this.CreateMethodCallInfo( mark : mark, methodParameters : x.ToJson() )}" );

					var res = new CreateProductModelResult( x );
					await ActionPolicies.GetAsync.Get( async () => res.Result = await magentoServiceLowLevelSoap.CreateProduct( x.StoreId, x.Name, x.Sku, x.IsInStock,x.ProductType ).ConfigureAwait( false ) ).ConfigureAwait( false );

					MagentoLogger.LogTrace( $"ProductCreated: {this.CreateMethodCallInfo( mark : mark, methodResult : res.ToJson(), methodParameters : x.ToJson() )}" );
					return res;
				} ).ConfigureAwait( false );

				var productsCreationInfoString = productsCreationInfo.ToJson();

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodParameters : methodParameters, notes : "ProductsCerated:\"{0}\"".FormatWith( productsCreationInfoString ) ) );

				return productsCreationInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark, methodParameters : methodParameters ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< CreateOrderModelResult > > CreateOrderAsync( IEnumerable< CreateOrderModel > models )
		{
			var methodParameters = models.ToJson();
			var mark = Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

				var pingres = await this.PingSoapAsync().ConfigureAwait( false );
				//crunch for old versions
				var magentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.Version, true, false );

				var productsCreationInfo = await models.ProcessInBatchAsync( 30, async x =>
				{
					MagentoLogger.LogTrace( $"CreatingOrder: {this.CreateMethodCallInfo( mark : mark, methodParameters : x.ToJson() )}" );

					var res = new CreateOrderModelResult( x );

					var shoppingCartIdTask = magentoServiceLowLevelSoap.CreateCart( x.StoreId );
					shoppingCartIdTask.Wait();
					var _shoppingCartId = shoppingCartIdTask.Result;

					var shoppingCartCustomerSetTask = magentoServiceLowLevelSoap.ShoppingCartGuestCustomerSet( _shoppingCartId, x.CustomerFirstName, x.CustomerMail, x.CustomerLastName, x.StoreId );
					shoppingCartCustomerSetTask.Wait();

					var shoppingCartAddressSet = magentoServiceLowLevelSoap.ShoppingCartAddressSet( _shoppingCartId, x.StoreId );
					shoppingCartAddressSet.Wait();

					var productTask = magentoServiceLowLevelSoap.ShoppingCartAddProduct( _shoppingCartId, x.ProductIds.First(), x.StoreId );
					productTask.Wait();

					var shippingMenthodTask = magentoServiceLowLevelSoap.ShoppingCartSetShippingMethod( _shoppingCartId, x.StoreId );
					shippingMenthodTask.Wait();

					var paymentMenthodTask = magentoServiceLowLevelSoap.ShoppingCartSetPaymentMethod( _shoppingCartId, x.StoreId );
					paymentMenthodTask.Wait();

					var orderIdTask = magentoServiceLowLevelSoap.CreateOrder( _shoppingCartId, x.StoreId );
					orderIdTask.Wait();
					res.OrderId = orderIdTask.Result;
					Task.Delay( 1000 );

					MagentoLogger.LogTrace( $"OrderCreated: {this.CreateMethodCallInfo( mark : mark, methodResult : res.ToJson(), methodParameters : x.ToJson() )}" );
					return res;
				} ).ConfigureAwait( false );

				var productsCreationInfoString = productsCreationInfo.ToJson();

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodParameters : methodParameters, notes : "OrdersCerated:\"{0}\"".FormatWith( productsCreationInfoString ) ) );

				return productsCreationInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark, methodParameters : methodParameters ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< DeleteProductModelResult > > DeleteProductAsync( IEnumerable< DeleteProductModel > models )
		{
			var methodParameters = models.ToJson();
			var mark = Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

				var pingres = await this.PingSoapAsync().ConfigureAwait( false );
				//crunch for old versions
				var magentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.Version, true, false );

				var productsCreationInfo = await models.ProcessInBatchAsync( 30, async x =>
				{
					MagentoLogger.LogTrace( $"DeleteProduct: {this.CreateMethodCallInfo( mark : mark, methodParameters : x.ToJson() )}" );

					var res = new DeleteProductModelResult( x );
					res.Result = await magentoServiceLowLevelSoap.DeleteProduct( x.StoreId, x.CategoryId, x.ProductId, x.IdentiferType ).ConfigureAwait( false );

					MagentoLogger.LogTrace( $"ProductDeleted: {this.CreateMethodCallInfo( mark : mark, methodResult : res.ToJson(), methodParameters : x.ToJson() )}" );
					return res;
				} ).ConfigureAwait( false );

				var productsCreationInfoString = productsCreationInfo.ToJson();

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodParameters : methodParameters, notes : "ProductsDeleted:\"{0}\"".FormatWith( productsCreationInfoString ) ) );

				return productsCreationInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark, methodParameters : methodParameters ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		#region constructor
		public MagentoService( MagentoAuthenticatedUserCredentials magentoAuthenticatedUserCredentials, MagentoConfig magentoConfig )
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

			this.MagentoServiceLowLevelRest = new MagentoServiceLowLevelRestRest(
				magentoAuthenticatedUserCredentials.ConsumerKey,
				magentoAuthenticatedUserCredentials.ConsumerSckretKey,
				magentoAuthenticatedUserCredentials.BaseMagentoUrl,
				magentoAuthenticatedUserCredentials.AccessToken,
				magentoAuthenticatedUserCredentials.AccessTokenSecret
				);

			//all methods should use factory, but it takes time to convert them, since there are a lot of errors in magento which we should avoid
			this.MagentoServiceLowLevelSoapFactory = new MagentoServiceLowLevelSoapFactory( null,
				magentoAuthenticatedUserCredentials.BaseMagentoUrl,
				magentoAuthenticatedUserCredentials.SoapApiKey,
				magentoAuthenticatedUserCredentials.SoapApiUser,
				new Dictionary< string, IMagentoServiceLowLevelSoap >
				{
					{ MagentoVersions.M_1_9_2_0, new MagentoServiceLowLevelSoap_v_1_9_2_1_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials ) },
					{ MagentoVersions.M_1_9_2_1, new MagentoServiceLowLevelSoap_v_1_9_2_1_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials ) },
					{ MagentoVersions.M_1_9_2_2, new MagentoServiceLowLevelSoap_v_1_9_2_1_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials ) },
					{ MagentoVersions.M_1_9_0_1, new MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials ) },
					{ MagentoVersions.M_1_8_1_0, new MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials ) },
					{ MagentoVersions.M_1_7_0_2, new MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials ) },
					{ MagentoVersions.M_1_14_1_0, new MagentoServiceLowLevelSoap_v_1_14_1_0_EE_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials ) },
					{ MagentoVersions.M_2_0_2_0, new MagentoServiceLowLevelSoap_v_2_0_2_0_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials ) },
					{ MagentoVersions.M_2_1_0_0, new MagentoServiceLowLevelSoap_v_2_1_0_0_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials ) },
				}
				);

			var defaultVersion = magentoConfig != null && !string.IsNullOrWhiteSpace( magentoConfig.VersionByDefault ) ? magentoConfig.VersionByDefault : MagentoVersions.M_1_7_0_2;
			this.MagentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( defaultVersion, true, false );
		}

		public MagentoService( MagentoNonAuthenticatedUserCredentials magentoUserCredentials )
		{
			this.MagentoServiceLowLevelRest = new MagentoServiceLowLevelRestRest(
				magentoUserCredentials.ConsumerKey,
				magentoUserCredentials.ConsumerSckretKey,
				magentoUserCredentials.BaseMagentoUrl,
				magentoUserCredentials.RequestTokenUrl,
				magentoUserCredentials.AuthorizeUrl,
				magentoUserCredentials.AccessTokenUrl
				);
		}
		#endregion

		#region ping
		public async Task< PingSoapInfo > DetermineMagentoVersionAndSetupServiceAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );

				var soapInfo = new PingSoapInfo( string.Empty, string.Empty, false );
				var soapInfos = await this.DetermineMagentoVersionAsync( mark ).ConfigureAwait( false );
				var pingSoapInfos = soapInfos as IList< PingSoapInfo > ?? soapInfos.ToList();
				if( pingSoapInfos.Any() )
				{
					var temp = pingSoapInfos.First();
					if( temp != null )
					{
						soapInfo = temp;
						if( !string.IsNullOrWhiteSpace( soapInfo.Version ) )
							this.MagentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( soapInfo.Version, true, false );
					}
				}

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : soapInfo.ToJson() ) );

				return soapInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< PingSoapInfo > > DetermineMagentoVersionAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );

				var magentoLowLevelServices = this.MagentoServiceLowLevelSoapFactory.GetAll();
				var storesVersions = await magentoLowLevelServices.ProcessInBatchAsync( 14, kvp => kvp.Value.GetMagentoInfoAsync( true ) ).ConfigureAwait( false );
				var workingStore = storesVersions.Where( x => x != null && !string.IsNullOrWhiteSpace( x.MagentoEdition ) && !string.IsNullOrWhiteSpace( x.MagentoVersion ) );
				var pingSoapInfo = workingStore.Select( x => new PingSoapInfo( x.MagentoVersion, x.MagentoEdition, true ) );

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : pingSoapInfo.ToJson() ) );

				return pingSoapInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< PingSoapInfo > PingSoapAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );
				var magentoInfo = await this.MagentoServiceLowLevelSoap.GetMagentoInfoAsync( false ).ConfigureAwait( false );
				var soapWorks = !string.IsNullOrWhiteSpace( magentoInfo.MagentoVersion ) || !string.IsNullOrWhiteSpace( magentoInfo.MagentoEdition );

				var magentoCoreInfo = new PingSoapInfo( magentoInfo.MagentoVersion, magentoInfo.MagentoEdition, soapWorks );
				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : magentoCoreInfo.ToJson() ) );

				return magentoCoreInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< PingRestInfo > PingRestAsync()
		{
			var mark = Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );

				var magentoOrders = await this.MagentoServiceLowLevelRest.GetProductsAsync( 1, 1, true ).ConfigureAwait( false );
				var restWorks = magentoOrders.Products != null;
				var magentoCoreInfo = new PingRestInfo( restWorks );

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : magentoCoreInfo.ToJson() ) );

				return magentoCoreInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}
		#endregion

		#region getOrders
		public async Task< IEnumerable< Order > > GetOrdersAsync( IEnumerable< string > orderIds )
		{
			var methodParameters = orderIds.ToJson();

			var mark = Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

				IMagentoServiceLowLevelSoap magentoServiceLowLevelSoap;
				var pingres = await this.PingSoapAsync().ConfigureAwait( false );
				//crunch for old versions
				magentoServiceLowLevelSoap = string.Equals( pingres.Edition, MagentoVersions.M_1_7_0_2, StringComparison.CurrentCultureIgnoreCase )
				                             || string.Equals( pingres.Edition, MagentoVersions.M_1_8_1_0, StringComparison.CurrentCultureIgnoreCase )
				                             || string.Equals( pingres.Edition, MagentoVersions.M_1_9_0_1, StringComparison.CurrentCultureIgnoreCase )
				                             || string.Equals( pingres.Edition, MagentoVersions.M_1_14_1_0, StringComparison.CurrentCultureIgnoreCase ) ? this.MagentoServiceLowLevelSoap : this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.Version, true, false );

				var salesOrderInfoResponses = await orderIds.ProcessInBatchAsync( 16, async x =>
				{
					MagentoLogger.LogTrace( $"OrderRequested: {this.CreateMethodCallInfo( mark : mark, methodParameters : x )}" );
					var res = await magentoServiceLowLevelSoap.GetOrderAsync( x ).ConfigureAwait( false );
					MagentoLogger.LogTrace( $"OrderReceived: {this.CreateMethodCallInfo( mark : mark, methodResult : res.ToJson(), methodParameters : x )}" );
					return res;
				} ).ConfigureAwait( false );

				var salesOrderInfoResponsesList = salesOrderInfoResponses.Where( x => x != null ).ToList();

				var resultOrders = new List< Order >();

				const int batchSize = 500;
				for( var i = 0; i < salesOrderInfoResponsesList.Count; i += batchSize )
				{
					var orderInfoResponses = salesOrderInfoResponsesList.Skip( i ).Take( batchSize );
					var resultOrderPart = orderInfoResponses.AsParallel().Select( x => new Order( x ) ).ToList();
					resultOrders.AddRange( resultOrderPart );
					var resultOrdersBriefInfo = resultOrderPart.ToJsonAsParallel( 0, batchSize );
					var partDescription = "From: " + i.ToString() + "," + ( ( i + batchSize < salesOrderInfoResponsesList.Count ) ? batchSize : salesOrderInfoResponsesList.Count % batchSize ).ToString() + " items(or few)";
					MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : resultOrdersBriefInfo, methodParameters : methodParameters, notes : "LogPart:\"{0}\"".FormatWith( partDescription ) ) );
				}

				return resultOrders;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark, methodParameters : methodParameters ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			var dateFromUtc = TimeZoneInfo.ConvertTimeToUtc( dateFrom );
			var dateToUtc = TimeZoneInfo.ConvertTimeToUtc( dateTo );
			var methodParameters = $"{{dateFrom:{dateFromUtc},dateTo:{dateToUtc}}}";

			var mark = Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

				var interval = new TimeSpan( 7, 0, 0, 0 );
				var intervalOverlapping = new TimeSpan( 0, 0, 0, 1 );

				var dates = SplitToDates( dateFromUtc, dateToUtc, interval, intervalOverlapping );

				var pingres = await this.PingSoapAsync().ConfigureAwait( false );
				//crunch for old versions
				var magentoServiceLowLevelSoap = string.Equals( pingres.Edition, MagentoVersions.M_1_7_0_2, StringComparison.CurrentCultureIgnoreCase )
				                                                         || string.Equals( pingres.Edition, MagentoVersions.M_1_8_1_0, StringComparison.CurrentCultureIgnoreCase )
				                                                         || string.Equals( pingres.Edition, MagentoVersions.M_1_9_0_1, StringComparison.CurrentCultureIgnoreCase )
				                                                         || string.Equals( pingres.Edition, MagentoVersions.M_1_14_1_0, StringComparison.CurrentCultureIgnoreCase ) ? this.MagentoServiceLowLevelSoap : this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.Version, true, false );
				var ordersBriefInfos = await dates.ProcessInBatchAsync( 30, async x =>
				{
					MagentoLogger.LogTrace( $"OrdersRequested: {this.CreateMethodCallInfo( mark : mark, methodParameters : $"{x.Item1},{x.Item2}" )}" );

					var res = await magentoServiceLowLevelSoap.GetOrdersAsync( x.Item1, x.Item2 ).ConfigureAwait( false );

					MagentoLogger.LogTrace( $"OrdersReceived: {this.CreateMethodCallInfo( mark : mark, methodResult : res.ToJson(), methodParameters : $"{x.Item1},{x.Item2}" )}" );
					return res;
				} ).ConfigureAwait( false );

				var getOrdersResponses = ordersBriefInfos.Where( x => x != null && x.Orders != null ).ToList();
				var ordersBriefInfo = getOrdersResponses.SelectMany( x => x.Orders ).ToList();

				ordersBriefInfo = ordersBriefInfo.Distinct( new SalesOrderByOrderIdComparer() ).ToList();

				var ordersBriefInfoString = ordersBriefInfo.ToJson();

				MagentoLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodParameters : methodParameters, notes : "BriefOrdersReceived:\"{0}\"".FormatWith( ordersBriefInfoString ) ) );

				var salesOrderInfoResponses = await ordersBriefInfo.ProcessInBatchAsync( 16, async x =>
				{
					MagentoLogger.LogTrace( $"OrderRequested: {this.CreateMethodCallInfo( mark : mark, methodParameters : x.incrementId )}" );
					var res = await magentoServiceLowLevelSoap.GetOrderAsync( x.incrementId ).ConfigureAwait( false );
					MagentoLogger.LogTrace( $"OrderReceived: {this.CreateMethodCallInfo( mark : mark, methodResult : res.ToJson(), methodParameters : x.incrementId )}" );
					return res;
				} ).ConfigureAwait( false );

				var salesOrderInfoResponsesList = salesOrderInfoResponses.Where( x => x != null ).ToList();

				var resultOrders = new List< Order >();

				const int batchSize = 500;
				for( var i = 0; i < salesOrderInfoResponsesList.Count; i += batchSize )
				{
					var orderInfoResponses = salesOrderInfoResponsesList.Skip( i ).Take( batchSize );
					var resultOrderPart = orderInfoResponses.AsParallel().Select( x => new Order( x ) ).ToList();
					resultOrders.AddRange( resultOrderPart );
					var resultOrdersBriefInfo = resultOrderPart.ToJsonAsParallel( 0, batchSize );
					var partDescription = "From: " + i.ToString() + "," + ( ( i + batchSize < salesOrderInfoResponsesList.Count ) ? batchSize : salesOrderInfoResponsesList.Count % batchSize ).ToString() + " items(or few)";
					MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : resultOrdersBriefInfo, methodParameters : methodParameters, notes : "LogPart:\"{0}\"".FormatWith( partDescription ) ) );
				}

				return resultOrders;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark, methodParameters : methodParameters ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync()
		{
			var mark = Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );
				var res = await this.MagentoServiceLowLevelRest.GetOrdersAsync().ConfigureAwait( false );
				var resHandled = res.Orders.Select( x => new Order( x ) );
				var orderBriefInfo = resHandled.ToJson();
				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : orderBriefInfo ) );
				return resHandled;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}
		#endregion

		#region getProducts
		public async Task< IEnumerable< Product > > GetProductsSimpleAsync()
		{
			var mark = Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );
				var res = await this.GetRestProductsAsync().ConfigureAwait( false );

				var productBriefInfo = res.ToJson();
				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : productBriefInfo ) );

				return res;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Product > > GetProductsAsync( bool includeDetails = false, string productType = null, bool excludeProductByType = false )
		{
			var mark = Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );

				var pingres = await this.PingSoapAsync().ConfigureAwait( false );
				var magentoServiceLowLevel = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.Version, true, false );
				var resultProducts = await this.GetProductsBySoap( magentoServiceLowLevel, includeDetails, productType, excludeProductByType ).ConfigureAwait( false );

				var resultProductsBriefInfo = resultProducts.ToJson();

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : resultProductsBriefInfo ) );

				return resultProducts;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Product > > FillProductsDetailsAsync( IEnumerable< Product > products )
		{
			var mark = Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );

				var pingres = await this.PingSoapAsync().ConfigureAwait( false );
				var magentoServiceLowLevel = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.Version, true, false );
				var resultProducts = ( await magentoServiceLowLevel.FillProductDetails( products.Select( x => new ProductDetails( x ) ) ).ConfigureAwait( false ) ).Select( y => y.ToProduct() );

				var resultProductsBriefInfo = resultProducts.ToJson();

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : resultProductsBriefInfo ) );

				return resultProducts;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}
		#endregion

		#region updateInventory
		public async Task UpdateInventoryAsync( IEnumerable< Inventory > products )
		{
			var productsBriefInfo = products.ToJson();
			var mark = Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, methodParameters : productsBriefInfo ) );

				var inventories = products as IList< Inventory > ?? products.ToList();
				var updateBriefInfo = PredefinedValues.NotAvailable;
				if( inventories.Any() )
				{
					var pingres = await this.PingSoapAsync().ConfigureAwait( false );
					//crunch for 1702
					updateBriefInfo = string.Equals( pingres.Version, MagentoVersions.M_1_7_0_2, StringComparison.CurrentCultureIgnoreCase ) 
						? await this.UpdateStockItemsBySoapByThePiece( inventories, mark ).ConfigureAwait( false ) 
						: await this.UpdateStockItemsBySoap( inventories, this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.Version, true, false ), mark ).ConfigureAwait( false );
				}

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodParameters : productsBriefInfo, methodResult : updateBriefInfo ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task UpdateInventoryBySkuAsync( IEnumerable< InventoryBySku > inventory )
		{
			var mark = Mark.CreateNew();
			var productsBriefInfo = inventory.ToJson();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, methodParameters : productsBriefInfo ) );

				var inventories = inventory as IList< InventoryBySku > ?? inventory.ToList();
				var updateBriefInfo = PredefinedValues.NotAvailable;
				if( inventories.Any() )
				{
					if( this.UseSoapOnly )
					{
						var pingres = await this.PingSoapAsync().ConfigureAwait( false );
						var magentoServiceLowLevelSoap = string.Equals( pingres.Edition, MagentoVersions.M_1_7_0_2, StringComparison.CurrentCultureIgnoreCase )
						                                 || string.Equals( pingres.Edition, MagentoVersions.M_1_8_1_0, StringComparison.CurrentCultureIgnoreCase )
						                                 || string.Equals( pingres.Edition, MagentoVersions.M_1_9_0_1, StringComparison.CurrentCultureIgnoreCase )
						                                 || string.Equals( pingres.Edition, MagentoVersions.M_1_14_1_0, StringComparison.CurrentCultureIgnoreCase ) ? this.MagentoServiceLowLevelSoap : this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.Version, true, false );

						var stockitems = await magentoServiceLowLevelSoap.GetStockItemsAsync( inventory.Select( x => x.Sku ).ToList() ).ConfigureAwait( false );
						var productsWithSkuQtyId = from i in inventory join s in stockitems.InventoryStockItems on i.Sku equals s.Sku select new Inventory() { ItemId = s.ProductId, ProductId = s.ProductId, Qty = i.Qty };
						await this.UpdateInventoryAsync( productsWithSkuQtyId ).ConfigureAwait( false );
					}
					else
					{
						var productsWithSkuUpdatedQtyId = await this.GetProductsAsync( ).ConfigureAwait( false );
						var resultProducts = productsWithSkuUpdatedQtyId.Select( x => new Inventory() { ItemId = x.EntityId, ProductId = x.ProductId, Qty = x.Qty.ToLongOrDefault() } );
						await this.UpdateInventoryAsync( resultProducts ).ConfigureAwait( false );
					}
				}

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodParameters : productsBriefInfo, methodResult : updateBriefInfo ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}
		#endregion

		#region auth
		public void InitiateDesktopAuthentication()
		{
			try
			{
				MagentoLogger.LogTraceStarted( "InitiateDesktopAuthentication()" );
				this.MagentoServiceLowLevelRest.TransmitVerificationCode = this.TransmitVerificationCode;
				var authorizeTask = this.MagentoServiceLowLevelRest.InitiateDescktopAuthenticationProcess();
				authorizeTask.Wait();

				this.AfterGettingToken?.Invoke( this.MagentoServiceLowLevelRest.AccessToken, this.MagentoServiceLowLevelRest.AccessTokenSecret );

				MagentoLogger.LogTraceEnded( "InitiateDesktopAuthentication()" );
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
				MagentoLogger.LogTraceStarted( "RequestVerificationUri()" );
				var res = this.MagentoServiceLowLevelRest.RequestVerificationUri();
				MagentoLogger.LogTraceEnded( "RequestVerificationUri()" );

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
				MagentoLogger.LogTraceStarted( "PopulateAccessTokenAndAccessTokenSecret(...)" );
				this.MagentoServiceLowLevelRest.PopulateAccessTokenAndAccessTokenSecret( verificationCode, requestToken, requestTokenSecret );

				this.AfterGettingToken?.Invoke( this.MagentoServiceLowLevelRest.AccessToken, this.MagentoServiceLowLevelRest.AccessTokenSecret );

				MagentoLogger.LogTraceEnded( "PopulateAccessTokenAndAccessTokenSecret(...)" );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( "Error.", exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}
		#endregion

		#region MethodsImplementations
		private string CreateMethodCallInfo( string methodParameters = "", Mark mark = null, string errors = "", string methodResult = "", string additionalInfo = "", [ CallerMemberName ] string memberName = "", string notes = "" )
		{
			additionalInfo = ( string.IsNullOrWhiteSpace( additionalInfo ) && this.AdditionalLogInfo != null ) ? this.AdditionalLogInfo() : PredefinedValues.EmptyJsonObject;
			mark = mark ?? Mark.Blank();
			var connectionInfo = this.MagentoServiceLowLevelSoap.ToJson();
			var str = $"{{MethodName:{memberName}, ConnectionInfo:{connectionInfo}, MethodParameters:{methodParameters}, Mark:\"{mark}\"{( string.IsNullOrWhiteSpace( errors ) ? string.Empty : ", Errors:" + errors )}{( string.IsNullOrWhiteSpace( methodResult ) ? string.Empty : ", Result:" + methodResult )}{( string.IsNullOrWhiteSpace( notes ) ? string.Empty : ", Notes:" + notes )}{( string.IsNullOrWhiteSpace( additionalInfo ) ? string.Empty : ", " + additionalInfo )}}}";
			return str;
		}

		private static List< Tuple< DateTime, DateTime > > SplitToDates( DateTime dateFromUtc, DateTime dateToUtc, TimeSpan interval, TimeSpan intervalOverlapping )
		{
			var dates = new List< Tuple< DateTime, DateTime > >();
			if( dateFromUtc > dateToUtc )
				return dates;
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

		private async Task< IEnumerable< Product > > GetProductsBySoap( IMagentoServiceLowLevelSoap magentoServiceLowLevelSoap, bool includeDetails, string productType, bool productTypeShouldBeExcluded )
		{
			const int stockItemsListMaxChunkSize = 1000;
			IEnumerable< Product > resultProducts = new List< Product >();
			var catalogProductListResponse = await magentoServiceLowLevelSoap.GetProductsAsync( productType, productTypeShouldBeExcluded ).ConfigureAwait( false );

			if( catalogProductListResponse?.Products == null )
				return resultProducts;

			var products = catalogProductListResponse.Products.ToList();

			var productsDevidedByChunks = products.Batch( stockItemsListMaxChunkSize );

			// this code works to solw on 1 core server (but seems faster on multicore)
			//var getStockItemsAsyncTasks = productsDevidedByChunks.Select( stockItemsChunk => this.MagentoServiceLowLevelSoap.GetStockItemsAsync( stockItemsChunk.Select( x => x.sku ).ToList() ) );
			//var stockItemsResponses = await Task.WhenAll(getStockItemsAsyncTasks).ConfigureAwait(false);
			//if (stockItemsResponses == null || !stockItemsResponses.Any())
			//	return Enumerable.Empty<Product>();
			//var stockItems = stockItemsResponses.Where(x => x != null && x.result != null).SelectMany(x => x.result).ToList();

			// this code works faster on 1 core machine 
			var getStockItemsAsync = new List< InventoryStockItem >();
			foreach( var productsDevidedByChunk in productsDevidedByChunks )
			{
				var catalogInventoryStockItemListResponse = await magentoServiceLowLevelSoap.GetStockItemsAsync( productsDevidedByChunk.Select( x => x.Sku ).ToList() ).ConfigureAwait( false );
				getStockItemsAsync.AddRange( catalogInventoryStockItemListResponse.InventoryStockItems.ToList() );
			}
			var stockItems = getStockItemsAsync.ToList();

			resultProducts = ( from stockItemEntity in stockItems join productEntity in products on stockItemEntity.ProductId equals productEntity.ProductId select new Product( stockItemEntity.ProductId, productEntity.ProductId, productEntity.Name, productEntity.Sku, stockItemEntity.Qty, 0, null, productEntity.Type) ).ToList();

			if( includeDetails )
				resultProducts = ( await magentoServiceLowLevelSoap.FillProductDetails( resultProducts.Select( x => new ProductDetails( x ) ) ).ConfigureAwait( false ) ).Where( x => x != null ).Select( y => y.ToProduct() );
			return resultProducts;
		}

		private async Task< IEnumerable< Product > > GetProductsByRest()
		{
			// this code doesn't work for magento 1.8.0.1 http://www.magentocommerce.com/bug-tracking/issue/index/id/130
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

			IEnumerable< Product > resultProducts = ( from stockItem in stockItems join product in products on stockItem.ProductId equals product.EntityId select new Product( stockItem.ProductId, stockItem.EntityId, product.Name, product.Sku, stockItem.Qty, product.Price, product.Description, product.ProductType ) ).ToList();
			return resultProducts;
		}

		private async Task< string > UpdateStockItemsBySoapByThePiece( IList< Inventory > inventories, Mark mark )
		{
			var productToUpdate = inventories.Select( x => new PutStockItem( x ) ).ToList();

			var batchResponses = await productToUpdate.ProcessInBatchAsync( 5, async x => new Tuple< bool, List< PutStockItem > >( await this.MagentoServiceLowLevelSoap.PutStockItemAsync( x, mark ).ConfigureAwait( false ), new List< PutStockItem > { x } ) );

			var updateBriefInfo = batchResponses.Where( x => x.Item1 ).SelectMany( y => y.Item2 ).ToJson();

			var notUpdatedProducts = batchResponses.Where( x => !x.Item1 ).SelectMany( y => y.Item2 );

			var notUpdatedBriefInfo = notUpdatedProducts.ToJson();

			if( notUpdatedProducts.Any() )
				throw new Exception( $"Not updated {notUpdatedBriefInfo}" );

			return updateBriefInfo;
		}

		private async Task< IEnumerable< Product > > GetRestProductsAsync()
		{
			var page = 1;
			const int itemsPerPage = 100;

			var getProductsResponse = await this.MagentoServiceLowLevelRest.GetProductsAsync( page, itemsPerPage ).ConfigureAwait( false );

			var productsChunk = getProductsResponse.Products;
			if( productsChunk.Count() < itemsPerPage )
				return productsChunk.Select( x => new Product( null, x.EntityId, x.Name, x.Sku, null, x.Price, x.Description, null ) );

			var receivedProducts = new List< Models.Services.Rest.GetProducts.Product >();

			var lastReceiveProducts = productsChunk;

			bool isLastAndCurrentResponsesHaveTheSameProducts;

			do
			{
				receivedProducts.AddRange( productsChunk );

				var getProductsTask = this.MagentoServiceLowLevelRest.GetProductsAsync( ++page, itemsPerPage );
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

			return receivedProducts.Select( x => new Product( null, x.EntityId, x.Name, x.Sku, null, x.Price, x.Description, null ) );
		}

		private async Task< IEnumerable< Product > > GetRestProductsAsyncPparallel()
		{
			var page = 1;
			const int itemsPerPage = 100;

			var getProductsResponse = await this.MagentoServiceLowLevelRest.GetProductsAsync( page, itemsPerPage ).ConfigureAwait( false );

			var productsChunk = getProductsResponse.Products;
			if( productsChunk.Count() < itemsPerPage )
				return productsChunk.Select( x => new Product( null, x.EntityId, x.Name, x.Sku, null, x.Price, x.Description, null ) );

			var receivedProducts = new List< Models.Services.Rest.GetProducts.Product >();

			var lastReceiveProducts = productsChunk;

			receivedProducts.AddRange( productsChunk );

			var getProductsTasks = new List< Task< List< Models.Services.Rest.GetProducts.Product > > > {
				Task.Factory.StartNew( () => this.GetRestProducts( lastReceiveProducts, itemsPerPage, ref page ) ),
				Task.Factory.StartNew( () => this.GetRestProducts( lastReceiveProducts, itemsPerPage, ref page ) ),
				Task.Factory.StartNew( () => this.GetRestProducts( lastReceiveProducts, itemsPerPage, ref page ) ),
				Task.Factory.StartNew( () => this.GetRestProducts( lastReceiveProducts, itemsPerPage, ref page ) ) };


			await Task.WhenAll( getProductsTasks ).ConfigureAwait( false );

			var results = getProductsTasks.SelectMany( x => x.Result ).ToList();
			receivedProducts.AddRange( results );
			receivedProducts = receivedProducts.Distinct( new ProductComparer() ).ToList();

			return receivedProducts.Select( x => new Product( null, x.EntityId, x.Name, x.Sku, null, x.Price, x.Description, null ) );
		}

		private List< Models.Services.Rest.GetProducts.Product > GetRestProducts( IEnumerable< Models.Services.Rest.GetProducts.Product > lastReceiveProducts, int itemsPerPage, ref int page )
		{
			var localIsLastAndCurrentResponsesHaveTheSameProducts = true;
			var localLastReceivedProducts = lastReceiveProducts;
			var localReceivedProducts = new List< Models.Services.Rest.GetProducts.Product >();
			do
			{
				Interlocked.Increment( ref page );

				var getProductsTask = this.MagentoServiceLowLevelRest.GetProductsAsync( page, itemsPerPage );
				getProductsTask.Wait();
				var localProductsChunk = getProductsTask.Result.Products;

				var lastProductsChunksList = localProductsChunk as IList< Models.Services.Rest.GetProducts.Product > ?? localProductsChunk.ToList();
				var repeatedItems = from c in lastProductsChunksList join l in localLastReceivedProducts on c.EntityId equals l.EntityId select l;

				localLastReceivedProducts = lastProductsChunksList;

				var repeatedItemsList = repeatedItems as IList< Models.Services.Rest.GetProducts.Product > ?? repeatedItems.ToList();
				localIsLastAndCurrentResponsesHaveTheSameProducts = repeatedItemsList.Any();

				// try to get items that was added before last iteration
				if( localIsLastAndCurrentResponsesHaveTheSameProducts )
				{
					var notRrepeatedItems = lastProductsChunksList.Where( x => !repeatedItemsList.Exists( r => r.EntityId == x.EntityId ) );
					localReceivedProducts.AddRange( notRrepeatedItems );
				}
				else
					localReceivedProducts.AddRange( lastProductsChunksList );
			} while( !localIsLastAndCurrentResponsesHaveTheSameProducts );

			return localReceivedProducts;
		}

		private async Task< IEnumerable< Product > > GetRestStockItemsAsync()
		{
			var page = 1;
			const int itemsPerPage = 100;

			var getProductsResponse = await this.MagentoServiceLowLevelRest.GetStockItemsAsync( page, itemsPerPage ).ConfigureAwait( false );

			var productsChunk = getProductsResponse.Items;
			if( productsChunk.Count() < itemsPerPage )
				return productsChunk.Select( x => new Product( null, x.ItemId, null, null, null, 0, null, null ) );

			var receivedProducts = new List< StockItem >();

			var lastReceiveProducts = productsChunk;

			bool isLastAndCurrentResponsesHaveTheSameProducts;

			do
			{
				receivedProducts.AddRange( productsChunk );

				var getProductsTask = this.MagentoServiceLowLevelRest.GetStockItemsAsync( ++page, itemsPerPage );
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

			return receivedProducts.Select( x => new Product( x.ProductId, x.ItemId, null, null, x.Qty, 0, "", null ) );
		}

		private async Task< string > UpdateStockItemsByRest( IList< Inventory > inventories, string markForLog = "" )
		{
			const int productsUpdateMaxChunkSize = 50;
			var inventoryItems = inventories.Select( x => new Models.Services.Rest.PutStockItems.StockItem
			{
				ItemId = x.ItemId,
				MinQty = x.MinQty,
				ProductId = x.ProductId,
				Qty = x.Qty,
				StockId = x.StockId,
			} ).ToList();

			var productsDevidedToChunks = inventoryItems.SplitToChunks( productsUpdateMaxChunkSize );

			var batchResponses = await productsDevidedToChunks.ProcessInBatchAsync( 1, async x => await this.MagentoServiceLowLevelRest.PutStockItemsAsync( x, markForLog ).ConfigureAwait( false ) ).ConfigureAwait( false );

			var updateResult = batchResponses.Where( y => y.Items != null ).SelectMany( x => x.Items ).ToList();

			var secessefullyUpdated = updateResult.Where( x => x.Code == "200" );

			var unSecessefullyUpdated = updateResult.Where( x => x.Code != "200" );

			var updateBriefInfo = updateResult.ToJson();

			if( unSecessefullyUpdated.Any() )
				throw new Exception( $"Not updated: {unSecessefullyUpdated.ToJson()}, Updated: {secessefullyUpdated.ToJson()}" );

			return updateBriefInfo;
		}

		private async Task< string > UpdateStockItemsBySoap( IList< Inventory > inventories, IMagentoServiceLowLevelSoap magentoService, Mark markForLog = null )
		{
			const int productsUpdateMaxChunkSize = 50;
			var productToUpdate = inventories.Select( x => new PutStockItem( x ) ).ToList();

			var productsDevidedToChunks = productToUpdate.SplitToChunks( productsUpdateMaxChunkSize );

			var batchResponses = await productsDevidedToChunks.ProcessInBatchAsync( 1, async x => new Tuple< bool, List< PutStockItem > >( await magentoService.PutStockItemsAsync( x, markForLog ).ConfigureAwait( false ), x ) );

			var updateBriefInfo = batchResponses.Where( x => x.Item1 ).SelectMany( y => y.Item2 ).ToJson();

			var notUpdatedProducts = batchResponses.Where( x => !x.Item1 ).SelectMany( y => y.Item2 );

			var notUpdatedBriefInfo = notUpdatedProducts.ToJson();

			if( notUpdatedProducts.Any() )
				throw new Exception( $"Not updated {notUpdatedBriefInfo}" );

			return updateBriefInfo;
		}
		#endregion
	}

	public class MagentoConfig
	{
		public string VersionByDefault{ get; set; }
		public string EditionByDefault{ get; set; }
	}

	public class ProductAttributeCodes
	{
		public const string Upc = "upc";
		public const string Cost = "cost";
		public const string Manufacturer = "manufacturer";
	}

	internal class ProductComparer : IEqualityComparer< Models.Services.Rest.GetProducts.Product >
	{
		public bool Equals( Models.Services.Rest.GetProducts.Product x, Models.Services.Rest.GetProducts.Product y )
		{
			return x.EntityId == y.EntityId;
		}

		public int GetHashCode( Models.Services.Rest.GetProducts.Product obj )
		{
			return obj.EntityId.GetHashCode();
		}
	}

	internal class SalesOrderByOrderIdComparer : IEqualityComparer< Models.Services.Soap.GetOrders.Order >
	{
		public bool Equals( Models.Services.Soap.GetOrders.Order x, Models.Services.Soap.GetOrders.Order y )
		{
			return x.incrementId == y.incrementId && x.OrderId == y.OrderId;
		}

		public int GetHashCode( Models.Services.Soap.GetOrders.Order obj )
		{
			return obj.OrderId.GetHashCode() ^ obj.incrementId.GetHashCode();
		}
	}
}