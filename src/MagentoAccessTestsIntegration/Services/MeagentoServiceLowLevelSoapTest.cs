using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Services;
using MagentoAccessTestsIntegration.TestEnvironment;
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

		[ SetUp ]
		public void Setup()
		{
			this._testData = new TestData( @"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv", @"..\..\Files\magento_VerifierCode.csv" );
			this._soapUserCredentials = this._testData.GetMagentoSoapUser();
			this._authorityUrls = this._testData.GetMagentoUrls();

			this._service = new MagentoServiceLowLevelSoap( this._soapUserCredentials.ApiUser, this._soapUserCredentials.ApiKey, this._authorityUrls.MagentoBaseUrl, null );
		}

		[ Test ]
		public void GetOrders_StoreContainsOrders_ReceiveOrders()
		{
			//------------ Arrange

			//------------ Act
			var modifiedFrom = DateTime.Parse( "2014-05-08 15:02:58" );
			var modifiedTo = DateTime.Parse( "2014-05-28 10:48:52" );
			var getOrdersTask = this._service.GetOrdersAsync( modifiedFrom, modifiedTo );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.Should().NotBeNull();
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
			getOrdersTask.Result.Should().NotBeNull();
		}

		[ Test ]
		public void GetProducts_StoreContainsProducts_ReceiveProducts()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._service.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull();
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
			getProductsTask.Result.Should().NotBeNull();
		}

		[ Test ]
		public void GetSessionId_StoreContainsUser_ReceiveSessionId()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._service.GetSessionId();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull();
		}

		[ Test ]
		public void GetSessionId_StoreDoesNotContainsUser_NoExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act

			Action act = () =>
			{
				this._service.ApiUser = "incorrect password";
				var getProductsTask = this._service.GetSessionId();
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
	}
}