using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.SOAP.GetInventory;
using MagentoAccess.Models.Services.SOAP.GetMagentoInfo;
using MagentoAccess.Models.Services.SOAP.GetOrders;
using MagentoAccess.Models.Services.SOAP.GetProducts;
using MagentoAccess.Services;

namespace MagentoAccessTests.TestEnvironment
{
	internal class MagentoServiceLowLevelSoapThrowExcetpionStub : MagentoServiceLowLevelSoap
	{
		public MagentoServiceLowLevelSoapThrowExcetpionStub( string apiUser, string apiKey, string baseMagentoUrl, string store ) : base( apiUser, apiKey, baseMagentoUrl, store )
		{
		}

		public override Task< GetMagentoInfoResponse > GetMagentoInfoAsync()
		{
			throw new Exception();
		}

		public override Task< OrderInfoResponse > GetOrderAsync( string incrementId )
		{
			throw new Exception();
		}

		public override Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			throw new Exception();
		}

		public override Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			throw new Exception();
		}

		public override Task< SoapGetProductsResponse > GetProductsAsync()
		{
			throw new Exception();
		}

		public override Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			throw new Exception();
		}

		public override Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, string markForLog = "" )
		{
			throw new Exception();
		}
	}
}