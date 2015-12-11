using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;

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
		Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, Mark markForLog );
		Task< GetMagentoInfoResponse > GetMagentoInfoAsync();
		string ToJsonSoapInfo();
		Task< bool > PutStockItemAsync( PutStockItem putStockItem, Mark markForLog );
		Task< int > CreateProduct( string storeId, string name, string sku, int isInStock );
	}
}