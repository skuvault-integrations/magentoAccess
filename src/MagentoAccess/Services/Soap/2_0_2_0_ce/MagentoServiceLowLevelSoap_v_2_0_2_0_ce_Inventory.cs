	using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Magento2salesOrderRepositoryV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_2_0_2_0_ce : IMagentoServiceLowLevelSoap
	{
		public virtual async Task<bool> PutStockItemsAsync(List<PutStockItem> stockItems, Mark markForLog = null)
		{
			var methodParameters = stockItems.ToJson();
			try
			{
				var stockItemsProcessed = stockItems.Select(x =>
				{
					var catalogInventoryStockItemUpdateEntity = (x.Qty > 0) ?
						new catalogInventoryStockItemUpdateEntity() { is_in_stock = 1, is_in_stockSpecified = true, qty = x.Qty.ToString() } :
						new catalogInventoryStockItemUpdateEntity() { is_in_stock = 0, is_in_stockSpecified = false, qty = x.Qty.ToString() };
					return Tuple.Create(x, catalogInventoryStockItemUpdateEntity);
				});

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = false;
				var privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

				await ActionPolicies.GetAsync.Do(async () =>
				{
					var statusChecker = new StatusChecker(maxCheckCount);
					TimerCallback tcb = statusChecker.CheckStatus;

					if (privateClient.State != CommunicationState.Opened
						&& privateClient.State != CommunicationState.Created
						&& privateClient.State != CommunicationState.Opening)
						privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

					var sessionId = await this.GetSessionId().ConfigureAwait(false);

					using (var stateTimer = new Timer(tcb, privateClient, 1000, delayBeforeCheck))
					{
						MagentoLogger.LogTraceStarted(this.CreateMethodCallInfo(methodParameters, mark: markForLog));

						var temp = await privateClient.catalogInventoryStockItemMultiUpdateAsync(sessionId, stockItemsProcessed.Select(x => x.Item1.ProductId).ToArray(), stockItemsProcessed.Select(x => x.Item2).ToArray()).ConfigureAwait(false);

						res = temp.result;

						var updateBriefInfo = string.Format("{{Success:{0}}}", res);
						MagentoLogger.LogTraceEnded(this.CreateMethodCallInfo(methodParameters, mark: markForLog, methodResult: updateBriefInfo));
					}
				}).ConfigureAwait(false);

				return res;
			}
			catch (Exception exc)
			{
				throw new MagentoSoapException(string.Format("An error occured during PutStockItemsAsync({0})", methodParameters), exc);
			}
		}

		public virtual async Task<bool> PutStockItemAsync(PutStockItem putStockItem, Mark markForLog)
		{
			var productsBriefInfo = new List<PutStockItem> { putStockItem }.ToJson();

			try
			{
				var catalogInventoryStockItemUpdateEntity = (putStockItem.Qty > 0) ?
					new catalogInventoryStockItemUpdateEntity() { is_in_stock = 1, is_in_stockSpecified = true, qty = putStockItem.Qty.ToString() } :
					new catalogInventoryStockItemUpdateEntity() { is_in_stock = 0, is_in_stockSpecified = false, qty = putStockItem.Qty.ToString() };

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 120000;

				var res = false;
				var privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

				await ActionPolicies.GetAsync.Do(async () =>
				{
					var statusChecker = new StatusChecker(maxCheckCount);
					TimerCallback tcb = statusChecker.CheckStatus;

					if (privateClient.State != CommunicationState.Opened
						&& privateClient.State != CommunicationState.Created
						&& privateClient.State != CommunicationState.Opening)
						privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

					var sessionId = await this.GetSessionId().ConfigureAwait(false);

					using (var stateTimer = new Timer(tcb, privateClient, 1000, delayBeforeCheck))
					{
						MagentoLogger.LogTraceStarted(this.CreateMethodCallInfo(productsBriefInfo, markForLog));

						var temp = await privateClient.catalogInventoryStockItemUpdateAsync(sessionId, putStockItem.ProductId, catalogInventoryStockItemUpdateEntity).ConfigureAwait(false);

						res = temp.result > 0;

						var updateBriefInfo = string.Format("{{Success:{0}}}", res);
						MagentoLogger.LogTraceEnded(this.CreateMethodCallInfo(productsBriefInfo, markForLog, methodResult: updateBriefInfo));
					}
				}).ConfigureAwait(false);

				return res;
			}
			catch (Exception exc)
			{
				throw new MagentoSoapException(string.Format("An error occured during PutStockItemsAsync({0})", productsBriefInfo), exc);
			}
		}

		public virtual async Task<SoapGetProductsResponse> GetProductsAsync()
		{
			try
			{
				var filters = new filters { filter = new associativeEntity[0] };

				var store = string.IsNullOrWhiteSpace(this.Store) ? null : this.Store;

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductListResponse();
				var privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

				await ActionPolicies.GetAsync.Do(async () =>
				{
					var statusChecker = new StatusChecker(maxCheckCount);
					TimerCallback tcb = statusChecker.CheckStatus;

					if (privateClient.State != CommunicationState.Opened
						&& privateClient.State != CommunicationState.Created
						&& privateClient.State != CommunicationState.Opening)
						privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

					var sessionId = await this.GetSessionId().ConfigureAwait(false);

					using (var stateTimer = new Timer(tcb, privateClient, 1000, delayBeforeCheck))
						res = await privateClient.catalogProductListAsync(sessionId, filters, store).ConfigureAwait(false);
				}).ConfigureAwait(false);

				return new SoapGetProductsResponse(res);
			}
			catch (Exception exc)
			{
				throw new MagentoSoapException(string.Format("An error occured during GetProductsAsync()"), exc);
			}
		}

		public virtual async Task<InventoryStockItemListResponse> GetStockItemsAsync(List<string> skusOrIds)
		{
			try
			{
				var skusArray = skusOrIds.ToArray();

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogInventoryStockItemListResponse();
				var privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

				await ActionPolicies.GetAsync.Do(async () =>
				{
					var statusChecker = new StatusChecker(maxCheckCount);
					TimerCallback tcb = statusChecker.CheckStatus;

					if (privateClient.State != CommunicationState.Opened
						&& privateClient.State != CommunicationState.Created
						&& privateClient.State != CommunicationState.Opening)
						privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

					var sessionId = await this.GetSessionId().ConfigureAwait(false);

					using (var stateTimer = new Timer(tcb, privateClient, 1000, delayBeforeCheck))
						res = await privateClient.catalogInventoryStockItemListAsync(sessionId, skusArray).ConfigureAwait(false);
				}).ConfigureAwait(false);

				return new InventoryStockItemListResponse(res);
			}
			catch (Exception exc)
			{
				var productsBriefInfo = string.Join("|", skusOrIds);
				throw new MagentoSoapException(string.Format("An error occured during GetStockItemsAsync({0})", productsBriefInfo), exc);
			}
		}

		public virtual async Task<ProductAttributeMediaListResponse> GetProductAttributeMediaListAsync(string productId)
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

				var res = new catalogProductAttributeMediaListResponse();
				await ActionPolicies.GetAsync.Do(async () =>
				{
					var statusChecker = new StatusChecker(maxCheckCount);
					TimerCallback tcb = statusChecker.CheckStatus;

					if (privateClient.State != CommunicationState.Opened
						&& privateClient.State != CommunicationState.Created
						&& privateClient.State != CommunicationState.Opening)
						privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

					var sessionId = await this.GetSessionId().ConfigureAwait(false);

					using (var stateTimer = new Timer(tcb, privateClient, 1000, delayBeforeCheck))
						res = await privateClient.catalogProductAttributeMediaListAsync(sessionId, productId, "0", "1").ConfigureAwait(false);
				}).ConfigureAwait(false);

				return new ProductAttributeMediaListResponse(res, productId);
			}
			catch (Exception exc)
			{
				throw new MagentoSoapException(string.Format("An error occured during GetProductAttributeMediaListAsync({0})", productId), exc);
			}
		}

		public virtual async Task<GetCategoryTreeResponse> GetCategoriesTreeAsync(string rootCategory = "1")
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

				var res = new catalogCategoryTreeResponse();
				await ActionPolicies.GetAsync.Do(async () =>
				{
					var statusChecker = new StatusChecker(maxCheckCount);
					TimerCallback tcb = statusChecker.CheckStatus;

					if (privateClient.State != CommunicationState.Opened
						&& privateClient.State != CommunicationState.Created
						&& privateClient.State != CommunicationState.Opening)
						privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

					var sessionId = await this.GetSessionId().ConfigureAwait(false);

					using (var stateTimer = new Timer(tcb, privateClient, 1000, delayBeforeCheck))
						res = await privateClient.catalogCategoryTreeAsync(sessionId, rootCategory, "0").ConfigureAwait(false);
				}).ConfigureAwait(false);

				return new GetCategoryTreeResponse(res);
			}
			catch (Exception exc)
			{
				throw new MagentoSoapException(string.Format("An error occured during GetCategoriesTree({0})", rootCategory), exc);
			}
		}

		public virtual async Task<CatalogProductInfoResponse> GetProductInfoAsync(string skusOrId, string[] custAttributes, bool idPassed = false)
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductInfoResponse();
				var privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

				await ActionPolicies.GetAsync.Do(async () =>
				{
					var statusChecker = new StatusChecker(maxCheckCount);
					TimerCallback tcb = statusChecker.CheckStatus;

					if (privateClient.State != CommunicationState.Opened
						&& privateClient.State != CommunicationState.Created
						&& privateClient.State != CommunicationState.Opening)
						privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

					var sessionId = await this.GetSessionId().ConfigureAwait(false);
					var attributes = new catalogProductRequestAttributes { additional_attributes = custAttributes ?? new string[0] };

					using (var stateTimer = new Timer(tcb, privateClient, 1000, delayBeforeCheck))
						res = await privateClient.catalogProductInfoAsync(sessionId, skusOrId, "0", attributes, idPassed ? "1" : "0").ConfigureAwait(false);
				}).ConfigureAwait(false);

				return new CatalogProductInfoResponse(res);
			}
			catch (Exception exc)
			{
				throw new MagentoSoapException(string.Format("An error occured during GetProductInfoAsync({0})", skusOrId), exc);
			}
		}

		public virtual async Task<CatalogProductAttributeInfoResponse> GetManufacturersInfoAsync(string attribute)
		{
			try
			{
				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductAttributeInfoResponse();
				var privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

				await ActionPolicies.GetAsync.Do(async () =>
				{
					var statusChecker = new StatusChecker(maxCheckCount);
					TimerCallback tcb = statusChecker.CheckStatus;

					if (privateClient.State != CommunicationState.Opened
						&& privateClient.State != CommunicationState.Created
						&& privateClient.State != CommunicationState.Opening)
						privateClient = this.CreateMagentoServiceClient(this.BaseMagentoUrl);

					var sessionId = await this.GetSessionId().ConfigureAwait(false);

					using (var stateTimer = new Timer(tcb, privateClient, 1000, delayBeforeCheck))
						res = await privateClient.catalogProductAttributeInfoAsync(sessionId, attribute).ConfigureAwait(false);
				}).ConfigureAwait(false);

				return new CatalogProductAttributeInfoResponse(res);
			}
			catch (Exception exc)
			{
				throw new MagentoSoapException(string.Format("An error occured during GetManufacturerAsync()"), exc);
			}
		}

	}
}