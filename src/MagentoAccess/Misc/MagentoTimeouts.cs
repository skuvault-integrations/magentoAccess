using System;
using System.Collections.Generic;

namespace MagentoAccess.Misc
{
	public enum MagentoOperationEnum
	{
		/// <summary>
		///	Get authorization token
		///		API: GET /V1/integration/admin/token
		/// </summary>
		GetToken,
		/// <summary>
		///	Get orders by ids
		///		API: GET /V1/orders?searchCriteria[filterGroups][0][filters][0][field]=increment_id
		///						&searchCriteria[filterGroups][0][filters][0][value]={orderId},{orderId2},...
		///						&searchCriteria[filterGroups][0][filters][0][conditionType]=in
		/// </summary>
		GetOrdersByIds,
		/// <summary>
		///	Get modified orders
		///		API: GET /V1/orders?searchCriteria[filterGroups][0][filters][0][field]=updated_at
		///						&searchCriteria[filterGroups][0][filters][0][value]={updatedDateFrom}
		///						&searchCriteria[filterGroups][0][filters][0][conditionType]=from
		///						&searchCriteria[filterGroups][1][filters][0][field]=updated_at
		///						&searchCriteria[filterGroups][1][filters][0][value]={updatedDateTo}
		///						&searchCriteria[filterGroups][1][filters][0][conditionType]=to
		/// </summary>
		GetModifiedOrders,
		/// <summary>
		///	Get products updated at date greater than specified
		///		API: GET /V1/products?searchCriteria[filterGroups][0][filters][0][field]=type_id
		///						&searchCriteria[filterGroups][0][filters][0][value]={productTypeId}
		///						&searchCriteria[filterGroups][0][filters][0][conditionType]=eq|neq
		///						&searchCriteria[filterGroups][1][filters][0][field]=updated_at
		///						&searchCriteria[filterGroups][1][filters][0][value]={updatedAt}
		///						&searchCriteria[filterGroups][1][filters][0][conditionType]=gt
		/// </summary>
		GetFilteredProducts,
		/// <summary>
		///	Get product details by sku
		///		API: GET /V1/products/{sku}
		/// </summary>
		GetProductBySku,
		/// <summary>
		///	Get products details by skus
		///		API: GET /V1/products?searchCriteria[filterGroups][0][filters][0][field]=sku
		///						&searchCriteria[filterGroups][0][filters][0][value]={sku,sku2,...skuN}
		///						&searchCriteria[filterGroups][0][filters][0][conditionType]=in
		/// </summary>
		GetProductsBySkus,
		/// <summary>
		///		API: GET /V1/categories	
		/// </summary>
		GetProductsCategories,
		/// <summary>
		///	Get products manufacturers
		///		API: GET /V1/products/attributes/manufacturer
		/// </summary>
		GetProductsManufacturers,
		/// <summary>
		///	Get stock item's quantity
		///		API: GET /V1/stockitems/{sku}
		/// </summary>
		GetStockItemQuantity,
		/// <summary>
		///	Update stock item's quantity
		///		API: PUT /V1/products/{sku}/stockItems/{itemId}
		/// </summary>
		UpdateStockItemQuantity
	}

	public class MagentoOperationTimeout
	{
		public int TimeoutInMs { get; private set; }

		public MagentoOperationTimeout( int timeoutInMs )
		{
			if( timeoutInMs <= 0 )
			{
				throw new ArgumentOutOfRangeException( nameof(timeoutInMs), timeoutInMs, "timeoutInMs must be greater than 0" );
			}

			this.TimeoutInMs = timeoutInMs;
		}
	}

	public class MagentoTimeouts
	{
		public const int DefaultTimeoutInMs = 5 * 60 * 1000;
		private Dictionary< MagentoOperationEnum, MagentoOperationTimeout > _timeouts;

		/// <summary>
		///	This timeout value will be used if specific timeout for operation is not provided. Default value can be changed through constructor.
		/// </summary>
		public MagentoOperationTimeout DefaultOperationTimeout { get; private set; }

		public int this[ MagentoOperationEnum operation ]
		{
			get
			{
				MagentoOperationTimeout timeout;
				if ( _timeouts.TryGetValue( operation, out timeout ) )
					return timeout.TimeoutInMs;

				return DefaultOperationTimeout.TimeoutInMs;
			}
		}

		public void Set( MagentoOperationEnum operation, MagentoOperationTimeout timeout )
		{
			_timeouts[ operation ] = timeout;
		}

		public MagentoTimeouts( int defaultTimeoutInMs )
		{
			_timeouts = new Dictionary< MagentoOperationEnum, MagentoOperationTimeout >();
			this.DefaultOperationTimeout = new MagentoOperationTimeout( defaultTimeoutInMs );
		}

		public MagentoTimeouts() : this( DefaultTimeoutInMs ) { }
	}
}