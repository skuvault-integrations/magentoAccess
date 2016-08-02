using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;

namespace MagentoAccessTests.TestEnvironment
{
	internal class MagentoServiceLowLevelSoapVFrom17To19CeThrowExcetpionStub : MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE
	{
		public MagentoServiceLowLevelSoapVFrom17To19CeThrowExcetpionStub( string apiUser, string apiKey, string baseMagentoUrl, string store ) : base( apiUser, apiKey, baseMagentoUrl, store )
		{
		}

		public override Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException )
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

		public override Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded )
		{
			throw new Exception();
		}

		public override Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			throw new Exception();
		}

		public override Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, Mark markForLog = null )
		{
			throw new Exception();
		}
	}
}