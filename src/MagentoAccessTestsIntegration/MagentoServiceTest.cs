using System;
using FluentAssertions;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	public class MagentoServiceTest : BaseTest
	{
		[ Test ]
		public void GetOrders_UserAlreadyHasAccessTokens_GetsOrders()
		{
			//------------ Arrange

			//------------ Act
			var getOrdersTask = this._service.GetOrdersAsync( DateTime.Now.AddMonths( -3 ), DateTime.Now );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.Should().NotBeNull().And.NotBeEmpty();
		}

		[ Test ]
		public void GetProducts_UserAlreadyHasAccessTokens_GetsProducts()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._service.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull().And.NotBeEmpty();
		}
	}
}