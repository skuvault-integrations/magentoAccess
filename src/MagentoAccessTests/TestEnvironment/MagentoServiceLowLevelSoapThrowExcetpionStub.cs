using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Models.Services.SOAP.GetOrders;
using MagentoAccess.Services;

namespace MagentoAccessTests.TestEnvironment
{
	internal class MagentoServiceLowLevelSoapThrowExcetpionStub : MagentoServiceLowLevelSoap
	{
		public MagentoServiceLowLevelSoapThrowExcetpionStub( string apiUser, string apiKey, string baseMagentoUrl, string store ) : base( apiUser, apiKey, baseMagentoUrl, store )
		{
		}

		public override Task< magentoInfoResponse > GetMagentoInfoAsync()
		{
			throw new Exception();
		}

		public override Task< salesOrderInfoResponse > GetOrderAsync( string incrementId )
		{
			throw new Exception();
		}

		public override Task< salesOrderListResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			throw new Exception();
		}

		public override Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			throw new Exception();
		}

		public override Task< catalogProductListResponse > GetProductsAsync()
		{
			throw new Exception();
		}

		public override Task< catalogInventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			throw new Exception();
		}

		public override Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, string markForLog = "" )
		{
			throw new Exception();
		}
	}
}