using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
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

		[ TestCase( "1.2.3.4", Result = "1.2.3.4" ) ]
		[ TestCase( "1.2.3.5", Result = "1.2.3.4" ) ]
		[ TestCase( "1.3.3.5", Result = "1.2.3.4" ) ]
		[ TestCase( "2.2.3.4", Result = "2.2.3.4" ) ]
		[ TestCase( "2.2.4.4", Result = "2.2.3.4" ) ]
		[ TestCase( "3.2.3.4", Result = "3.2.3.4" ) ]
		[ TestCase( "3.2.5.4", Result = "3.2.3.4" ) ]
		[ TestCase( "3.9.9.9", Result = "3.2.3.4" ) ]
		[ Test ]
		public string GetMagentoServiceLowLevel_InputIsCorrectVersion_SimilarOrEvenExectlyTheSameVersionOfServiceFound( string magentoVer )
		{
			//------------ Arrange
			var s1 = new MagentoServiceLowLevelStub( "1.2.3.4" );
			var s2 = new MagentoServiceLowLevelStub( "2.2.3.4" );
			var s3 = new MagentoServiceLowLevelStub( "2.2.3.5" );
			var s4 = new MagentoServiceLowLevelStub( "3.2.3.4" );
			var s5 = new MagentoServiceLowLevelStub( "3.2.4.4" );

			Dictionary< string, IMagentoServiceLowLevelSoap > factories = new Dictionary< string, IMagentoServiceLowLevelSoap >
			{
				{ s1.Store, s1 },
				{ s2.Store, s2 },
				{ s3.Store, s3 },
				{ s4.Store, s4 },
				{ s5.Store, s5 },
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
		}
	}
}