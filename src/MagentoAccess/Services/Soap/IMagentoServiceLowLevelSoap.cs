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
using Netco.Logging;
using MagentoAccess.Models.GetShipments;

namespace MagentoAccess.Services.Soap
{
	internal interface IMagentoServiceLowLevelSoap
	{
		string ApiUser { get; }
		string ApiKey { get; }
		string Store { get; }

		/// <summary>
		/// Default REST/SOAP Api Url
		/// </summary>
		string DefaultApiUrl { get; }
		
		string StoreVersion { get; set; }
		bool GetStockItemsWithoutSkuImplementedWithPages { get; }
		bool GetOrderByIdForFullInformation { get; }
		bool GetOrdersUsesEntityInsteadOfIncrementId { get; }
		/// <summary>
		///	This property can be used by the client to monitor the last access library's network activity time.
		/// </summary>
		DateTime? LastActivityTime { get; }

		Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo, CancellationToken cancellationToken, Mark mark = null );
		Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds, CancellationToken cancellationToken, string searchField = "increment_id" );
		Task< Dictionary< string, IEnumerable< Shipment > > > GetOrdersShipmentsAsync( DateTime modifiedFrom, DateTime modifiedTo, CancellationToken cancellationToken, Mark mark = null );
		Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, CancellationToken cancellationToken, Mark mark = null );
		Task< SoapGetProductsResponse > GetProductsBySkusAsync( IEnumerable< string > skus, CancellationToken cancellationToken, Mark mark = null );
		Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds, IEnumerable< int > scopes, CancellationToken cancellationToken, Mark mark = null );
		Task< OrderInfoResponse > GetOrderAsync( string incrementId, CancellationToken cancellationToken, Mark childMark );
		Task< OrderInfoResponse > GetOrderAsync( Order order, CancellationToken cancellationToken, Mark childMark );
		Task< IEnumerable< RpcInvoker.RpcRequestResponse< PutStockItem, object > > > PutStockItemsAsync( List< PutStockItem > stockItems, CancellationToken cancellationToken, Mark mark = null );
		Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException, CancellationToken cancellationToken, Mark mark = null );
		string ToJsonSoapInfo();
		Task< bool > PutStockItemAsync( PutStockItem putStockItem, CancellationToken cancellationToken, Mark markForLog );
		Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType, CancellationToken cancellationToken, Mark markForLog = null );
		Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType, CancellationToken token );
		Task< int > CreateCart( string storeid, CancellationToken token );
		Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store, CancellationToken token );
		Task< bool > ShoppingCartAddressSet( int shoppingCart, string store, CancellationToken token );
		Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store, CancellationToken token );
		Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store, CancellationToken token );
		Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store, CancellationToken token );
		Task< string > CreateOrder( int shoppingcartid, string store, CancellationToken token );
		Task< GetSessionIdResponse > GetSessionId( CancellationToken token, bool throwException = true );
		string GetServiceVersion();

		/// <summary>
		/// Provides additional information about product. (Description,ShortDescription,Price,SpecialPrice,Weight,ProductId,CategoryIds)
		/// </summary>
		/// <param name="catalogProductInfoRequest"></param>
		/// <param name="throwException"></param>
		/// <returns></returns>
		Task< CatalogProductInfoResponse > GetProductInfoAsync( CatalogProductInfoRequest catalogProductInfoRequest, CancellationToken token, bool throwException = true );

		Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, CancellationToken token, bool throwException = true );
		Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( CancellationToken token, string rootCategory = "1" );
		Task< CatalogProductAttributeInfoResponse > GetManufacturersInfoAsync( string attribute, CancellationToken token );
		Task< InventoryStockItemListResponse > GetStockItemsWithoutSkuAsync( IEnumerable< string > skusOrIds, IEnumerable< int > scopes, CancellationToken cancellationToken, Mark mark = null );
		Task< bool > InitAsync( bool suppressExceptions = false );
	}

	internal interface IMagentoServiceLowLevelSoapGetProductsBySku
	{
		Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, IReadOnlyCollection< string > skus, CancellationToken cancellationToken, Mark mark = null );
	}

	internal interface IMagentoServiceLowLevelSoapFillProductsDetails
	{
		Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts, CancellationToken cancellationToken );
	}
}