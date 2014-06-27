using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Services;
using MagentoAccessTestsIntegration.TestEnvironment;
using Netco.Logging;
using Netco.Logging.NLogIntegration;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services
{
	[ TestFixture ]
	public class MeagentoServiceLowLevelSoapTest
	{
		private TestData _testData;
		private MagentoUrls _authorityUrls;
		private MagentoServiceLowLevelSoap _service;
		private MagentoSoapCredentials _soapUserCredentials;
		private int _shoppingCartId;
		private int _customerId;

		[ SetUp ]
		public void Setup()
		{
			this._testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv", @"..\..\Files\magento_VerifierCode.csv" );
			this._soapUserCredentials = this._testData.GetMagentoSoapUser();
			this._authorityUrls = this._testData.GetMagentoUrls();

			this._service = new MagentoServiceLowLevelSoap( this._soapUserCredentials.ApiUser, this._soapUserCredentials.ApiKey, this._authorityUrls.MagentoBaseUrl, null );

			NetcoLogger.LoggerFactory = new NLogLoggerFactory();

			///
			var shoppingCartIdTask = this._service.CreateCart( "0" );
			shoppingCartIdTask.Wait();
			this._shoppingCartId = shoppingCartIdTask.Result;

			//var customerIdTask = this._service.CreateCustomer();
			//customerIdTask.Wait();
			//this._customerId = customerIdTask.Result;
			this._customerId = 6;

			//var shoppingCartCustomerSetTask = this._service.ShoppingCartCustomerSet( this._shoppingCartId, this._customerId, "password", "0" );
			//shoppingCartCustomerSetTask.Wait();
			//var isSuccess = shoppingCartCustomerSetTask.Result;

			var shoppingCartCustomerSetTask = this._service.ShoppingCartGuestCustomerSet( this._shoppingCartId, "max", "qwe@qwe.com", "kits", "0" );
			shoppingCartCustomerSetTask.Wait();
			var isSuccess = shoppingCartCustomerSetTask.Result;

			var shoppingCartAddressSet = this._service.ShoppingCartAddressSet( this._shoppingCartId, "0" );
			shoppingCartAddressSet.Wait();
			var isSuccess2 = shoppingCartAddressSet.Result;
			//
		}

		[ TearDown ]
		public void TearDown()
		{
			///
			//var customerIdTask = this._service.DeleteCustomer(this._customerId);
			//
		}

		[ Test ]
		public void GetOrders_StoreContainsOrders_ReceiveOrders()
		{
			//------------ Arrange

			var shippingMenthodTask = this._service.ShoppingCartSetShippingMethod( this._shoppingCartId, "0" );
			shippingMenthodTask.Wait();
			var isshippingMenthodTaskSuccess = shippingMenthodTask.Result;

			var paymentMenthodTask = this._service.ShoppingCartSetPaymentMethod( this._shoppingCartId, "0" );
			paymentMenthodTask.Wait();
			var ispaymentMenthodTaskSuccess = paymentMenthodTask.Result;

			var productTask = this._service.ShoppingCartAddProduct( this._shoppingCartId, "1", "0" );
			productTask.Wait();
			var isSuccess = productTask.Result;

			var orderIdTask = this._service.CreateOrder( this._shoppingCartId, "0" );
			orderIdTask.Wait();
			var orderId = orderIdTask.Result;

			//------------ Act
			var modifiedFrom = DateTime.Parse( "2014-05-08 15:02:58" );
			var modifiedTo = DateTime.Parse( "2014-05-28 10:48:52" );
			var getOrdersTask = this._service.GetOrdersAsync( modifiedFrom, modifiedTo );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.result.Should().NotBeEmpty();
		}

		[ Test ]
		public void GetOrders_ByIdsStoreContainsOrders_ReceiveOrders()
		{
			//------------ Arrange

			//------------ Act
			var ordersIds = new List< string >() { "100000001", "100000002" };

			var getOrdersTask = this._service.GetOrdersAsync( ordersIds );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.result.Should().NotBeEmpty();
		}

		[ Test ]
		public void GetProducts_StoreContainsProducts_ReceiveProducts()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._service.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.result.Should().NotBeEmpty();
		}

		[ Test ]
		public void GetStockItems_StoreContainsStockItems_ReceiveStockItems()
		{
			//------------ Arrange

			//------------ Act
			var skusorids = new List< string >() { "501shirt", "311" };

			var getProductsTask = this._service.GetStockItemsAsync( skusorids );
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.result.Should().NotBeEmpty();
		}

		[ Test ]
		public void GetSessionId_StoreContainsUser_ReceiveSessionId()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._service.GetSessionId( false );
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull();
		}

		[ Test ]
		public void GetSessionId_IncorrectApiUser_NoExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act

			Action act = () =>
			{
				var service = new MagentoServiceLowLevelSoap(
					"incorrect api user",
					this._testData.GetMagentoSoapUser().ApiKey,
					this._testData.GetMagentoUrls().MagentoBaseUrl,
					null );

				var getProductsTask = service.GetSessionId( false );
				getProductsTask.Wait();
			};

			//------------ Assert

			act.ShouldNotThrow();
		}

		[ Test ]
		public void UpdateInventory_StoreWithItems_ItemsUpdated()
		{
			//------------ Arrange

			//------------ Act

			var productsAsync = this._service.GetProductsAsync();
			productsAsync.Wait();

			var itemsToUpdate = productsAsync.Result.result.Select( x => new PutStockItem( x.product_id, new catalogInventoryStockItemUpdateEntity() { qty = "123" } ) ).ToList();

			var getProductsTask = this._service.PutStockItemsAsync( itemsToUpdate );
			getProductsTask.Wait();

			//------------ Assert
		}

		[ Test ]
		public void GetMagentoInfoAsync_StoreExist_StoreVersionRecived()
		{
			//------------ Arrange

			//------------ Act

			var productsAsync = this._service.GetMagentoInfoAsync();
			productsAsync.Wait();

			//------------ Assert
			productsAsync.Result.result.magento_version.Should().NotBeNullOrWhiteSpace();
		}
	}
}