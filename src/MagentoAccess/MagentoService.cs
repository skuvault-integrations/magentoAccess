using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using MagentoAccess.Misc;
using MagentoAccess.Models.CreateOrders;
using MagentoAccess.Models.CreateProducts;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.DeleteProducts;
using MagentoAccess.Models.GetMagentoCoreInfo;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Models.GetShipments;
using MagentoAccess.Services;
using MagentoAccess.Services.Rest.v2x;
using MagentoAccess.Services.Soap;
using MagentoAccess.Services.Soap._1_14_1_0_ee;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce_Zoey;
using MagentoAccess.Services.Soap._1_9_2_1_ce;
using MagentoAccess.Services.Soap._2_0_2_0_ce;
using MagentoAccess.Services.Soap._2_1_0_0_ce;
using Netco.Extensions;
using Netco.Logging;
using BillingAddress = MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository.BillingAddress;
using Order = MagentoAccess.Models.GetOrders.Order;
using Payment = MagentoAccess.Models.Services.Soap.GetOrders.Payment;

namespace MagentoAccess
{
	public class MagentoService: IMagentoService
	{
		public bool UseSoapOnly{ get; set; }
		internal virtual IMagentoServiceLowLevelSoap MagentoServiceLowLevelSoap{ get; set; }
		internal MagentoServiceLowLevelSoapFactory MagentoServiceLowLevelSoapFactory{ get; set; }

		public delegate void SaveAccessToken( string token, string secret );

		public SaveAccessToken AfterGettingToken{ get; set; }
		public Func< string > AdditionalLogInfo{ get; set; }
		public MagentoConfig Config{ get; set; }
		private MagentoAuthenticatedUserCredentials Credentials { get; set; }
		public const string UserAgentHeader = "SkuVault MagentoAccessLibrary C#";

		public async Task< IEnumerable< CreateProductModelResult > > CreateProductAsync( IEnumerable< CreateProductModel > models, CancellationToken token )
		{
			var methodParameters = models.ToJson();
			var mark = Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

				var pingres = await this.PingSoapAsync( token ).ConfigureAwait( false );
				//crunch for old versions
				var magentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.StoreVersion, true, false );

