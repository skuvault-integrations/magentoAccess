using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;
using Netco.Logging;

namespace MagentoAccessTests.TestEnvironment
{
	internal class MagentoServiceLowLevelSoapVFrom17To19CeThrowExcetpionStub : MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE
	{
		public MagentoServiceLowLevelSoapVFrom17To19CeThrowExcetpionStub( string apiUser, string apiKey, string baseMagentoUrl, string store ) : base( apiUser, apiKey, baseMagentoUrl, store, 300000, 30, true, new MagentoConfig() )
		{
		}

		public override Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException, Mark mark = null )
		{
			throw new Exception();
		}

		public override Task< OrderInfoResponse > GetOrderAsync( string incrementId, Mark childMark )
		{
			throw new Exception();
		}

		public override Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo, Mark mark = null )
		{
			throw new Exception();
		}

		public override Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds, string searchField )
		{
			throw new Exception();
		}

		public override Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, Mark mark = null )
		{
			throw new Exception();
		}

		public override Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds, IEnumerable< int > scopes, Mark mark = null )
		{
			throw new Exception();
		}

		public override Task< IEnumerable< RpcInvoker.RpcRequestResponse< PutStockItem, object > > > PutStockItemsAsync( List< PutStockItem > stockItems, Mark mark )
		{
			throw new Exception();
		}
	}
}