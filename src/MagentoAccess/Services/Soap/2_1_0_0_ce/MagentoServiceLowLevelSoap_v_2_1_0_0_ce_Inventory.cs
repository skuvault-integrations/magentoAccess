using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.M2catalogInventoryStockRegistryV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2backendModuleServiceV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2catalogCategoryManagementV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2catalogProductAttributeMediaGalleryManagementV1_v_2_1_0_0_CE;
using MagentoAccess.Magento2catalogProductRepositoryV1_v_2_1_0_0_CE;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services.Rest.v2x;
using Netco.Extensions;
using CatalogInventoryDataStockItemInterface = MagentoAccess.M2catalogInventoryStockRegistryV1_v_2_1_0_0_CE.CatalogInventoryDataStockItemInterface;
using Category = MagentoAccess.Models.Services.Soap.GetProducts.Category;
using MagentoUrl = MagentoAccess.Models.Services.Soap.GetProducts.MagentoUrl;

namespace MagentoAccess.Services.Soap._2_1_0_0_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_2_1_0_0_ce : IMagentoServiceLowLevelSoap
	{
		public string StoreVersion { get; set; }

		public virtual async Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, Mark markForLog = null )
		{
			var methodParameters = stockItems.ToJson();
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );

				var res = new List< UpdateRessult< PutStockItem > >();

				await stockItems.DoInBatchAsync( 10, async x =>
				{
					await ActionPolicies.GetAsync.Do( async () =>
					{
						var statusChecker = new StatusChecker( maxCheckCount );
						TimerCallback tcb = statusChecker.CheckStatus;

						if( privateClient.State != CommunicationState.Opened
						    && privateClient.State != CommunicationState.Created
						    && privateClient.State != CommunicationState.Opening )
							privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );

						var updateResult = new UpdateRessult< PutStockItem >( x, 0 );
						res.Add( updateResult );

						using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						{
							MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( methodParameters, mark : markForLog ) );

							var catalogInventoryDataStockItemInterface = new CatalogInventoryDataStockItemInterface()
							{
								qty = x.Qty.ToString(),
								productId = int.Parse( x.ProductId ),
								productIdSpecified = true,
								isInStock = x.Qty > 0,
								isQtyDecimal = false,
								showDefaultNotificationMessage = false,
								useConfigMinQty = true,
								minQty = 0,
								useConfigMinSaleQty = 1,
								minSaleQty = 1,
								useConfigMaxSaleQty = true,
								maxSaleQty = 10000,
								useConfigBackorders = true,
								backorders = 0,
								useConfigNotifyStockQty = true,
								notifyStockQty = 1,
								useConfigQtyIncrements = true,
								qtyIncrements = 0,
								useConfigEnableQtyInc = false,
								enableQtyIncrements = false,
								useConfigManageStock = true,
								manageStock = true,
								//lowStockDate = "2016-02-29 20:48:26",
								isDecimalDivided = false,
								stockStatusChangedAuto = 1
							};
							var catalogInventoryStockRegistryV1UpdateStockItemBySkuRequest = new CatalogInventoryStockRegistryV1UpdateStockItemBySkuRequest()
							{
								productSku = x.Sku,
								stockItem = catalogInventoryDataStockItemInterface
							};

							var temp = await privateClient.catalogInventoryStockRegistryV1UpdateStockItemBySkuAsync( catalogInventoryStockRegistryV1UpdateStockItemBySkuRequest ).ConfigureAwait( false );

							updateResult.Success = temp.catalogInventoryStockRegistryV1UpdateStockItemBySkuResponse.result;
						}
					} ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( methodParameters, mark : markForLog, methodResult : res.ToJson() ) );

				return res.All( x => x.Success > 0 );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during PutStockItemsAsync({methodParameters})", exc );
			}
		}

		public virtual async Task< bool > PutStockItemAsync( PutStockItem putStockItem, Mark markForLog )
		{
			var productsBriefInfo = new List< PutStockItem > { putStockItem }.ToJson();

			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 120000;

				var res = false;
				var privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened
					    && privateClient.State != CommunicationState.Created
					    && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						MagentoLogger.LogTraceStarted( this.CreateMethodCallInfo( productsBriefInfo, markForLog ) );

						var catalogInventoryDataStockItemInterface = new CatalogInventoryDataStockItemInterface()
						{
							qty = putStockItem.Qty.ToString(),
							productId = int.Parse( putStockItem.ProductId ),
							productIdSpecified = true,
							isInStock = putStockItem.Qty > 0,
							isQtyDecimal = false,
							showDefaultNotificationMessage = false,
							useConfigMinQty = true,
							minQty = 0,
							useConfigMinSaleQty = 1,
							minSaleQty = 1,
							useConfigMaxSaleQty = true,
							maxSaleQty = 10000,
							useConfigBackorders = true,
							backorders = 0,
							useConfigNotifyStockQty = true,
							notifyStockQty = 1,
							useConfigQtyIncrements = true,
							qtyIncrements = 0,
							useConfigEnableQtyInc = false,
							enableQtyIncrements = false,
							useConfigManageStock = true,
							manageStock = true,
							//lowStockDate = "2016-02-29 20:48:26",
							isDecimalDivided = false,
							stockStatusChangedAuto = 1
						};
						var catalogInventoryStockRegistryV1UpdateStockItemBySkuRequest = new CatalogInventoryStockRegistryV1UpdateStockItemBySkuRequest()
						{
							//productSku = x.Sku,
							stockItem = catalogInventoryDataStockItemInterface
						};

						var temp = await privateClient.catalogInventoryStockRegistryV1UpdateStockItemBySkuAsync( catalogInventoryStockRegistryV1UpdateStockItemBySkuRequest ).ConfigureAwait( false );

						res = temp.catalogInventoryStockRegistryV1UpdateStockItemBySkuResponse.result > 0;

						var updateBriefInfo = string.Format( "{{Success:{0}}}", res );
						MagentoLogger.LogTraceEnded( this.CreateMethodCallInfo( productsBriefInfo, markForLog, methodResult : updateBriefInfo ) );
					}
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during PutStockItemsAsync({productsBriefInfo})", exc );
			}
		}

		//TODO: refactor, remove redundant wrapper
		public virtual async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom )
		{
			return await this.GetProductsAsync( int.MaxValue, productType, productTypeShouldBeExcluded, updatedFrom );
		}

		private async Task< SoapGetProductsResponse > GetProductsOldAsync( int limit, string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new List< CatalogDataProductInterface >();
				var privateClient = this.CreateMagentoCatalogProductRepositoryServiceClient( this.BaseMagentoUrl );
				catalogProductRepositoryV1GetListResponse1 catalogProductRepositoryV1GetListResponse = null;
				var currentPage = 0;

				do
				{
					await ActionPolicies.GetAsync.Do( async () =>
					{
						var statusChecker = new StatusChecker( maxCheckCount );
						TimerCallback tcb = statusChecker.CheckStatus;

						if( privateClient.State != CommunicationState.Opened
						    && privateClient.State != CommunicationState.Created
						    && privateClient.State != CommunicationState.Opening )
							privateClient = this.CreateMagentoCatalogProductRepositoryServiceClient( this.BaseMagentoUrl );

						using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						{
							//var frameworkSearchFilterGroup1 = new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { conditionType = "gt", field = "created_at", value = "2000-01-01 01:01:01" } } };
							//var frameworkSearchFilterGroup2 = new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { conditionType = "lt", field = "created_at", value = "2100-01-01 01:01:01" } } };
							//var frameworkSearchFilterGroups = new[] { frameworkSearchFilterGroup1, frameworkSearchFilterGroup2 };

							var frameworkSearchFilterGroups = new List< FrameworkSearchFilterGroup >();

							var frameworkSearchCriteriaInterface = new FrameworkSearchCriteriaInterface()
							{
								currentPage = currentPage,
								currentPageSpecified = true,
								pageSize = 100,
								pageSizeSpecified = true,
								//filterGroups = frameworkSearchFilterGroups
							};

							if( updatedFrom.HasValue )
							{
								var filter = new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { conditionType = "gt", field = "updated_at", value = updatedFrom.Value.ToSoapParameterString() } } };
								frameworkSearchFilterGroups.Add( filter );
							}

							// filtering by typeId doesn't works for magento2.0.2
							//if( productType != null )
							//{
							//	if( frameworkSearchCriteriaInterface.filterGroups == null )
							//		frameworkSearchCriteriaInterface.filterGroups = new FrameworkSearchFilterGroup[] { };

							//	var temp = frameworkSearchCriteriaInterface.filterGroups.ToList();
							//	temp.Add( new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { conditionType = "like", field = "type", value = productType } } } );
							//	frameworkSearchCriteriaInterface.filterGroups = temp.ToArray();
							//}

							if( frameworkSearchFilterGroups.Any() )
								frameworkSearchCriteriaInterface.filterGroups = frameworkSearchFilterGroups.ToArray();

							var catalogProductRepositoryV1GetListRequest = new CatalogProductRepositoryV1GetListRequest() { searchCriteria = frameworkSearchCriteriaInterface };
							catalogProductRepositoryV1GetListResponse = await privateClient.catalogProductRepositoryV1GetListAsync( catalogProductRepositoryV1GetListRequest ).ConfigureAwait( false );
							var catalogDataProductInterfaces = catalogProductRepositoryV1GetListResponse == null ? new List< CatalogDataProductInterface >() : catalogProductRepositoryV1GetListResponse.catalogProductRepositoryV1GetListResponse.result.items.ToList();

							res.AddRange( catalogDataProductInterfaces );
							currentPage++;
						}
					} ).ConfigureAwait( false );
				} while( catalogProductRepositoryV1GetListResponse != null && res.Count < limit && res.Count < catalogProductRepositoryV1GetListResponse.catalogProductRepositoryV1GetListResponse.result.totalCount );

				if( !string.IsNullOrWhiteSpace( productType ) )
				{
					res = productTypeShouldBeExcluded
						? res.Where( x => !string.Equals( x.typeId, productType, StringComparison.InvariantCultureIgnoreCase ) ).ToList()
						: res.Where( x => string.Equals( x.typeId, productType, StringComparison.InvariantCultureIgnoreCase ) ).ToList();
				}

				return new SoapGetProductsResponse( res.TakeWhile( ( x, i ) => i < limit ).ToList() );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		private async Task< SoapGetProductsResponse > GetProductsAsync( int limit, string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom )
		{
			try
			{
				var pagingModel = new PagingModel( 200, 0 );
				var firstPage = await this.GetProductsPageAsync( pagingModel.CurrentPage, pagingModel.ItemsPerPage, updatedFrom ).ConfigureAwait( false );
				if( firstPage == null )
					return new SoapGetProductsResponse( new List< CatalogDataProductInterface >() );

				var pagesNumbers = pagingModel.GetPages( firstPage.totalCount, limit );
				var pages = await pagesNumbers.ProcessInBatchAsync( 4, async x => await this.GetProductsPageAsync( x, pagingModel.ItemsPerPage, updatedFrom ).ConfigureAwait( false ) ).ConfigureAwait( false );
				var pagesWithLimits = pages.SelectMany( x => x.items );
				var pagesWithLimitsDistinct = pagesWithLimits.GroupBy( x => x.sku ).Select( x => x.First() );

				if( !string.IsNullOrWhiteSpace( productType ) )
					pagesWithLimitsDistinct = productTypeShouldBeExcluded
						? pagesWithLimitsDistinct.Where( x => !string.Equals( x.typeId, productType, StringComparison.InvariantCultureIgnoreCase ) ).ToList()
						: pagesWithLimitsDistinct.Where( x => string.Equals( x.typeId, productType, StringComparison.InvariantCultureIgnoreCase ) ).ToList();

				var catalogDataProductInterfaces = pagesWithLimitsDistinct.TakeWhile( ( x, i ) => i < limit ).OrderBy( x => x.sku ).ToList();

				return new SoapGetProductsResponse( catalogDataProductInterfaces );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}


		private static void AddFilter( FrameworkSearchCriteriaInterface filters, string value, string key, string valueKey )
		{
			if( filters.filterGroups == null )
				filters.filterGroups = new FrameworkSearchFilterGroup[ 0 ];

			var temp = filters.filterGroups.ToList();
			temp.Add( new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { conditionType = valueKey, field = key, value = value } } } );
			filters.filterGroups = temp.ToArray();
		}

		private async Task< CatalogDataProductSearchResultsInterface > GetProductsPageAsync( int currentPage, int pageSize, DateTime? updatedFrom )
		{
			var parameters = Tuple.Create( currentPage, pageSize, updatedFrom );
			var cachedR = this.getProductsPageCache.Get( parameters );
			if( cachedR != null )
				return cachedR;

			var frameworkSearchCriteriaInterface = new FrameworkSearchCriteriaInterface()
			{
				currentPage = currentPage,
				currentPageSpecified = true,
				pageSize = pageSize,
				pageSizeSpecified = true,
				//filterGroups = frameworkSearchFilterGroups
			};

			if( updatedFrom.HasValue )
				AddFilter( frameworkSearchCriteriaInterface, updatedFrom.Value.ToSoapParameterString(), "updated_at", "gt" );

			// filtering by typeId doesn't works for magento2.0.2
			//if( productType != null )
			//{
			//	if( frameworkSearchCriteriaInterface.filterGroups == null )
			//		frameworkSearchCriteriaInterface.filterGroups = new FrameworkSearchFilterGroup[] { };

			//	var temp = frameworkSearchCriteriaInterface.filterGroups.ToList();
			//	temp.Add( new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { conditionType = "like", field = "type", value = productType } } } );
			//	frameworkSearchCriteriaInterface.filterGroups = temp.ToArray();
			//}

			var catalogProductRepositoryV1GetListRequest = new CatalogProductRepositoryV1GetListRequest() { searchCriteria = frameworkSearchCriteriaInterface };

			//return await this.GetWithAsync(
			var result = await this.GetWithAsync(
				res => res?.catalogProductRepositoryV1GetListResponse.result,
				//res => new SoapGetProductsResponse(res == null? new List<CatalogDataProductSearchResultsInterface>(): new List<CatalogDataProductSearchResultsInterface>() { res.catalogProductRepositoryV1GetListResponse }),
				//res => new SoapGetProductsResponse(  res.catalogProductRepositoryV1GetListResponse.result ),
				async ( client, session ) => await client.catalogProductRepositoryV1GetListAsync( catalogProductRepositoryV1GetListRequest ).ConfigureAwait( false ), 1800000, this.CreateMagentoCatalogProductRepositoryServiceClient, this.RecreateMagentoServiceClientIfItNeed2 );
			this.getProductsPageCache.Add( result, parameters, TimeSpan.FromSeconds( 300 ) );
			return result;
		}

		protected Cache< Tuple< int, int, DateTime? >, CatalogDataProductSearchResultsInterface> getProductsPageCache = new Cache< Tuple< int, int, DateTime? >, CatalogDataProductSearchResultsInterface>();

		private async Task< GetBackEndMuduleServiceResponse > GetBackEndModulesAsync()
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				GetBackEndMuduleServiceResponse res = null;
				var privateClient = this.CreateMagentoBackendModuleServiceV1Client( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened
					    && privateClient.State != CommunicationState.Created
					    && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoBackendModuleServiceV1Client( this.BaseMagentoUrl );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						var backendModuleServiceV1GetModulesResponse1 = await privateClient.backendModuleServiceV1GetModulesAsync( new BackendModuleServiceV1GetModulesRequest() ).ConfigureAwait( false );
						res = new GetBackEndMuduleServiceResponse( backendModuleServiceV1GetModulesResponse1 );
					}
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		public virtual async Task< InventoryStockItemListResponse > GetStockItemsOldAsync( List< string > skusOrIds )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );

				var responses = await skusOrIds.ProcessInBatchAsync( 10, async x =>
				{
					var res = new catalogInventoryStockRegistryV1GetStockItemBySkuResponse1();
					await ActionPolicies.GetAsync.Do( async () =>
					{
						var statusChecker = new StatusChecker( maxCheckCount );
						TimerCallback tcb = statusChecker.CheckStatus;

						if( privateClient.State != CommunicationState.Opened
						    && privateClient.State != CommunicationState.Created
						    && privateClient.State != CommunicationState.Opening )
							privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );

						using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						{
							var catalogInventoryStockRegistryV1GetStockItemBySkuRequest = new CatalogInventoryStockRegistryV1GetStockItemBySkuRequest() { productSku = x };
							try
							{
								res = await privateClient.catalogInventoryStockRegistryV1GetStockItemBySkuAsync( catalogInventoryStockRegistryV1GetStockItemBySkuRequest ).ConfigureAwait( false );
							}
							catch( CommunicationException ex ) //crutch for magento 2.1
							{
								var exceptionMessge = ex.Message.ToLower();
								if( exceptionMessge.Contains( @"(soap12 (http://www.w3.org/2003/05/soap-envelope))" ) &&
								    exceptionMessge.Contains( @"(soap11 (http://schemas.xmlsoap.org/soap/envelope/))" ) )
								{
									res = null;
									return;
								}
								throw;
							}
						}
					} ).ConfigureAwait( false );

					return res == null ? null : Tuple.Create( x, res.catalogInventoryStockRegistryV1GetStockItemBySkuResponse.result );
				} ).ConfigureAwait( false );

				return new InventoryStockItemListResponse( responses.Where( x => x != null ) );
			}
			catch( Exception exc )
			{
				var productsBriefInfo = string.Join( "|", skusOrIds );
				throw new MagentoSoapException( $"An error occured during GetStockItemsAsync({productsBriefInfo})", exc );
			}
		}

		public virtual async Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			try
			{
				var inventory = await this.GetStockItemsWithoutSkuAsync().ConfigureAwait( false );

				var products = await this.GetProductsAsync( null, false, null ).ConfigureAwait( false );

				var inventoryWithSku = ( from pr in products.Products
					join inv in inventory.InventoryStockItems on pr.ProductId equals inv.ProductId
					select new InventoryStockItem( inv ) { Sku = pr.Sku } ).OrderBy( x => x.Sku ).ToList();

				inventory.InventoryStockItems = ( from i in inventoryWithSku join s in skusOrIds on i.Sku equals s select i ).OrderBy( x => x.Sku ).ToList();
				return inventory;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetStockItemsAsync({0})", "" ), exc );
			}
		}

		public virtual async Task< InventoryStockItemListResponse > GetStockItemsWithoutSkuAsync()
		{
			try
			{
				var pageSize = 500;
				var res = await this.GetStockItemsPageAsync( 1, pageSize ).ConfigureAwait( false );
				if( res.catalogInventoryStockRegistryV1GetLowStockItemsResponse.result.totalCount <= pageSize )
					return new InventoryStockItemListResponse( new[] { Tuple.Create( 1, res.catalogInventoryStockRegistryV1GetLowStockItemsResponse.result ) } );

				var pagingModel = new PagingModel( pageSize, 0 );
				var responses = await pagingModel.GetPages( res.catalogInventoryStockRegistryV1GetLowStockItemsResponse.result.totalCount ).ProcessInBatchAsync( 10, async x =>
				{
					var pageResp = await this.GetStockItemsPageAsync( x, pageSize ).ConfigureAwait( false );
					return Tuple.Create( x, pageResp.catalogInventoryStockRegistryV1GetLowStockItemsResponse.result );
				} ).ConfigureAwait( false );
				var inventory = new InventoryStockItemListResponse( responses );

				return inventory;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetStockItemsWithoutSkuAsync({0})", "" ), exc );
			}
		}

		private async Task< catalogInventoryStockRegistryV1GetLowStockItemsResponse1 > GetStockItemsPageOld( int currentPage, int pageSize )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );
				var res = new catalogInventoryStockRegistryV1GetLowStockItemsResponse1();
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened
					    && privateClient.State != CommunicationState.Created
					    && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						var catalogInventoryStockRegistryV1GetStockItemBySkuRequest = new CatalogInventoryStockRegistryV1GetLowStockItemsRequest() { currentPage = currentPage, currentPageSpecified = true, pageSize = pageSize, pageSizeSpecified = true, qty = 999999999999, scopeId = 1 };
						res = await privateClient.catalogInventoryStockRegistryV1GetLowStockItemsAsync( catalogInventoryStockRegistryV1GetStockItemBySkuRequest ).ConfigureAwait( false );
					}
				} ).ConfigureAwait( false );
				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetStockItemsAsync({0})", "" ), exc );
			}
		}

		private async Task< catalogInventoryStockRegistryV1GetLowStockItemsResponse1 > GetStockItemsPageAsync( int currentPage, int pageSize )
		{
			var catalogInventoryStockRegistryV1GetStockItemBySkuRequest = new CatalogInventoryStockRegistryV1GetLowStockItemsRequest() { currentPage = currentPage, currentPageSpecified = true, pageSize = pageSize, pageSizeSpecified = true, qty = 999999999999, scopeId = 1 };

			return await this.GetWithAsync(
				res => res,
				async ( client, session ) => await client.catalogInventoryStockRegistryV1GetLowStockItemsAsync( catalogInventoryStockRegistryV1GetStockItemBySkuRequest ).ConfigureAwait( false ), 600000, this.CreateMagentoCatalogInventoryStockServiceClient, this.RecreateMagentoServiceClientIfItNeed );
		}

		public virtual async Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, bool throwException = true )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentocatalogProductAttributeMediaGalleryRepositoryServiceClient( this.BaseMagentoUrl );

				var res = new catalogProductAttributeMediaGalleryManagementV1GetListResponse1();
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened
					    && privateClient.State != CommunicationState.Created
					    && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentocatalogProductAttributeMediaGalleryRepositoryServiceClient( this.BaseMagentoUrl );

					var catalogProductAttributeMediaGalleryManagementV1GetListRequest = new CatalogProductAttributeMediaGalleryManagementV1GetListRequest() { sku = getProductAttributeMediaListRequest.Sku };
					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductAttributeMediaGalleryManagementV1GetListAsync( catalogProductAttributeMediaGalleryManagementV1GetListRequest ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new ProductAttributeMediaListResponse( res, getProductAttributeMediaListRequest.ProductId, getProductAttributeMediaListRequest.Sku );
			}
			catch( Exception exc )
			{
				if( throwException )
					throw new MagentoSoapException( $"An error occured during GetProductAttributeMediaListAsync({getProductAttributeMediaListRequest})", exc );
				else
					return new ProductAttributeMediaListResponse( exc );
			}
		}

		public virtual async Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( string rootCategory = "1" )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoCategoriesRepositoryServiceClient( this.BaseMagentoUrl );

				var res = new catalogCategoryManagementV1GetTreeResponse1();
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened
					    && privateClient.State != CommunicationState.Created
					    && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoCategoriesRepositoryServiceClient( this.BaseMagentoUrl );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						var someRequest = new CatalogCategoryManagementV1GetTreeRequest() { depth = 111, depthSpecified = true, rootCategoryId = ( int )rootCategory.ToDecimalOrDefault(), rootCategoryIdSpecified = true };
						res = await privateClient.catalogCategoryManagementV1GetTreeAsync( someRequest ).ConfigureAwait( false );
					}
				} ).ConfigureAwait( false );

				return new GetCategoryTreeResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( $"An error occured during GetCategoriesTree({rootCategory})", exc );
			}
		}

		public virtual async Task< CatalogProductInfoResponse > GetProductInfoAsync( CatalogProductInfoRequest catalogProductInfoRequest, bool throwException = true )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductRepositoryV1GetResponse1();
				var privateClient = this.CreateMagentoCatalogProductRepositoryServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened
					    && privateClient.State != CommunicationState.Created
					    && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoCatalogProductRepositoryServiceClient( this.BaseMagentoUrl );

					// we don't need them, since Magento 2.0 returns all attributes
					//var attributes = new catalogProductRequestAttributes { additional_attributes = custAttributes ?? new string[ 0 ] };

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						var catalogProductRepositoryV1GetRequest = new CatalogProductRepositoryV1GetRequest() { sku = catalogProductInfoRequest.Sku };
						res = await privateClient.catalogProductRepositoryV1GetAsync( catalogProductRepositoryV1GetRequest ).ConfigureAwait( false );
					}
				} ).ConfigureAwait( false );

				return new CatalogProductInfoResponse( res );
			}
			catch( Exception exc )
			{
				if( throwException )
					throw new MagentoSoapException( $"An error occured during GetProductInfoAsync({catalogProductInfoRequest.ToJson()})", exc );
				else
					return new CatalogProductInfoResponse( exc );
			}
		}

		public virtual async Task< CatalogProductAttributeInfoResponse > GetManufacturersInfoAsync( string attribute )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductAttributeInfoResponse();
				var privateClient = this.CreateMagentoSalesOrderRepositoryServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened
					    && privateClient.State != CommunicationState.Created
					    && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoSalesOrderRepositoryServiceClient( this.BaseMagentoUrl );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						//res = await privateClient.catalogProductAttributeInfoAsync( sessionId, attribute ).ConfigureAwait( false );
						res = await Task.FromResult< catalogProductAttributeInfoResponse >( null ); //TODO: Implement
				} ).ConfigureAwait( false );

				return new CatalogProductAttributeInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetManufacturerAsync()" ), exc );
			}
		}

		public virtual async Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts )
		{
			var resultProductslist = resultProducts as IList< ProductDetails > ?? resultProducts.ToList();
			var attributes = new string[] { ProductAttributeCodes.Cost, ProductAttributeCodes.Manufacturer, ProductAttributeCodes.Upc };
#if DEBUG
			var batchSize = 30;
#else
			var batchSize = 10;
#endif
			//var productAttributes = this.GetManufacturersInfoAsync( ProductAttributeCodes.Manufacturer );
			//productAttributes.Wait();

			var productsInfo = await resultProductslist.ProcessInBatchAsync( batchSize, async x => await this.GetProductInfoAsync( new CatalogProductInfoRequest( attributes, x.Sku, x.ProductId ), false ).ConfigureAwait( false ) ).ConfigureAwait( false );
			var categoriesTreeResponse = await this.GetCategoriesTreeAsync().ConfigureAwait( false );

			productsInfo = productsInfo.Where( x => x.Exc == null );
			var magentoCategoriesList = categoriesTreeResponse.RootCategory == null ? new List< CategoryNode >() : categoriesTreeResponse.RootCategory.Flatten();

			Func< IEnumerable< ProductDetails >, IEnumerable< ProductAttributeMediaListResponse >, IEnumerable< ProductDetails > > FillImageUrls = ( prods, mediaLists ) =>
				( from rp in prods
					join pi in mediaLists on rp.ProductId equals pi.ProductId into pairs
					from pair in pairs.DefaultIfEmpty()
					let magentoUrls = pair.MagentoImages.Select( x => new MagentoUrl( x ) )
					select pair == null ? rp : new ProductDetails( rp, magentoUrls ) );

			Func< IEnumerable< ProductDetails >, IEnumerable< CatalogProductInfoResponse >, IEnumerable< ProductDetails > > FillWeightDescriptionShortDescriptionPricev =
				( prods, prodInfos ) => ( from rp in prods
					join pi in prodInfos on rp.ProductId equals pi.ProductId into pairs
					from pair in pairs.DefaultIfEmpty()
					let imageesUrls = ( pair.Attributes ?? new List< ProductAttribute >() ).Where( IsImageUrlAttribute ).Select( x => new MagentoUrl( new MagentoImage( x.Key, this.BaseMagentoUrl + x.Value ) ) ).ToList() // new List< MagentoUrl >()
					select pair == null ? rp : new ProductDetails( rp, upc : pair.GetUpcAttributeValue(), manufacturer : pair.GetManufacturerAttributeValue(), cost : pair.GetCostAttributeValue().ToDecimalOrDefault(), weight : pair.Weight, shortDescription : pair.ShortDescription, description : pair.Description, specialPrice : pair.SpecialPrice, price : pair.Price, categories : pair.CategoryIds.Select( z => new Category( z ) ), images : imageesUrls ) );

			Func< IEnumerable< ProductDetails >, CatalogProductAttributeInfoResponse, IEnumerable< ProductDetails > > FillManufactures =
				( prods, prodInfos ) => ( from rp in prods
					join pi in prodInfos != null ? prodInfos.Attributes : new List< ProductAttributeInfo >() on rp.Manufacturer equals pi.Value into pairs
					from pair in pairs.DefaultIfEmpty()
					select pair == null ? rp : new ProductDetails( rp, manufacturer : pair.Label ) );

			Func< IEnumerable< ProductDetails >, IEnumerable< Category >, IEnumerable< ProductDetails > > FillProductsDeepestCategory =
				( prods, allCategories ) => ( from prod in prods
					let prodCategories = ( from prodCategory in ( prod.Categories ?? Enumerable.Empty< Category >() )
						join itemFromAllCategories in allCategories on prodCategory.Id equals itemFromAllCategories.Id
						select itemFromAllCategories )
					select new ProductDetails( prod, categories : prodCategories ) );

			resultProducts = FillWeightDescriptionShortDescriptionPricev( resultProductslist, productsInfo ).ToList();
			//resultProducts = FillImageUrls( resultProducts, mediaListResponses ).ToList();
			//resultProducts = FillManufactures( resultProducts, productAttributes.Result ).ToList();//TODO: remove completely
			resultProducts = FillProductsDeepestCategory( resultProducts, magentoCategoriesList.Select( y => new Category( y ) ).ToList() ).ToList();
			return resultProducts;
		}

		private static bool IsImageUrlAttribute( ProductAttribute x )
		{
			return string.Compare( x.Key, "swatch_image", StringComparison.CurrentCultureIgnoreCase ) == 0
			       || string.Compare( x.Key, "thumbnail", StringComparison.CurrentCultureIgnoreCase ) == 0
			       || string.Compare( x.Key, "small_image", StringComparison.CurrentCultureIgnoreCase ) == 0
			       || string.Compare( x.Key, "image", StringComparison.CurrentCultureIgnoreCase ) == 0;
		}

		protected catalogInventoryStockRegistryV1PortTypeClient RecreateMagentoServiceClientIfItNeed( catalogInventoryStockRegistryV1PortTypeClient privateClient )
		{
			if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
				privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );
			return privateClient; 
		}

		protected catalogProductRepositoryV1PortTypeClient RecreateMagentoServiceClientIfItNeed2( catalogProductRepositoryV1PortTypeClient privateClient )
		{
			if( privateClient.State != CommunicationState.Opened && privateClient.State != CommunicationState.Created && privateClient.State != CommunicationState.Opening )
				privateClient = this.CreateMagentoCatalogProductRepositoryServiceClient( this.BaseMagentoUrl );
			return privateClient;
		}

		private static class ClientBaseActionRunner
		{
			public static async Task< Tuple< TClientResponse, bool > > RunWithAbortAsync< TClientResponse, TClient >( int delayBeforeCheck, Func< Task< TClientResponse > > func, TClient cleintBase ) where TClient : class
			{
				var statusChecker = new StatusChecker( 2 );
				TimerCallback tcb = statusChecker.CheckStatus3< TClient >;

				using( var stateTimer = new Timer( tcb, cleintBase, 1000, delayBeforeCheck ) )
				{
					var clientResponse = await func().ConfigureAwait( false );
					stateTimer.Change( Timeout.Infinite, Timeout.Infinite );
					return Tuple.Create( clientResponse, statusChecker.IsAborted );
				}
			}
		}

		private async Task< TResult > GetWithAsync< TResult, TServerResponse, TClient >( Func< TServerResponse, TResult > converter, Func<TClient, string, Task< TServerResponse > > action, int abortAfter, Func< string, TClient> clientFactory, Func<TClient, TClient> clientRecreateFactory, bool suppressException = false, [ CallerMemberName ] string callerName = null ) where TServerResponse : new() where TClient : class
		{
			try
			{
				var res = new TServerResponse();
				var privateClient = clientFactory( this.BaseMagentoUrl );
				await ActionPolicies.GetAsync.Do( async () =>
				{
					privateClient = clientRecreateFactory( privateClient );
					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					var temp = await ClientBaseActionRunner.RunWithAbortAsync(
						abortAfter,
						async () => res = await action( privateClient, sessionId.SessionId ).ConfigureAwait( false ),
						privateClient );

					if( temp.Item2 )
						throw new TaskCanceledException();
				} ).ConfigureAwait( false );

				return converter( res );
			}
			catch( Exception exc )
			{
				if( suppressException )
				{
					return default(TResult);
				}
				throw new MagentoSoapException( $"An error occured during{callerName}->{nameof( this.GetWithAsync )}", exc );
			}
		}
	}

}