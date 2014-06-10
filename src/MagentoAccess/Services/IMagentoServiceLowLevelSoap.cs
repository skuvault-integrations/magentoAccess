using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Services
{
	internal interface IMagentoServiceLowLevelSoap
	{
		string UserName { get; set; }
		string Password { get; set; }
		string Store { get; set; }
		Task< salesOrderListResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo );
		Task< salesOrderListResponse > GetOrdersAsync( IEnumerable< string > ordersIds );
		Task< catalogProductListResponse > GetProductsAsync();
		Task< catalogInventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds );
	}
}