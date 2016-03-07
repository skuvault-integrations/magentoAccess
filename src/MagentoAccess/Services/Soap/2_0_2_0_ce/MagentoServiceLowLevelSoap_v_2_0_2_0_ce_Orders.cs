using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Magento2salesOrderRepositoryV1_v_2_0_2_0_CE;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetOrders;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_2_0_2_0_ce: IMagentoServiceLowLevelSoap
	{
		public virtual async Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			try
			{
				var frameworkSearchFilterGroups = new List< FrameworkSearchFilterGroup >
				{
					new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { field = "updated_At", conditionType = "gt", value = modifiedFrom.ToSoapParameterString() } } },
					new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { field = "updated_At", conditionType = "lt", value = modifiedTo.ToSoapParameterString() } } },
				};
				if( string.IsNullOrWhiteSpace( this.Store ) )
					frameworkSearchFilterGroups.Add( new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { field = "store_Id", conditionType = "eq", value = this.Store } } } );

				var filters = new SalesOrderRepositoryV1GetListRequest
				{
					searchCriteria = new FrameworkSearchCriteria()
					{
						currentPage = 1,
						currentPageSpecified = true,
						filterGroups = frameworkSearchFilterGroups.ToArray(),
						sortOrders = new FrameworkSortOrder[] { new FrameworkSortOrder() { direction = "ASC", field = "Id" } },
						pageSize = 100,
						pageSizeSpecified = true,
					}
				};

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new salesOrderRepositoryV1GetListResponse1();

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
						res = await privateClient.salesOrderRepositoryV1GetListAsync( filters ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new GetOrdersResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrdersAsync(modifiedFrom:{0},modifiedTo{1})", modifiedFrom, modifiedTo ), exc );
			}
		}

		public virtual async Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			var ordersIdsAgregated = string.Empty;
			try
			{
				ordersIdsAgregated = string.Join( ",", ordersIds );
				var frameworkSearchFilterGroups = new List< FrameworkSearchFilterGroup >
				{
					new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { field = "increment_id", conditionType = "eq", value = ordersIdsAgregated } } },
				};
				if( string.IsNullOrWhiteSpace( this.Store ) )
					frameworkSearchFilterGroups.Add( new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { field = "store_Id", conditionType = "eq", value = this.Store } } } );

				var filters = new SalesOrderRepositoryV1GetListRequest
				{
					searchCriteria = new FrameworkSearchCriteria()
					{
						currentPage = 1,
						currentPageSpecified = true,
						filterGroups = frameworkSearchFilterGroups.ToArray(),
						sortOrders = new FrameworkSortOrder[] { new FrameworkSortOrder() { direction = "ASC", field = "Id" } },
						pageSize = 100,
						pageSizeSpecified = true,
					}
				};

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new salesOrderRepositoryV1GetListResponse1();

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
						res = await privateClient.salesOrderRepositoryV1GetListAsync( filters ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new GetOrdersResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrdersAsync({0})", ordersIdsAgregated ), exc );
			}
		}

		public virtual async Task< OrderInfoResponse > GetOrderAsync( string incrementId )
		{
			try
			{
				//var frameworkSearchFilterGroups = new List< FrameworkSearchFilterGroup >
				//{
				//	new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { field = "increment_id", conditionType = "eq", value = incrementId } } },
				//};
				//if( string.IsNullOrWhiteSpace( this.Store ) )
				//	frameworkSearchFilterGroups.Add( new FrameworkSearchFilterGroup() { filters = new[] { new FrameworkFilter() { field = "store_Id", conditionType = "eq", value = this.Store } } } );

				var filters = new SalesOrderRepositoryV1GetRequest
				{
					id = int.Parse( incrementId )
				};

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new salesOrderRepositoryV1GetResponse1();

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
						res = await privateClient.salesOrderRepositoryV1GetAsync( filters ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new OrderInfoResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetOrderAsync(incrementId:{0})", incrementId ), exc );
			}
		}
	}
}