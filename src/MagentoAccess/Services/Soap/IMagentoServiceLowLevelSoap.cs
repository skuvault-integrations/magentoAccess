using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.SOAP.GetInventory;
using MagentoAccess.Models.Services.SOAP.GetMagentoInfo;
using MagentoAccess.Models.Services.SOAP.GetOrders;
using MagentoAccess.Models.Services.SOAP.GetProducts;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;

namespace MagentoAccess.Services.Soap
{
	internal interface IMagentoServiceLowLevelSoap
	{
		string ApiUser { get; }
		string ApiKey { get; }
		string Store { get; }
		Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo );
		Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds );
		Task< SoapGetProductsResponse > GetProductsAsync();
		Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds );
		Task< OrderInfoResponse > GetOrderAsync( string incrementId );
		Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, string markForLog );
		Task< GetMagentoInfoResponse > GetMagentoInfoAsync();
		string ToJsonSoapInfo();
		Task< bool > PutStockItemAsync( PutStockItem putStockItem, string markForLog );
	}
}