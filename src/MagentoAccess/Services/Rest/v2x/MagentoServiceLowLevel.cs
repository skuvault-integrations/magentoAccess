using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Rest.v2x.Products;
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
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccess.Services.Soap;
using Newtonsoft.Json;
using SearchCriteria = MagentoAccess.Models.Services.Rest.v2x.SearchCriteria;

namespace MagentoAccess.Services.Rest.v2x
{
	internal class MagentoServiceLowLevel : IMagentoServiceLowLevelSoap
	{
		public string ApiUser { get; }
		public string ApiKey { get; }
		public string Store { get; }
		public string StoreVersion { get; set; }
		protected IProductRepository ProductRepository { get;set;}
		protected IntegrationAdminTokenRepository IntegrationAdminTokenRepository { get;set;}

		public Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			return null;
		}

		public Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			return null;
		}

		public async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom )
		{
			var products = await this.ProductRepository.GetProductsAsync( updatedFrom ?? DateTime.MinValue, productType, productTypeShouldBeExcluded ).ConfigureAwait(false);
			return new SoapGetProductsResponse(products.SelectMany(x=>x.items).ToList());
		}

		public Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds )
		{
			return null;
		}

		public Task< OrderInfoResponse > GetOrderAsync( string incrementId )
		{
			return null;
		}

		public Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, Mark markForLog )
		{
			return null;
		}

		public Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException )
		{
			return null;
		}

		public string ToJsonSoapInfo()
		{
			return null;
		}

		public Task< bool > PutStockItemAsync( PutStockItem putStockItem, Mark markForLog )
		{
			return null;
		}

		public Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType, Mark markForLog = null )
		{
			return null;
		}

		public Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType )
		{
			return null;
		}

		public Task< int > CreateCart( string storeid )
		{
			return null;
		}

		public Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store )
		{
			return null;
		}

		public Task< bool > ShoppingCartAddressSet( int shoppingCart, string store )
		{
			return null;
		}

		public Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store )
		{
			return null;
		}

		public Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store )
		{
			return null;
		}

		public Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store )
		{
			return null;
		}

		public Task< string > CreateOrder( int shoppingcartid, string store )
		{
			return null;
		}

		public Task< GetSessionIdResponse > GetSessionId( bool throwException = true )
		{
			return null;
		}

		public Task< CatalogProductInfoResponse > GetProductInfoAsync( CatalogProductInfoRequest catalogProductInfoRequest, bool throwException = true )
		{
			return null;
		}

		public Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, bool throwException = true )
		{
			return null;
		}

		public Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( string rootCategory = "1" )
		{
			return null;
		}

		public Task< CatalogProductAttributeInfoResponse > GetManufacturersInfoAsync( string attribute )
		{
			return null;
		}

		public Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts )
		{
			return null;
		}
	}
}