				var productsCreationInfo = await models.ProcessInBatchAsync( 30, async x =>
				{
					MagentoLogger.LogTrace( $"CreatingProduct: {this.CreateMethodCallInfo( mark : mark, methodParameters : x.ToJson() )}" );

					var res = new CreateProductModelResult( x );
					res.Result = await magentoServiceLowLevelSoap.CreateProduct( x.StoreId, x.Name, x.Sku, x.IsInStock, x.ProductType, token ).ConfigureAwait( false );

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

		public async Task< IEnumerable< CreateOrderModelResult > > CreateOrderAsync( IEnumerable< CreateOrderModel > models, CancellationToken token )
		{
			var methodParameters = models.ToJson();
			var mark = Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

				var pingres = await this.PingSoapAsync( token ).ConfigureAwait( false );
				//crunch for old versions
				var magentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.StoreVersion, true, false );

				var productsCreationInfo = await models.ProcessInBatchAsync( 30, async x =>
				{
					MagentoLogger.LogTrace( $"CreatingOrder: {this.CreateMethodCallInfo( mark : mark, methodParameters : x.ToJson() )}" );

					var res = new CreateOrderModelResult( x );

					var shoppingCartIdTask = magentoServiceLowLevelSoap.CreateCart( x.StoreId, token );
					shoppingCartIdTask.Wait();
					var _shoppingCartId = shoppingCartIdTask.Result;

					var shoppingCartCustomerSetTask = magentoServiceLowLevelSoap.ShoppingCartGuestCustomerSet( _shoppingCartId, x.CustomerFirstName, x.CustomerMail, x.CustomerLastName, x.StoreId, token );
					shoppingCartCustomerSetTask.Wait();

					var shoppingCartAddressSet = magentoServiceLowLevelSoap.ShoppingCartAddressSet( _shoppingCartId, x.StoreId, token );
					shoppingCartAddressSet.Wait();

					var productTask = magentoServiceLowLevelSoap.ShoppingCartAddProduct( _shoppingCartId, x.ProductIds.First(), x.StoreId, token );
					productTask.Wait();

					var shippingMenthodTask = magentoServiceLowLevelSoap.ShoppingCartSetShippingMethod( _shoppingCartId, x.StoreId, token );
					shippingMenthodTask.Wait();

					var paymentMenthodTask = magentoServiceLowLevelSoap.ShoppingCartSetPaymentMethod( _shoppingCartId, x.StoreId, token );
					paymentMenthodTask.Wait();

					var orderIdTask = magentoServiceLowLevelSoap.CreateOrder( _shoppingCartId, x.StoreId, token );
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

		public async Task< IEnumerable< DeleteProductModelResult > > DeleteProductAsync( IEnumerable< DeleteProductModel > models, CancellationToken token )
		{
			var methodParameters = models.ToJson();
			var mark = Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

				var pingres = await this.PingSoapAsync( token ).ConfigureAwait( false );
				//crunch for old versions
				var magentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.StoreVersion, true, false );

				var productsCreationInfo = await models.ProcessInBatchAsync( 30, async x =>
				{
					MagentoLogger.LogTrace( $"DeleteProduct: {this.CreateMethodCallInfo( mark : mark, methodParameters : x.ToJson() )}" );

					var res = new DeleteProductModelResult( x );
					res.Result = await magentoServiceLowLevelSoap.DeleteProduct( x.StoreId, x.CategoryId, x.ProductId, x.IdentiferType, token ).ConfigureAwait( false );

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
		public MagentoService( MagentoAuthenticatedUserCredentials magentoAuthenticatedUserCredentials, MagentoConfig magentoConfig, MagentoTimeouts operationsTimeouts )
		{
			ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			this.Config = magentoConfig;
			this.Credentials = magentoAuthenticatedUserCredentials;

			//all methods should use factory, but it takes time to convert them, since there are a lot of errors in magento which we should avoid
			var lowLevelServicesList = new Dictionary< string, IMagentoServiceLowLevelSoap >();

			var cfg = this.Config.DefaultIfNull();
			switch( cfg.Protocol )
			{
				case MagentoDefaultProtocol.RestOnly:
					lowLevelServicesList.Add( MagentoVersions.MR_2_0_0_0, new MagentoServiceLowLevelSoap_v_r_2_0_0_0_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
					break;
				case MagentoDefaultProtocol.SoapOnly:
					lowLevelServicesList.Add( MagentoVersions.M_2_0_2_0, new MagentoServiceLowLevelSoap_v_2_0_2_0_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
					lowLevelServicesList.Add( MagentoVersions.M_2_1_0_0, new MagentoServiceLowLevelSoap_v_2_1_0_0_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
					break;
				case MagentoDefaultProtocol.Default:
				default:
					lowLevelServicesList.Add( MagentoVersions.MR_2_0_0_0, new MagentoServiceLowLevelSoap_v_r_2_0_0_0_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
					lowLevelServicesList.Add( MagentoVersions.M_2_0_2_0, new MagentoServiceLowLevelSoap_v_2_0_2_0_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
					lowLevelServicesList.Add( MagentoVersions.M_2_1_0_0, new MagentoServiceLowLevelSoap_v_2_1_0_0_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
					break;
			}

			lowLevelServicesList.Add( MagentoVersions.M_1_9_2_0, new MagentoServiceLowLevelSoap_v_1_9_2_1_ce_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
			lowLevelServicesList.Add( MagentoVersions.M_1_9_0_1, new MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
			lowLevelServicesList.Add( MagentoVersions.M_1_8_1_0, new MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
			lowLevelServicesList.Add( MagentoVersions.M_1_7_0_2, new MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
			lowLevelServicesList.Add( MagentoVersions.M_1_14_1_0, new MagentoServiceLowLevelSoap_v_1_14_1_0_EE_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );
			lowLevelServicesList.Add( MagentoVersions.ZS_1_7_0_1, new ZoeyServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory().CreateMagentoLowLevelService( magentoAuthenticatedUserCredentials, cfg, operationsTimeouts ) );

			if( cfg.UseVersionByDefaultOnly && !string.IsNullOrWhiteSpace( cfg.VersionByDefault ) )
			{
				lowLevelServicesList = lowLevelServicesList.Where( x => string.Equals( x.Key, cfg.VersionByDefault, StringComparison.OrdinalIgnoreCase ) ).ToDictionary( x => x.Key, y => y.Value );
			}

			this.MagentoServiceLowLevelSoapFactory = new MagentoServiceLowLevelSoapFactory( magentoAuthenticatedUserCredentials, lowLevelServicesList, cfg, operationsTimeouts );
			var defaultVersion = !string.IsNullOrWhiteSpace( magentoConfig?.VersionByDefault ) ? magentoConfig.VersionByDefault : MagentoVersions.M_1_7_0_2;
			this.MagentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( defaultVersion, true, false );
		}
		#endregion

		#region ping
		public async Task< PingSoapInfo > DetermineMagentoVersionAndSetupServiceAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark ?? Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );

				var soapInfo = new PingSoapInfo( string.Empty, string.Empty, false, String.Empty );
				var soapInfos = await this.DetermineMagentoVersionAsync( token, mark ).ConfigureAwait( false );
				var pingSoapInfos = soapInfos as IList< PingSoapInfo > ?? soapInfos.ToList();
				if( pingSoapInfos.Any() )
				{
					var temp = pingSoapInfos.First();
					if( temp != null )
					{
						soapInfo = temp;
						if( !string.IsNullOrWhiteSpace( soapInfo.StoreVersion ) )
							this.MagentoServiceLowLevelSoap = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( soapInfo.StoreVersion, true, false );
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

		public async Task< IEnumerable< PingSoapInfo > > DetermineMagentoVersionAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark ?? Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark ) );

				var magentoLowLevelServices = this.MagentoServiceLowLevelSoapFactory.GetAll();
				var storeVersionFromApi = await this.GetMagentoStoreVersionAsync( mark ).ConfigureAwait( false );

				// use Magento Rest API for version higher than 2.1+ or when version is not set (prerelease)
				if ( storeVersionFromApi != null 
					&& ( storeVersionFromApi.Version.Major == 2 && storeVersionFromApi.Version.Minor > 1
						|| storeVersionFromApi.Version == new Version( 1, 0 ) ) )
				{
					var restService = magentoLowLevelServices.FirstOrDefault( s => s.Key.Equals( MagentoVersions.MR_2_0_0_0 ) );
						
					if ( restService.Value != null )
					{
						if ( await restService.Value.InitAsync( true ).ConfigureAwait( false ) )
						{
							return new [] { new PingSoapInfo( storeVersionFromApi.Version.ToString(), storeVersionFromApi.MagentoEdition, true, MagentoVersions.MR_2_0_0_0 ) };
						}
					}
				}

				var legacyStoreVersions = await magentoLowLevelServices.ProcessInBatchAsync( 14, async kvp =>
				{
					if( !await kvp.Value.InitAsync( true ).ConfigureAwait( false ) )
						return null;
					var getMagentoInfoResponse = await kvp.Value.GetMagentoInfoAsync( true, token ).ConfigureAwait( false );
					if( getMagentoInfoResponse != null )
						getMagentoInfoResponse.ServiceVersion = kvp.Key;
					return getMagentoInfoResponse;
				} ).ConfigureAwait( false );
				var workingStores = legacyStoreVersions.Where( x => !string.IsNullOrWhiteSpace( x?.MagentoEdition ) && !string.IsNullOrWhiteSpace( x.MagentoVersion ) );
				var pingSoapInfos = workingStores.Select( x => new PingSoapInfo( x.MagentoVersion, x.MagentoEdition, true, x.ServiceVersion ) );

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : pingSoapInfos.ToJson() ) );

				return pingSoapInfos;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : mark ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		/// <summary>
		///	Detects Magento store version using url http(s)://magento_store_base_url/magento_version.
		///	This feature works only for Magento 2.0+
		/// </summary>
		/// <param name="mark"></param>
		/// <returns>store version if the correspondent point is accessible otherwise null</returns>
		private async Task< MagentoStoreVersion > GetMagentoStoreVersionAsync( Mark mark = null )
		{
			mark = mark ?? Mark.CreateNew();

			try
			{
				using( var httpClient = CreateHttpClient() )
				{
					string additionalInfo = "headers: " + httpClient.DefaultRequestHeaders.ToJson();
					MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo: additionalInfo ) );
					var storeVersionRaw = await httpClient.GetStringAsync( "magento_version" ).ConfigureAwait( false );
					MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : mark, methodResult : storeVersionRaw ) );

					if ( !string.IsNullOrEmpty( storeVersionRaw ) )
						return storeVersionRaw.ParseMagentoStoreInfoString();
				}
			}
			catch ( Exception )
			{
				// only Magento 2.0+ supports this feature
			}

			return null;
		}

		private HttpClient CreateHttpClient()
		{
			var httpClient = new HttpClient
			{
				BaseAddress = new Uri( this.Credentials.BaseMagentoUrl )
			};
			httpClient.DefaultRequestHeaders.TryAddWithoutValidation( "User-Agent", UserAgentHeader );
			return httpClient;
		}

		public async Task< PingSoapInfo > PingSoapAsync( CancellationToken token, Mark mark = null )
		{
			var markLocal = mark ?? Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo(), markLocal );
				var magentoInfo = await this.MagentoServiceLowLevelSoap.GetMagentoInfoAsync( false, token, markLocal ).ConfigureAwait( false );
				var soapWorks = !string.IsNullOrWhiteSpace( magentoInfo.MagentoVersion ) || !string.IsNullOrWhiteSpace( magentoInfo.MagentoEdition );

				var magentoCoreInfo = new PingSoapInfo( magentoInfo.MagentoVersion, magentoInfo.MagentoEdition, soapWorks, magentoInfo.ServiceVersion );
				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( methodResult : magentoCoreInfo.ToJson() ), markLocal );

				return magentoCoreInfo;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : markLocal ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}
		#endregion

		#region getOrders
		public async Task< IEnumerable< Order > > GetOrdersAsync( IEnumerable< string > orderIds, CancellationToken token )
		{
			var methodParameters = orderIds.ToJson();

			var mark = Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark ) );

				IMagentoServiceLowLevelSoap magentoServiceLowLevelSoap;
				var pingres = await this.PingSoapAsync( token ).ConfigureAwait( false );
				//crunch for old versions
				magentoServiceLowLevelSoap = string.Equals( pingres.StoreEdition, MagentoVersions.M_1_7_0_2, StringComparison.CurrentCultureIgnoreCase )
				                             || string.Equals( pingres.StoreEdition, MagentoVersions.M_1_8_1_0, StringComparison.CurrentCultureIgnoreCase )
				                             || string.Equals( pingres.StoreEdition, MagentoVersions.M_1_9_0_1, StringComparison.CurrentCultureIgnoreCase )
				                             || string.Equals( pingres.StoreEdition, MagentoVersions.M_1_14_1_0, StringComparison.CurrentCultureIgnoreCase ) ? this.MagentoServiceLowLevelSoap : this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.StoreVersion, true, false );

				var salesOrderInfoResponses = await orderIds.ProcessInBatchAsync( 16, async x =>
				{
					MagentoLogger.LogTrace( $"OrderRequested: {this.CreateMethodCallInfo( mark : mark, methodParameters : x )}" );
					var res = await magentoServiceLowLevelSoap.GetOrderAsync( x, token, mark ).ConfigureAwait( false );
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

		public async Task< IEnumerable< Order > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo, CancellationToken token, Mark mark = null )
		{
			var dateFromUtc = TimeZoneInfo.ConvertTimeToUtc( dateFrom );
			var dateToUtc = TimeZoneInfo.ConvertTimeToUtc( dateTo );
			var methodParameters = $"{{dateFrom:{dateFromUtc},dateTo:{dateToUtc}}}";

			mark = mark ?? Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters ), mark );

				var interval = new TimeSpan( 7, 0, 0, 0 );
				var intervalOverlapping = new TimeSpan( 0, 0, 0, 1 );

				var dates = SplitToDates( dateFromUtc, dateToUtc, interval, intervalOverlapping );

				var pingres = await this.PingSoapAsync( token, Mark.CreateNew( mark ) ).ConfigureAwait( false );
				//crunch for old versions
				var magentoServiceLowLevelSoap = string.Equals( pingres.StoreEdition, MagentoVersions.M_1_7_0_2, StringComparison.CurrentCultureIgnoreCase )
				                                 || string.Equals( pingres.StoreEdition, MagentoVersions.M_1_8_1_0, StringComparison.CurrentCultureIgnoreCase )
				                                 || string.Equals( pingres.StoreEdition, MagentoVersions.M_1_9_0_1, StringComparison.CurrentCultureIgnoreCase )
				                                 || string.Equals( pingres.StoreEdition, MagentoVersions.M_1_14_1_0, StringComparison.CurrentCultureIgnoreCase ) 
												 || this.Config.UseVersionByDefaultOnly
					? this.MagentoServiceLowLevelSoap 
					: this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.StoreVersion, true, false );
				var ordersBriefInfos = await dates.ProcessInBatchAsync( 30, async x =>
				{
					var atomicMark = Mark.CreateNew( mark );
					MagentoLogger.LogTrace( $"OrdersRequested: {this.CreateMethodCallInfo( methodParameters : $"{x.Item1},{x.Item2}" )}", atomicMark );

					var res = await magentoServiceLowLevelSoap.GetOrdersAsync( x.Item1, x.Item2, token, atomicMark ).ConfigureAwait( false );

					MagentoLogger.LogTrace( $"OrdersReceived: {this.CreateMethodCallInfo( methodResult : res.ToJson(), methodParameters : $"{x.Item1},{x.Item2}" )}", atomicMark );
					return res;
				} ).ConfigureAwait( false );

				var orders = ordersBriefInfos.Where( x => x?.Orders != null ).SelectMany( x => x.Orders ).Distinct( new SalesOrderByOrderIdComparer() ).ToList();

				var ordersBriefInfoString = orders.ToJson();

				MagentoLogger.LogTrace( this.CreateMethodCallInfo( methodParameters : methodParameters, notes : "BriefOrdersReceived:\"{0}\"".FormatWith( ordersBriefInfoString ) ), mark );

				IEnumerable< OrderInfoResponse > salesOrderInfoResponses;
				if( this.MagentoServiceLowLevelSoap.GetOrderByIdForFullInformation )
				{
					salesOrderInfoResponses = await orders.ProcessInBatchAsync( 16, async x =>
					{
						var childMark = Mark.CreateNew( mark );
						MagentoLogger.LogTrace( $"OrderRequested: {this.CreateMethodCallInfo( methodParameters : x.ToStringIds() )}", childMark );
						var res = await magentoServiceLowLevelSoap.GetOrderAsync( x, token, childMark ).ConfigureAwait( false );
						MagentoLogger.LogTrace( $"OrderReceived: {this.CreateMethodCallInfo( methodResult : res.ToJson(), methodParameters : x.ToStringIds() )}", childMark );
						return res;
					} ).ConfigureAwait( false );
				}
				else
				{
					salesOrderInfoResponses = orders.Select( Mapper.Map< OrderInfoResponse > ).ToList();
				}

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
					MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( methodResult : resultOrdersBriefInfo, methodParameters : methodParameters, notes : "LogPart:\"{0}\"".FormatWith( partDescription ) ), mark );
				}

				return resultOrders;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( methodParameters : methodParameters ), exception, mark : mark );
				MagentoLogger.LogTraceException( mexc, mark );
				throw mexc;
			}
		}
		#endregion

