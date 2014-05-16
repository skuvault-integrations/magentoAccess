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
			var res = this._service.GetOrders( DateTime.Now.AddMonths( -3 ), DateTime.Now );

			//------------ Assert
			res.Should().NotBeNull().And.NotBeEmpty();
		}

		[ Test ]
		public void GetProducts_UserAlreadyHasAccessTokens_GetsProducts()
		{
			//------------ Arrange

			//------------ Act
			var res = this._service.GetProducts();

			//------------ Assert
			res.Should().NotBeNull().And.NotBeEmpty();
		}
	}
}