using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetSessionId;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;

namespace MagentoAccess.Services.Soap
{
	internal interface IMagentoServiceLowLevelSoap
	{
		string ApiUser { get; }
		string ApiKey { get; }
		string Store { get; }
		string StoreVersion { get; set; }
		bool GetStockItemsWithoutSkuImplementedWithPages { get; }
		bool GetOrderByIdForFullInformation { get; }
		bool GetOrdersUsesEntityInsteadOfIncrementId { get; }
		Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo, Mark mark = null );
		Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds );
		Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, Mark mark = null );
		Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds, IEnumerable< int > scopes, Mark mark = null );
		Task< OrderInfoResponse > GetOrderAsync( string incrementId );
		Task< OrderInfoResponse > GetOrderAsync( Order order );
		Task< IEnumerable< RpcInvoker.RpcRequestResponse< PutStockItem, object > > > PutStockItemsAsync( List< PutStockItem > stockItems, Mark mark = null );
		Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException, CancellationToken ctx, Mark mark = null );
		string ToJsonSoapInfo();
		Task< bool > PutStockItemAsync( PutStockItem putStockItem, Mark markForLog );
		Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType, Mark markForLog = null );
		Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType );
		Task< int > CreateCart( string storeid );
		Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store );
		Task< bool > ShoppingCartAddressSet( int shoppingCart, string store );
		Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store );
		Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store );
		Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store );
		Task< string > CreateOrder( int shoppingcartid, string store );
		Task< GetSessionIdResponse > GetSessionId( CancellationToken ctx = default(CancellationToken), bool throwException = true );

		/// <summary>
		/// Provides additional information about product. (Description,ShortDescription,Price,SpecialPrice,Weight,ProductId,CategoryIds)
		/// </summary>
		/// <param name="catalogProductInfoRequest"></param>
		/// <param name="throwException"></param>
		/// <returns></returns>
		Task< CatalogProductInfoResponse > GetProductInfoAsync( CatalogProductInfoRequest catalogProductInfoRequest, bool throwException = true );

		Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, bool throwException = true );
		Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( string rootCategory = "1" );
		Task< CatalogProductAttributeInfoResponse > GetManufacturersInfoAsync( string attribute );
		Task< InventoryStockItemListResponse > GetStockItemsWithoutSkuAsync( IEnumerable< string > skusOrIds, IEnumerable< int > scopes, Mark mark = null );
		Task< bool > InitAsync( bool supressExceptions = false );
	}

	internal interface IMagentoServiceLowLevelSoapGetProductsBySku
	{
		Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, IReadOnlyCollection< string > skus, Mark mark = null );
	}

	internal interface IMagentoServiceLowLevelSoapFillProductsDetails
	{
		Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts );
	}
}