		#region getShipments
		public async Task< Dictionary< string, IEnumerable< Shipment > > > GetOrdersShipmentsAsync( DateTime modifiedFrom, DateTime modifiedTo, CancellationToken token, Mark mark = null )
		{
			var modifiedDateFromUtc = TimeZoneInfo.ConvertTimeToUtc( modifiedFrom );
			var modifiedDateToToUtc = TimeZoneInfo.ConvertTimeToUtc( modifiedTo );
			var methodParameters = $"{{modifiedDateFrom:{modifiedDateFromUtc},modifiedDateTo:{modifiedDateToToUtc}}}";

			mark = mark ?? Mark.CreateNew();

			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters ), mark );
				
				var pingres = await this.PingSoapAsync( token, mark ).ConfigureAwait( false );
				var magentoServiceLowLevel = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.ServiceUsedVersion, true, false );
				var shipments = await magentoServiceLowLevel.GetOrdersShipmentsAsync( modifiedDateFromUtc, modifiedDateToToUtc, mark ).ConfigureAwait( false );

				var shipmentsSummary = $"Count:{shipments.Count()},Shipments:{shipments.ToJson()}";
				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( methodResult : shipmentsSummary ), mark );

				return shipments;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( methodParameters : methodParameters ), exception, mark : mark );
				MagentoLogger.LogTraceException( mexc, mark );
				throw mexc;
			}
		}
		#endregion

		#region getProducts
		public async Task< IEnumerable< Product > > GetProductsAsync( CancellationToken token, IEnumerable< int > scopes = null, bool includeDetails = false, string productType = null, bool excludeProductByType = false, DateTime? updatedFrom = null, IEnumerable< string > skus = null, bool stockItemsOnly = true, Mark mark = null )
		{
			var markLocal = mark ?? Mark.CreateNew();
			var parameters = $"includeDetails:{includeDetails},productType:{productType},excludeProductByType:{excludeProductByType},updatedFrom:{updatedFrom}";
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters : parameters ), markLocal );

				var pingres = await this.PingSoapAsync( token, markLocal ).ConfigureAwait( false );
				var magentoServiceLowLevel = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.ServiceUsedVersion, true, false );
				var resultProducts = await GetProductsViaSoapAsync( magentoServiceLowLevel, includeDetails, productType, excludeProductByType, scopes ?? new[] { 0, 1 }, updatedFrom, skus, stockItemsOnly, token, markLocal ).ConfigureAwait( false );
				var productBriefInfo = $"Count:{resultProducts.Count()},Product:{resultProducts.ToJson()}";
				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( methodResult : productBriefInfo ), markLocal );

				return resultProducts;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : markLocal ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task< IEnumerable< Product > > FillProductsDetailsAsync( IEnumerable< Product > products, CancellationToken token, Mark mark )
		{
			var markLocal = mark ?? Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : markLocal ) );

				var pingres = await this.PingSoapAsync( token ).ConfigureAwait( false );
				var magentoServiceLowLevel = this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.StoreVersion, true, false );

				IEnumerable< Product > resultProducts;
				var magentoServiceLowLevelFillProducts = magentoServiceLowLevel as IMagentoServiceLowLevelSoapFillProductsDetails;
				string productBriefInfo;
				if( magentoServiceLowLevelFillProducts != null )
				{
					resultProducts = ( await magentoServiceLowLevelFillProducts.FillProductDetails( products.Select( x => new ProductDetails( x ) ), token ).ConfigureAwait( false ) ).Select( y => y.ToProduct() );
					productBriefInfo = $"Count:{resultProducts.Count()},Product:{resultProducts.ToJson()}";
				}
				else
				{
					MagentoLogger.LogTrace( this.CreateMethodCallInfo( mark : markLocal, notes : "Current store version doesn't need fill product details. Return products as is." ) );
					resultProducts = products;
					productBriefInfo = $"Count:{products.Count()},Product:{products.ToJson()}";
				}
				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : markLocal, methodResult : productBriefInfo ) );

				return resultProducts;
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : markLocal ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}
		#endregion

		#region updateInventory
		public async Task UpdateInventoryAsync( IEnumerable< Inventory > products, CancellationToken token, Mark mark = null )
		{
			var productsBriefInfo = products.ToJson();
			var markLocal = mark ?? Mark.CreateNew();
			try
			{
				MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : markLocal, methodParameters : productsBriefInfo ) );

				var inventories = products as IList< Inventory > ?? products.ToList();
				var updateBriefInfo = PredefinedValues.NotAvailable;
				if( inventories.Any() )
				{
					var pingres = await this.PingSoapAsync( token ).ConfigureAwait( false );
					//crunch for 1702
					updateBriefInfo = string.Equals( pingres.StoreVersion, MagentoVersions.M_1_7_0_2, StringComparison.CurrentCultureIgnoreCase )
						? await this.UpdateStockItemsBySoapByThePiece( inventories, token, markLocal ).ConfigureAwait( false )
						: await this.UpdateStockItemsBySoap( inventories, this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.StoreVersion, true, false ), token, markLocal ).ConfigureAwait( false );
				}

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( mark : markLocal, methodParameters : productsBriefInfo, methodResult : updateBriefInfo ) );
			}
			catch( Exception exception )
			{
				var mexc = new MagentoCommonException( this.CreateMethodCallInfo( mark : markLocal ), exception );
				MagentoLogger.LogTraceException( mexc );
				throw mexc;
			}
		}

		public async Task UpdateInventoryBySkuAsync( IEnumerable< InventoryBySku > inventory, CancellationToken token, IEnumerable< int > scopes = null )
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
						var pingres = await this.PingSoapAsync( token ).ConfigureAwait( false );
						var magentoServiceLowLevelSoap = string.Equals( pingres.StoreEdition, MagentoVersions.M_1_7_0_2, StringComparison.CurrentCultureIgnoreCase )
						                                 || string.Equals( pingres.StoreEdition, MagentoVersions.M_1_8_1_0, StringComparison.CurrentCultureIgnoreCase )
						                                 || string.Equals( pingres.StoreEdition, MagentoVersions.M_1_9_0_1, StringComparison.CurrentCultureIgnoreCase )
						                                 || string.Equals( pingres.StoreEdition, MagentoVersions.M_1_14_1_0, StringComparison.CurrentCultureIgnoreCase ) ? this.MagentoServiceLowLevelSoap : this.MagentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( pingres.StoreVersion, true, false );

						var stockitems = await magentoServiceLowLevelSoap.GetStockItemsAsync( inventory.Select( x => x.Sku ).ToList(), scopes ?? new[] { 0, 1 }, token ).ConfigureAwait( false );
						var productsWithSkuQtyId = from i in inventory join s in stockitems.InventoryStockItems on i.Sku equals s.Sku select new Inventory() { ItemId = s.ProductId, ProductId = s.ProductId, Qty = i.Qty };
						await this.UpdateInventoryAsync( productsWithSkuQtyId, token ).ConfigureAwait( false );
					}
					else
					{
						var products = await this.GetProductsAsync( token, scopes ?? new[] { 0, 1 } ).ConfigureAwait( false );
						var productsWithSkuQtyId = from i in inventory join s in products on i.Sku equals s.Sku select new Inventory { ItemId = s.ProductId, ProductId = s.ProductId, Qty = i.Qty, Sku = s.Sku };
						await this.UpdateInventoryAsync( productsWithSkuQtyId, token ).ConfigureAwait( false );
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

		#region MethodsImplementations
		private string CreateMethodCallInfo( string methodParameters = "", Mark mark = null, string errors = "", string methodResult = "", string additionalInfo = "", [ CallerMemberName ] string memberName = "", string notes = "" )
		{
			additionalInfo = ( string.IsNullOrWhiteSpace( additionalInfo ) && this.AdditionalLogInfo != null ) ? this.AdditionalLogInfo() : PredefinedValues.EmptyJsonObject;
			mark = mark ?? Mark.Blank();
			var connectionInfo = this.MagentoServiceLowLevelSoap.ToJson();
			var str = $"{{MethodName:{memberName}, ConnectionInfo:{connectionInfo}, MethodParameters:{methodParameters}, Mark:\"{mark.ToStringSafe()}\"{( string.IsNullOrWhiteSpace( errors ) ? string.Empty : ", Errors:" + errors )}{( string.IsNullOrWhiteSpace( methodResult ) ? string.Empty : ", Result:" + methodResult )}{( string.IsNullOrWhiteSpace( notes ) ? string.Empty : ", Notes:" + notes )}{( string.IsNullOrWhiteSpace( additionalInfo ) ? string.Empty : ", " + additionalInfo )}}}";
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

		private static async Task< IEnumerable< Product > > GetProductsViaSoapAsync( IMagentoServiceLowLevelSoap magentoServiceLowLevelSoap, bool includeDetails, string productType, bool productTypeShouldBeExcluded, IEnumerable< int > scopes, DateTime? updatedFrom, IEnumerable< string > skus, bool stockItemsOnly, CancellationToken cancellationToken, Mark mark = null )
		{
			const int stockItemsListMaxChunkSize = 1000;
			SoapGetProductsResponse catalogProductListResponse;
			if( skus != null && skus.Any() )
			{
				var magentoServiceLowLevelSoapGetProductsBySku = magentoServiceLowLevelSoap as IMagentoServiceLowLevelSoapGetProductsBySku;
				if( magentoServiceLowLevelSoapGetProductsBySku != null )
				{
					catalogProductListResponse = await magentoServiceLowLevelSoapGetProductsBySku.GetProductsAsync( productType, productTypeShouldBeExcluded, updatedFrom, skus as IReadOnlyCollection< string >, cancellationToken, mark ).ConfigureAwait( false );
				}
				else
				{
					catalogProductListResponse = new SoapGetProductsResponse
					{
						Products = await GetProductsBySkusViaRestAsync( magentoServiceLowLevelSoap, skus, cancellationToken, mark )
					};
				}
			}
			else
			{
				catalogProductListResponse = await magentoServiceLowLevelSoap.GetProductsAsync( productType, productTypeShouldBeExcluded, updatedFrom, cancellationToken, mark ).ConfigureAwait( false );
			}

			if( catalogProductListResponse?.Products == null || !catalogProductListResponse.Products.Any() )
				return new List< Product >();

			var products = catalogProductListResponse.Products.ToList();
			List< InventoryStockItem > stockItems;
			if( magentoServiceLowLevelSoap.GetStockItemsWithoutSkuImplementedWithPages )
			{
				var inventory = await magentoServiceLowLevelSoap.GetStockItemsWithoutSkuAsync( products.Select( x => x.Sku ), scopes, cancellationToken ).ConfigureAwait( false );
				stockItems = inventory.InventoryStockItems.ToList();
			}
			else
			{
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
					var catalogInventoryStockItemListResponse = await magentoServiceLowLevelSoap.GetStockItemsAsync( productsDevidedByChunk.Select( x => x.Sku ).ToList(), scopes, cancellationToken, mark ).ConfigureAwait( false );
					getStockItemsAsync.AddRange( catalogInventoryStockItemListResponse.InventoryStockItems.ToList() );
				}
				stockItems = getStockItemsAsync.ToList();
			}

			IEnumerable< Product > resultProducts;
			if( stockItemsOnly )
				resultProducts = ( from stockItemEntity in stockItems join productEntity in products on stockItemEntity.ProductId equals productEntity.ProductId select new Product( stockItemEntity.ProductId, productEntity.ProductId, productEntity.Name, productEntity.Sku, stockItemEntity.Qty, 0, null, productEntity.Type, productEntity.UpdatedAt, stockItemEntity.IsInStock == "True" ) );
			else
				resultProducts = ( from productEntity in products join stockItemEntity in stockItems on productEntity.ProductId equals stockItemEntity.ProductId into productsList from stockItemEntity in productsList.DefaultIfEmpty() select new Product( productEntity.ProductId, productEntity.ProductId, productEntity.Name, productEntity.Sku, stockItemEntity == null ? "0" : stockItemEntity.Qty, 0, null, productEntity.Type, productEntity.UpdatedAt, stockItemEntity == null ? false : stockItemEntity.IsInStock == "True" ) );
			resultProducts = resultProducts.Where( p => !string.IsNullOrWhiteSpace( p.Sku ) );

			if( includeDetails )
			{
				var fillService = ( magentoServiceLowLevelSoap as IMagentoServiceLowLevelSoapFillProductsDetails );
				if( fillService != null )
					resultProducts = ( await fillService.FillProductDetails( resultProducts.Select( x => new ProductDetails( x ) ), cancellationToken ).ConfigureAwait( false ) ).Where( x => x != null ).Select( y => y.ToProduct() );
			}
			return resultProducts;
		}

		private static async Task< IEnumerable< SoapProduct > > GetProductsBySkusViaRestAsync( IMagentoServiceLowLevelSoap magentoServiceLowLevelSoap, IEnumerable< string > skus, CancellationToken cancellationToken, Mark mark )
		{
			var products = await magentoServiceLowLevelSoap.GetProductsBySkusAsync( skus, cancellationToken, mark ).ConfigureAwait( false );
			if( products?.Products == null || !products.Products.Any() )
				return new List< SoapProduct >();

			return products.Products;
		}

		private async Task< string > UpdateStockItemsBySoapByThePiece( IList< Inventory > inventories, CancellationToken cancellationToken, Mark mark )
		{
			var productToUpdate = inventories.Select( x => new PutStockItem( x ) ).ToList();

			var batchResponses = await productToUpdate.ProcessInBatchAsync( 5, async x => new Tuple< bool, List< PutStockItem > >( await this.MagentoServiceLowLevelSoap.PutStockItemAsync( x, cancellationToken, mark ).ConfigureAwait( false ), new List< PutStockItem > { x } ) ).ConfigureAwait( false );

			var updateBriefInfo = batchResponses.Where( x => x.Item1 ).SelectMany( y => y.Item2 ).ToJson();

			var notUpdatedProducts = batchResponses.Where( x => !x.Item1 ).SelectMany( y => y.Item2 );

			var notUpdatedBriefInfo = notUpdatedProducts.ToJson();

			if( notUpdatedProducts.Any() )
				throw new Exception( $"Not updated {notUpdatedBriefInfo}" );

			return updateBriefInfo;
		}

		private async Task< string > UpdateStockItemsBySoap( IList< Inventory > inventories, IMagentoServiceLowLevelSoap magentoService, CancellationToken cancellationToken, Mark markForLog = null )
		{
			const int productsUpdateMaxChunkSize = 50;
			var productToUpdate = inventories.Select( x => new PutStockItem( x ) ).ToList();

			var productsDevidedToChunks = productToUpdate.SplitToChunks( productsUpdateMaxChunkSize );

			var batchResponses = await productsDevidedToChunks.ProcessInBatchAsync( 1, async x => new Tuple< IEnumerable< RpcInvoker.RpcRequestResponse< PutStockItem, object > >, List< PutStockItem > >( await magentoService.PutStockItemsAsync( x, cancellationToken, markForLog.CreateChildOrNull() ).ConfigureAwait( false ), x ) ).ConfigureAwait( false );

			var batchResponsesList = batchResponses as IList< Tuple< IEnumerable< RpcInvoker.RpcRequestResponse< PutStockItem, object > >, List< PutStockItem > > > ?? batchResponses.ToList();
			var updateBriefInfo = batchResponsesList.SelectMany( x => x.Item1 ).Where( y => y.Response.ErrorCode == RpcInvoker.SoapErrorCode.Success ).ToJson();

			var notUpdatedProducts = batchResponsesList.SelectMany( x => x.Item1 ).Where( y => y.Response.ErrorCode != RpcInvoker.SoapErrorCode.Success ).ToList();

			var notUpdatedBriefInfo = notUpdatedProducts.ToJson();

			var cfg = this.Config.DefaultIfNull();
			if( cfg.OnUpdateInventory == ThrowExceptionIfFailed.OneItem && notUpdatedProducts.Any() )
				throw new Exception( $"Not updated {notUpdatedBriefInfo}" );
			if( cfg.OnUpdateInventory == ThrowExceptionIfFailed.AllItems && notUpdatedProducts.Count == inventories.Count )
				throw new Exception( $"Not updated {notUpdatedBriefInfo}" );

			if( notUpdatedProducts.Any() )
				MagentoLogger.LogTrace( this.CreateMethodCallInfo( mark : markForLog, methodParameters : inventories.ToJson(), methodResult : notUpdatedProducts.ToJson(), notes : "Following items can't be updated" ) );

			return updateBriefInfo;
		}
		#endregion

		public async Task< bool > InitAsync( bool supressExc = false )
		{
			try
			{
				var initTask = this.MagentoServiceLowLevelSoap.InitAsync();
				//Mapper.Initialize( cfg => cfg.CreateMap< Models.Services.Soap.GetOrders.Order, OrderInfoResponse >() );

				Mapper.Initialize( cfg =>
				{
					cfg.CreateMap< int?, string >().ConvertUsing( Extensions.ToStringEmptyOnNull );
					cfg.CreateMap< string, string >().ConvertUsing( x => x ?? string.Empty );

					cfg.CreateMap< Models.Services.Soap.GetOrders.Order, OrderInfoResponse >();
					//cfg.AddConditionalObjectMapper().((s, d) => s.Name == d.Name + "Dto");
					//cfg.AddConditionalObjectMapper().Where((source, destination) => s.Name.Replace("(_)([a-z])","\U1") == d.Name );
					//cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
					//cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();

					//ReplaceValue(new Match(new Regex("(_)([a-z])"),0,));
					cfg.AddMemberConfiguration().AddName< ReplaceName >( _ => _.AddReplace( "_", "" ) );
					cfg.AddMemberConfiguration().AddName< CaseInsensitiveName >();
					//cfg.AddMemberConfiguration().

					cfg.CreateMap< Models.Services.Rest.v2x.CatalogStockItemRepository.StockItem, InventoryStockItem >();
					cfg.CreateMap< Item2, OrderItemEntity >();

					cfg.CreateMap< Models.Services.Soap.GetOrders.Order, OrderInfoResponse >()
						.ForMember( x => x.Payment, opt => opt.MapFrom( src => new Payment() { Method = src.PaymentMethod } ))
						//.ForMember(x => x.BillingAddress, opt => opt.MapFrom(src => new BillingAddress()
						//{
						//	lastname = src.ShippingLastname,
						//	firstname = src.ShippingFirstname,
						//}))
						//.ForMember( x => x.ShippingAddress, opt => opt.MapFrom( src => new ShippingAddress() {
						//	Lastname = src.ShippingLastname,
						//	Firstname = src.ShippingFirstname,
						//} ) )
						;

					cfg.CreateMap< Address, ShippingAddress >()
						.ForMember( x => x.Street, opt => opt.MapFrom( src => src.street != null ? string.Join( ", ", src.street ) : null ) ) //since soap returns null instead of "", can be changed in future.
						.ForMember( x => x.Region, opt => opt.MapFrom( src => string.IsNullOrWhiteSpace( src.region ) ? null : src.region ) ) //since soap returns null instead of "", can be changed in future.
						.ForMember( x => x.Company, opt => opt.MapFrom( src => string.IsNullOrWhiteSpace( src.company ) ? null : src.company ) ); //since soap returns null instead of "", can be changed in future.

					cfg.CreateMap< BillingAddress, Models.Services.Soap.GetOrders.BillingAddress >()
						.ForMember( x => x.Street, opt => opt.MapFrom( src => src.street != null ? string.Join( ", ", src.street ) : null ) )
						.ForMember( x => x.Company, opt => opt.MapFrom( src => string.IsNullOrWhiteSpace( src.company ) ? null : src.company ) ) //since soap returns null instead of "", can be changed in future.
						.ForMember( x => x.Region, opt => opt.MapFrom( src => string.IsNullOrWhiteSpace( src.region ) ? null : src.region ) ); //since soap returns null instead of "", can be changed in future.

					cfg.CreateMap< Item, Models.Services.Soap.GetOrders.Order >()
						.ForMember( x => x.OrderId, opt => opt.MapFrom( src => src.entity_id ) )
						.ForMember( x => x.BillingFirstname, opt => opt.MapFrom( src => src != null ? src.billing_address != null ? src.billing_address.firstname ?? string.Empty : string.Empty : string.Empty ) )
						.ForMember( x => x.BillingLastname, opt => opt.MapFrom( src => src != null ? src.billing_address != null ? src.billing_address.lastname ?? string.Empty : string.Empty : string.Empty ) )
						.ForMember( x => x.PaymentMethod, opt => opt.MapFrom( src => src != null ? src.payment != null ? src.payment.method ?? string.Empty : string.Empty : string.Empty ) )
						.ForMember( x => x.ShippingAddressId, opt => opt.MapFrom( src => src != null && src.extension_attributes != null && src.extension_attributes.shipping_assignments != null ? ( src.extension_attributes.shipping_assignments.Any() ? ( src.extension_attributes.shipping_assignments[ 0 ] != null ? ( src.extension_attributes.shipping_assignments[ 0 ].shipping != null ? ( src.extension_attributes.shipping_assignments[ 0 ].shipping.address != null ? src.extension_attributes.shipping_assignments[ 0 ].shipping.address.customer_address_id : null ) : null ) : null ) : null ) : null ) )
						.ForMember( x => x.ShippingAddress, opt => opt.MapFrom( src => src != null && src.extension_attributes != null && src.extension_attributes.shipping_assignments != null && src.extension_attributes.shipping_assignments.Exists( x => x != null && x.shipping != null && x.shipping.address != null && Equals( x.shipping.address.address_type, "shipping" ) ) ? src.extension_attributes.shipping_assignments.FirstOrDefault( y => Equals( y.shipping.address.address_type, "shipping" ) ).shipping.address : null ) )
						.ForMember( x => x.ShippingLastname, opt => opt.MapFrom( src => src != null && src.extension_attributes != null && src.extension_attributes.shipping_assignments != null ? ( src.extension_attributes.shipping_assignments.Any() ? ( src.extension_attributes.shipping_assignments[ 0 ] != null ? ( src.extension_attributes.shipping_assignments[ 0 ].shipping != null ? ( src.extension_attributes.shipping_assignments[ 0 ].shipping.address != null ? src.extension_attributes.shipping_assignments[ 0 ].shipping.address.lastname : null ) : null ) : null ) : null ) : null ) )
						.ForMember( x => x.ShippingFirstname, opt => opt.MapFrom( src => src != null && src.extension_attributes != null && src.extension_attributes.shipping_assignments != null ? ( src.extension_attributes.shipping_assignments.Any() ? ( src.extension_attributes.shipping_assignments[ 0 ] != null ? ( src.extension_attributes.shipping_assignments[ 0 ].shipping != null ? ( src.extension_attributes.shipping_assignments[ 0 ].shipping.address != null ? src.extension_attributes.shipping_assignments[ 0 ].shipping.address.firstname : null ) : null ) : null ) : null ) : null ) )
						.ForMember( x => x.ShippingMethod, opt => opt.MapFrom( src => src != null && src.extension_attributes != null && src.extension_attributes.shipping_assignments != null ? ( src.extension_attributes.shipping_assignments.Any() ? ( src.extension_attributes.shipping_assignments[ 0 ] != null ? ( src.extension_attributes.shipping_assignments[ 0 ].shipping != null ? src.extension_attributes.shipping_assignments[ 0 ].shipping.method : null ) : null ) : null ) : null ) );
					;

					//zoey
					cfg.CreateMap< TsZoey_v_1_9_0_1_CE.salesOrderListEntity, Models.Services.Soap.GetOrders.Order >();
					cfg.CreateMap< TsZoey_v_1_9_0_1_CE.salesOrderItemEntity, Models.Services.Soap.GetOrders.OrderItemEntity >();
					cfg.CreateMap< TsZoey_v_1_9_0_1_CE.salesOrderStatusHistoryEntity, MagentoAccess.Models.Services.Soap.GetOrders.StatusHistoryRecord >();
					cfg.CreateMap< TsZoey_v_1_9_0_1_CE.salesOrderStatusHistoryEntity, MagentoAccess.Models.Services.Soap.GetOrders.StatusHistoryRecord >();
					cfg.CreateMap< TsZoey_v_1_9_0_1_CE.salesOrderAddressEntity, MagentoAccess.Models.Services.Soap.GetOrders.BillingAddress >();
					cfg.CreateMap< TsZoey_v_1_9_0_1_CE.salesOrderPaymentEntity, MagentoAccess.Models.Services.Soap.GetOrders.Payment >();

					//.ForAllMembers(x =>
					//{
					//	x.NullSubstitute(string.Empty);
					//	//x.Condition((i, o, o1, o2, rc) =>
					//	//{
					//	//	rc.
					//	//})
					//})
					;
				} );

				/////////

				//var config = new MapperConfiguration(cfg => cfg.CreateMap<Source, Dest>().ForMember(dest => dest.Value, opt => opt.NullSubstitute("Other Value"));

				await initTask.ConfigureAwait( false );
				return true;
			}
			catch( Exception e )
			{
				if( !supressExc )
					throw e;

				return false;
			}
		}
		
		/// <summary>
		///	Last service's network activity time. Can be used to monitor service's state.
		/// </summary>
		public DateTime LastActivityTime
		{
			get
			{
				return MagentoServiceLowLevelSoap.LastActivityTime ?? DateTime.UtcNow;
			}
		}
	}

	public class MagentoConfig
	{
		public string VersionByDefault{ get; set; }
		public string EditionByDefault{ get; set; }
		public MagentoDefaultProtocol Protocol{ get; set; }
		public ThrowExceptionIfFailed OnUpdateInventory{ get; set; }
		public bool BindingDecompressionEnabled{ get; set; }

		/// <summary>
		/// If true - will use internal mechanism only for provided version. In such case, if VersionByDefault is incorrect, detection of real version will be impossible
		/// If false - Version detection will be able to detect all internal mechanisms which suited to the real store version. Even if these mechanisms are from others store versions
		/// </summary>
		public bool UseVersionByDefaultOnly{ get; set; }
	}

	public static class MagentoConfigExtension
	{
		public static MagentoConfig DefaultIfNull( this MagentoConfig cfg )
		{
			return cfg ?? new MagentoConfig()
			{
				EditionByDefault = "ce",
				OnUpdateInventory = ThrowExceptionIfFailed.OneItem,
				Protocol = MagentoDefaultProtocol.SoapOnly,
				VersionByDefault = "1.9.2.2",
				BindingDecompressionEnabled = false,
			};
		}
	}

	public enum ThrowExceptionIfFailed
	{
		OneItem = 0,
		AllItems = 1,
		Never = 2,
	}

	public enum MagentoDefaultProtocol
	{
		Default = 0,
		SoapOnly = 1,
		RestOnly = 2,
	}

	public class ProductAttributeCodes
	{
		public const string Upc = "upc";
		public const string Cost = "cost";
		public const string Manufacturer = "manufacturer";
	}

	internal class SalesOrderByOrderIdComparer: IEqualityComparer< Models.Services.Soap.GetOrders.Order >
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