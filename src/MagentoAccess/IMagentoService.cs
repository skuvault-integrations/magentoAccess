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
using MagentoAccess.Models.PingRest;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Services.Rest;

namespace MagentoAccess
{
	public interface IMagentoService
	{
		Task< IEnumerable< Order > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo );

		Task< IEnumerable< Order > > GetOrdersAsync();

		Task UpdateInventoryAsync( IEnumerable< Inventory > products );

		Task< IEnumerable< Product > > GetProductsSimpleAsync();

		Task< IEnumerable< Product > > GetProductsAsync( bool includeDetails = false, string productType = null, bool excludeProductByType = false, DateTime? updatedFrom = null );

		VerificationData RequestVerificationUri();

		void PopulateAccessTokenAndAccessTokenSecret( string verificationCode, string requestToken, string requestTokenSecret );

		Task< PingSoapInfo > PingSoapAsync( Mark mark = null );

		Task< PingRestInfo > PingRestAsync();

		Task UpdateInventoryBySkuAsync( IEnumerable< InventoryBySku > inventory );

		Task< IEnumerable< CreateProductModelResult > > CreateProductAsync( IEnumerable< CreateProductModel > models );

		Task< IEnumerable< DeleteProductModelResult > > DeleteProductAsync( IEnumerable< DeleteProductModel > models );

		Task< IEnumerable< CreateOrderModelResult > > CreateOrderAsync( IEnumerable< CreateOrderModel > models );

		Task< IEnumerable< Order > > GetOrdersAsync( IEnumerable< string > orderIds );

		Task< IEnumerable< Product > > FillProductsDetailsAsync( IEnumerable< Product > products );

		Task< IEnumerable< PingSoapInfo > > DetermineMagentoVersionAsync( Mark mark = null );

		MagentoService.SaveAccessToken AfterGettingToken { get; set; }

		TransmitVerificationCodeDelegate TransmitVerificationCode { get; set; }

		Func< string > AdditionalLogInfo { get; set; }

		Task< PingSoapInfo > DetermineMagentoVersionAndSetupServiceAsync( Mark mark = null );
	}
}