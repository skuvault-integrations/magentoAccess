﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Magento2catalogInventoryStockRegistryV1_v_2_0_2_0_CE;
using MagentoAccess.Magento2catalogProductAttributeMediaGalleryManagementV1_v_2_0_2_0_CE;
using MagentoAccess.Magento2catalogProductRepositoryV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using Netco.Extensions;
using CatalogInventoryDataStockItemInterface = MagentoAccess.Magento2catalogInventoryStockRegistryV1_v_2_0_2_0_CE.CatalogInventoryDataStockItemInterface;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_2_0_2_0_ce: IMagentoServiceLowLevelSoap
	{
		private class UpdateRessult
		{
			public UpdateRessult( PutStockItem putStockItem, int success )
			{
				this.PutStockItem = putStockItem;
				this.Success = success;
			}

			public int Success{ get; set; }
			public PutStockItem PutStockItem{ get; set; }
		}

		public virtual async Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, Mark markForLog = null )
		{
			var methodParameters = stockItems.ToJson();
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoCatalogInventoryStockServiceClient( this.BaseMagentoUrl );

				var res = new List< UpdateRessult >();

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

						var updateResult = new UpdateRessult( x, 0 );
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
				throw new MagentoSoapException( string.Format( "An error occured during PutStockItemsAsync({0})", methodParameters ), exc );
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
				throw new MagentoSoapException( string.Format( "An error occured during PutStockItemsAsync({0})", productsBriefInfo ), exc );
			}
		}

		public virtual async Task< SoapGetProductsResponse > GetProductsAsync()
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

							var frameworkSearchCriteriaInterface = new FrameworkSearchCriteriaInterface()
							{
								currentPage = currentPage,
								currentPageSpecified = true,
								pageSize = 100,
								pageSizeSpecified = true,
								//filterGroups = frameworkSearchFilterGroups
							};

							var catalogProductRepositoryV1GetListRequest = new CatalogProductRepositoryV1GetListRequest() { searchCriteria = frameworkSearchCriteriaInterface };
							catalogProductRepositoryV1GetListResponse = await privateClient.catalogProductRepositoryV1GetListAsync( catalogProductRepositoryV1GetListRequest ).ConfigureAwait( false );
							var catalogDataProductInterfaces = catalogProductRepositoryV1GetListResponse == null ? new List< CatalogDataProductInterface >() : catalogProductRepositoryV1GetListResponse.catalogProductRepositoryV1GetListResponse.result.items.ToList();
							res.AddRange( catalogDataProductInterfaces );
							currentPage++;
						}
					} ).ConfigureAwait( false );
				} while( catalogProductRepositoryV1GetListResponse != null && res.Count < catalogProductRepositoryV1GetListResponse.catalogProductRepositoryV1GetListResponse.result.totalCount );

				return new SoapGetProductsResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		public virtual async Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
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
							res = await privateClient.catalogInventoryStockRegistryV1GetStockItemBySkuAsync( catalogInventoryStockRegistryV1GetStockItemBySkuRequest ).ConfigureAwait( false );
						}
					} ).ConfigureAwait( false );

					return Tuple.Create( x, res.catalogInventoryStockRegistryV1GetStockItemBySkuResponse.result );
				} ).ConfigureAwait( false );

				return new InventoryStockItemListResponse( responses );
			}
			catch( Exception exc )
			{
				var productsBriefInfo = string.Join( "|", skusOrIds );
				throw new MagentoSoapException( string.Format( "An error occured during GetStockItemsAsync({0})", productsBriefInfo ), exc );
			}
		}

		public virtual async Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( string productId )
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

					var catalogProductAttributeMediaGalleryManagementV1GetListRequest = new CatalogProductAttributeMediaGalleryManagementV1GetListRequest()
					{ sku = "" };
					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductAttributeMediaGalleryManagementV1GetListAsync( catalogProductAttributeMediaGalleryManagementV1GetListRequest ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new ProductAttributeMediaListResponse( res, productId );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductAttributeMediaListAsync({0})", productId ), exc );
			}
		}

		public virtual async Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( string rootCategory = "1" )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoSalesOrderRepositoryServiceClient( this.BaseMagentoUrl );

				var res = new catalogCategoryTreeResponse();
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened
					    && privateClient.State != CommunicationState.Created
					    && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoSalesOrderRepositoryServiceClient( this.BaseMagentoUrl );

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						//res = await privateClient.catalogCategoryTreeAsync(sessionId, rootCategory, "0").ConfigureAwait(false);
						res = null; //TODO: Implement
				} ).ConfigureAwait( false );

				return new GetCategoryTreeResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetCategoriesTree({0})", rootCategory ), exc );
			}
		}

		public virtual async Task< CatalogProductInfoResponse > GetProductInfoAsync( string skusOrId, string[] custAttributes, bool idPassed = false )
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductInfoResponse();
				var privateClient = this.CreateMagentoCatalogProductRepositoryServiceClient( this.BaseMagentoUrl );

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					if( privateClient.State != CommunicationState.Opened
					    && privateClient.State != CommunicationState.Created
					    && privateClient.State != CommunicationState.Opening )
						privateClient = this.CreateMagentoCatalogProductRepositoryServiceClient(this.BaseMagentoUrl);

					var attributes = new catalogProductRequestAttributes { additional_attributes = custAttributes ?? new string[ 0 ] };

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					{
						var qwe = new CatalogProductRepositoryV1GetRequest();
						res = await privateClient.catalogProductRepositoryV1GetAsync( qwe).ConfigureAwait(false);
					}
					//res = await privateClient.catalogProductRepositoryV1GetAsync( skusOrId, "0", attributes, idPassed ? "1" : "0").ConfigureAwait(false);
						res = null; //TODO: Implement
				} ).ConfigureAwait( false );

				return new CatalogProductInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductInfoAsync({0})", skusOrId ), exc );
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

					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						//res = await privateClient.catalogProductAttributeInfoAsync( sessionId, attribute ).ConfigureAwait( false );
						res = null; //TODO: Implement
				} ).ConfigureAwait( false );

				return new CatalogProductAttributeInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetManufacturerAsync()" ), exc );
			}
		}
	}
}