using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MagentoAccess.Models.Services.PutStockItems;
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

		[SetUp]
		public void Setup()
		{
			//this._testData = new TestData(@"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv", @"..\..\Files\magento_VerifierCode.csv");
			//this._authorityUrls = this._testData.GetMagentoUrls();
			this._service = new MagentoServiceLowLevelSoap("MaxKits", "123456", "http://192.168.0.103/magento/index.php/api/v2_soap/index/");
		}

		[ Test ]
		public void GetOrders_StoreContainsOrders_ReceiveOrders()
		{
			//------------ Arrange

			//------------ Act
			var modifiedFrom = DateTime.Parse("2014-05-08 15:02:58");
			var modifiedTo = DateTime.Parse("2014-05-28 10:48:52");
			var getOrdersTask = this._service.GetOrders(modifiedFrom, modifiedTo);

			//------------ Assert
			getOrdersTask.Result.Should().NotBeNull();
		}
	}
}