using System;
using System.Collections.Generic;
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
		Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo );
		Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds );
		Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom );
		Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds );
		Task< OrderInfoResponse > GetOrderAsync( string incrementId );
		Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, Mark markForLog );
		Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException );
		string ToJsonSoapInfo();
		Task< bool > PutStockItemAsync( PutStockItem putStockItem, Mark markForLog );
		Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType );
		Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType );
		Task< int > CreateCart( string storeid );
		Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store );
		Task< bool > ShoppingCartAddressSet( int shoppingCart, string store );
		Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store );
		Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store );
		Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store );
		Task< string > CreateOrder( int shoppingcartid, string store );
		Task< GetSessionIdResponse > GetSessionId( bool throwException = true );

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
		Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts );
	}
}