using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.Rest.v1x.GetOrders;
using MagentoAccess.Models.Services.Rest.v1x.GetProduct;
using MagentoAccess.Models.Services.Rest.v1x.GetProducts;
using MagentoAccess.Models.Services.Rest.v1x.GetStockItems;
using MagentoAccess.Models.Services.Rest.v1x.PutStockItems;
using MagentoAccess.Services;
using MagentoAccess.Services.Rest.v1x;
using StockItem = MagentoAccess.Models.Services.Rest.v1x.PutStockItems.StockItem;

namespace MagentoAccessTests.TestEnvironment
{
	internal class MagentoServiceLowLevelRestRestStubThrowExceptionStub : MagentoServiceLowLevelRestRest
	{
		public MagentoServiceLowLevelRestRestStubThrowExceptionStub( string consumerKey, string consumerSecretKey, string baseMagentoUrl, string requestTokenUrl, string authorizeUrl, string accessTokenUrl ) : base( consumerKey, consumerSecretKey, baseMagentoUrl, requestTokenUrl, authorizeUrl, accessTokenUrl )
		{
		}

		public MagentoServiceLowLevelRestRestStubThrowExceptionStub( string consumerKey, string consumerSecretKey, string baseMagentoUrl, string accessToken, string accessTokenSecret ) : base( consumerKey, consumerSecretKey, baseMagentoUrl, accessToken, accessTokenSecret )
		{
		}

		public override Task InitiateDescktopAuthenticationProcess()
		{
			throw new Exception();
		}

		public override Task< GetProductResponse > GetProductAsync( string id )
		{
			throw new Exception();
		}

		public override Task< GetProductsResponse > GetProductsAsync( int page, int limit, bool thrownExc = false )
		{
			throw new Exception();
		}

		public override Task< GetStockItemsResponse > GetStockItemsAsync( int page, int limit )
		{
			throw new Exception();
		}

		public override Task< PutStockItemsResponse > PutStockItemsAsync( IEnumerable< StockItem > inventoryItems, string mark = "" )
		{
			throw new Exception();
		}

		public override Task< GetOrdersResponse > GetOrdersAsync()
		{
			throw new Exception();
		}

		public override Task< GetOrdersResponse > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			throw new Exception();
		}

		public override VerificationData RequestVerificationUri()
		{
			throw new Exception();
		}

		public override void PopulateAccessTokenAndAccessTokenSecret( string verificationCode, string requestToken, string requestTokenSecret )
		{
			throw new Exception();
		}
	}
}