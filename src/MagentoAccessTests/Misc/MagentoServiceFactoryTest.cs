﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.GetShipments;
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
using MagentoAccess.Services.Soap;
using Netco.Logging;
using NUnit.Framework;

namespace MagentoAccessTests.Misc
{
	[ TestFixture ]
	internal class MagentoServiceFactoryTest
	{
		[ TestCase( 0, "1.2.3.4", ExpectedResult = "1" ) ]
		[ TestCase( 3, "1.2.3.4", ExpectedResult = "4" ) ]
		[ TestCase( 0, "11.2.3.4", ExpectedResult = "11" ) ]
		[ TestCase( 3, "11.2.3.44", ExpectedResult = "44" ) ]
		[ Test ]
		public string GetMagentoSubVersion_InputIsCorrectVersion_SubversionReturned( int deep, string magentoVer )
		{
			//------------ Arrange
			var magentoServiceLowLevelSoapFactory = new MagentoServiceLowLevelSoapFactory( null, null, null, null );

			//------------ Act
			var version = magentoServiceLowLevelSoapFactory.GetSubVersion( deep, magentoVer );
			//------------ Assert
			return version;
		}

		[TestCase("1.7.0.2", ExpectedResult = "1.7.0.2")]
		[TestCase("1.7.1.2", ExpectedResult = "1.7.0.2")]
		[TestCase("1.8.1.2", ExpectedResult = "1.8.1.0")]
		[TestCase("1.8.0.2", ExpectedResult = "1.8.0.1")]
		[TestCase("1.9.1.3", ExpectedResult = "1.9.1.0")]
		[TestCase("1.9.2.3", ExpectedResult = "1.9.2.0")]
		[ Test ]
		public string GetMagentoServiceLowLevel_InputIsCorrectVersion_SimilarOrEvenExectlyTheSameVersionOfServiceFound( string magentoVer )
		{
			//------------ Arrange
			var s1 = new MagentoServiceLowLevelStub( "1.7.0.2" );
			var s2 = new MagentoServiceLowLevelStub( "1.8.0.1" );
			var s3 = new MagentoServiceLowLevelStub( "1.8.1.0" );
			var s4 = new MagentoServiceLowLevelStub( "1.9.0.1" );
			var s5 = new MagentoServiceLowLevelStub( "1.9.1.0" );
			var s6 = new MagentoServiceLowLevelStub( "1.9.2.0" );
			var s7 = new MagentoServiceLowLevelStub( "1.9.2.1" );
			var s8= new MagentoServiceLowLevelStub( "1.9.2.2" );

			var factories = new Dictionary< string, IMagentoServiceLowLevelSoap >
			{
				{ s1.Store, s1 },
				{ s2.Store, s2 },
				{ s3.Store, s3 },
				{ s4.Store, s4 },
				{ s5.Store, s5 },
				{ s6.Store, s6 },
				{ s7.Store, s7 },
				{ s8.Store, s8 },
			};
			var magentoServiceLowLevelSoapFactory = new MagentoServiceLowLevelSoapFactory( new MagentoAuthenticatedUserCredentials( "", "", "http://base.url", "", "", "", "", 0, 0, false ), factories, null, null );

			//------------ Act
			var magentoServiceLowLevelSoap = magentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( magentoVer, true, false );
			//------------ Assert
			return magentoServiceLowLevelSoap.Store;
		}

		public class MagentoServiceLowLevelStub : IMagentoServiceLowLevelSoap
		{
			public string ApiUser => null;
			public string ApiKey => null;
			public string DefaultApiUrl => "";
			public string StoreVersion { get; set; }
			public string Store { get; private set; }

			public MagentoServiceLowLevelStub( string store )
			{
				this.Store = store;
			}

			public bool GetStockItemsWithoutSkuImplementedWithPages
			{
				get { return false; }
			}

			public bool GetOrderByIdForFullInformation => false;

			public bool GetOrdersUsesEntityInsteadOfIncrementId => false;

			public string GetServiceVersion()
			{
				return string.Empty;
			}

			public DateTime? LastActivityTime
			{
				get { return null; }
			}

			public Task< bool > InitAsync( bool suppressExceptions = false )
			{
				return Task.FromResult( true );
			}

			public Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo, CancellationToken cancellationToken, Mark mark = null )
			{
				return null;
			}

			public Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds, CancellationToken cancellationToken, string searchField )
			{
				return null;
			}

			public Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, CancellationToken cancellationToken, Mark mark = null )
			{
				return null;
			}

			public Task< SoapGetProductsResponse > GetProductsBySkusAsync(  IEnumerable< string > skus, CancellationToken cancellationToken, Mark mark = null )
			{
				return null;
			}

			public Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds, IEnumerable< int > scopes, CancellationToken cancellationToken, Mark mark = null )
			{
				return null;
			}

			public Task< OrderInfoResponse > GetOrderAsync( string incrementId, CancellationToken cancellationToken, Mark childMark )
			{
				return null;
			}

			public Task< OrderInfoResponse > GetOrderAsync( Order order, CancellationToken cancellationToken, Mark childMark )
			{
				return null;
			}

			Task< IEnumerable< RpcInvoker.RpcRequestResponse< PutStockItem, object > > > IMagentoServiceLowLevelSoap.PutStockItemsAsync( List< PutStockItem > stockItems, CancellationToken cancellationToken, Mark mark )
			{
				throw new NotImplementedException();
			}

			public Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException, CancellationToken cancellationToken, Mark mark = null )
			{
				return null;
			}

			public string ToJsonSoapInfo()
			{
				return null;
			}

			public Task< bool > PutStockItemAsync( PutStockItem putStockItem, CancellationToken cancellationToken, Mark markForLog )
			{
				return null;
			}

			public Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType, CancellationToken cancellationToken, Mark markForLog )
			{
				return null;
			}

			public Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< int > CreateCart( string storeid, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< bool > ShoppingCartAddressSet( int shoppingCart, string store, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< string > CreateOrder( int shoppingcartid, string store, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< GetSessionIdResponse > GetSessionId( CancellationToken cancellationToken, bool throwException = true )
			{
				return null;
			}

			public Task< CatalogProductInfoResponse > GetProductInfoAsync( CatalogProductInfoRequest catalogProductInfoRequest, CancellationToken cancellationToken, bool throwException = true )
			{
				return null;
			}

			public Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, CancellationToken cancellationToken, bool throwException = true )
			{
				return null;
			}

			public Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( CancellationToken cancellationToken, string rootCategory = "1" )
			{
				return null;
			}

			public Task< CatalogProductAttributeInfoResponse > GetManufacturersInfoAsync( string attribute, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts, CancellationToken cancellationToken )
			{
				return null;
			}

			public Task< InventoryStockItemListResponse > GetStockItemsWithoutSkuAsync( IEnumerable< string > skusOrIds, IEnumerable< int > scopes, CancellationToken cancellationToken, Mark mark = null )
			{
				throw new NotImplementedException();
			}

			public Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, IReadOnlyCollection< string > skus )
			{
				throw new NotImplementedException();
			}

			public Task<Dictionary< string, IEnumerable< Shipment > > > GetOrdersShipmentsAsync( DateTime modifiedFrom, DateTime modifiedTo, CancellationToken token, Mark mark = null )
			{
				throw new NotImplementedException();
			}
		}
	}
}