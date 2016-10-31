using System.Collections.Generic;
using FluentAssertions;
using MagentoAccess.Models.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services
{
	[ TestFixture ]
	internal class MeagentoServiceLowLevelRestTest2 : BaseTest
	{
		[ Test ]
		public void GetOrders_StoreContainsOrders_ReceiveOrders2()
		{
			//------------ Arrange

			//------------ Act
			var sc = new SearchCriteria( new List< SearchCriteria.FilterGroup >()
			{
				new SearchCriteria.FilterGroup( new List< SearchCriteria.FilterGroup.Filter >()
				{
					new SearchCriteria.FilterGroup.Filter( @"updated_at", @"2016-07-01 00:00:00", SearchCriteria.FilterGroup.Filter.ConditionType.GreaterThan ),
				} )
			} )
			{ CurrentPage = 1, PageSize = 100 };

			//------------ Assert
			var qwe = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Products )
				.Parameters( sc )
				.Url( MagentoUrl.Create("http://xxx") )
				;
			var res = qwe.RunAsync();
			res.Wait();


		}

		[Test]
		public void GetOrders_StoreContainsOrders_ReceiveOrders3()
		{
			//------------ Arrange

			//------------ Act

			var er = new MagentoServiceLowLevel();
			var res =er.GetProductsAsync();
			res.Wait();
			res.Result.items.Count.Should().BeGreaterThan(0);
		}
	}
}