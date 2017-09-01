using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.CreateOrders;
using MagentoAccess.Models.CreateProducts;
using MagentoAccess.Models.DeleteProducts;
using MagentoAccess.Models.GetMagentoCoreInfo;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutInventory;

namespace MagentoAccess
{
	public interface IMagentoService
	{
		Task< IEnumerable< Order > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo, Mark mark2 = null );

		Task UpdateInventoryAsync( IEnumerable< Inventory > products, Mark mark = null );

		Task< IEnumerable< Product > > GetProductsAsync( IEnumerable< int > scopes = null, bool includeDetails = false, string productType = null, bool excludeProductByType = false, DateTime? updatedFrom = null, IEnumerable< string > skus = null, bool stockItemsOnly = true, Mark mark = null );

		Task< PingSoapInfo > PingSoapAsync( Mark mark = null );

		Task UpdateInventoryBySkuAsync( IEnumerable< InventoryBySku > inventory, IEnumerable< int > scopes = null );

		Task< IEnumerable< CreateProductModelResult > > CreateProductAsync( IEnumerable< CreateProductModel > models );

		Task< IEnumerable< DeleteProductModelResult > > DeleteProductAsync( IEnumerable< DeleteProductModel > models );

		Task< IEnumerable< CreateOrderModelResult > > CreateOrderAsync( IEnumerable< CreateOrderModel > models );

		Task< IEnumerable< Order > > GetOrdersAsync( IEnumerable< string > orderIds );

		Task< IEnumerable< Product > > FillProductsDetailsAsync( IEnumerable< Product > products );

		Task< IEnumerable< PingSoapInfo > > DetermineMagentoVersionAsync( Mark mark = null );

		MagentoService.SaveAccessToken AfterGettingToken { get; set; }

		Func< string > AdditionalLogInfo { get; set; }

		Task< PingSoapInfo > DetermineMagentoVersionAndSetupServiceAsync( Mark mark = null );
		Task< bool > InitAsync( bool supressExc = false );
	}
}