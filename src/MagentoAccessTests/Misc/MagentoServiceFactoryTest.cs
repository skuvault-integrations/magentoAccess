using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services.Soap;
using NUnit.Framework;

namespace MagentoAccessTests.Misc
{
	[ TestFixture ]
	internal class MagentoServiceFactoryTest
	{
		[ TestCase( 0, "1.2.3.4", Result = "1" ) ]
		[ TestCase( 3, "1.2.3.4", Result = "4" ) ]
		[ TestCase( 0, "11.2.3.4", Result = "11" ) ]
		[ TestCase( 3, "11.2.3.44", Result = "44" ) ]
		[ Test ]
		public string GetMagentoSubVersion_InputIsCorrectVersion_SubversionReturned( int deep, string magentoVer )
		{
			//------------ Arrange
			var magentoServiceLowLevelSoapFactory = new MagentoServiceLowLevelSoapFactory( null, null, null, null, null );

			//------------ Act
			var version = magentoServiceLowLevelSoapFactory.GetSubVersion( deep, magentoVer );
			//------------ Assert
			return version;
		}

		[TestCase("1.7.0.2", Result = "1.7.0.2")]
		[TestCase("1.7.1.2", Result = "1.7.0.2")]
		[TestCase("1.8.1.2", Result = "1.8.1.0")]
		[TestCase("1.8.0.2", Result = "1.8.0.1")]
		[TestCase("1.9.1.3", Result = "1.9.1.0")]
		[TestCase("1.9.2.3", Result = "1.9.2.0")]
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

			Dictionary< string, IMagentoServiceLowLevelSoap > factories = new Dictionary< string, IMagentoServiceLowLevelSoap >
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
			var magentoServiceLowLevelSoapFactory = new MagentoServiceLowLevelSoapFactory( null, null, null, null, factories );

			//------------ Act
			var magentoServiceLowLevelSoap = magentoServiceLowLevelSoapFactory.GetMagentoServiceLowLevelSoap( magentoVer, true );
			//------------ Assert
			return magentoServiceLowLevelSoap.Store;
		}

		public class MagentoServiceLowLevelStub : IMagentoServiceLowLevelSoap
		{
			public string ApiUser
			{
				get { return null; }
			}

			public string ApiKey
			{
				get { return null; }
			}

			public string Store { get; private set; }

			public MagentoServiceLowLevelStub( string store )
			{
				Store = store;
			}

			public Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
			{
				return null;
			}

			public Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
			{
				return null;
			}

			public Task< SoapGetProductsResponse > GetProductsAsync()
			{
				return null;
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

			public Task< GetMagentoInfoResponse > GetMagentoInfoAsync()
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

			public Task< int > CreateProduct( string storeId, string name, string sku, int isInStock )
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

			public Task< string > GetSessionId( bool throwException = true )
			{
				return null;
			}

			public Task< CatalogProductInfoResponse > GetProductInfoAsync( string skusOrId, bool idPassed = false )
			{
				return null;
			}

			public Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( string productId )
			{
				return null;
			}

			public Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( string rootCategory = "1" )
			{
				return null;
			}
		}
	}
}