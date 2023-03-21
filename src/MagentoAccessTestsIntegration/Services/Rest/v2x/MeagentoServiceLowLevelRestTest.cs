﻿using System.Collections.Generic;
using System.Threading;
using MagentoAccess.Models.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x
{
	[ TestFixture ]
	[ Ignore( "Not implemented" ) ]
	internal class MeagentoServiceLowLevelRestTest2 : BaseTest
	{
		[ Test ]
		public void GetOrders_StoreContainsOrders_ReceiveOrders2()
		{
			//------------ Arrange

			//------------ Act
			var sc = new SearchCriteria()
			{
				filter_groups = new List< FilterGroup >()
				{
					new FilterGroup()
					{
						filters = new List< Filter >
						{
							new Filter( @"updated_at", @"2016-07-01 00:00:00", Filter.ConditionType.GreaterThan ),
						}
					}
				},
				page_size = 100, current_page = 1
			};

			//------------ Assert
			var qwe = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.CreateProductsServicePath() )
				.Parameters( sc )
				.Url( MagentoUrl.Create( "http://xxx", "" ) )
				;
			var res = qwe.RunAsync( CancellationToken.None );
			res.Wait();
		}

		[ Test ]
		public void GetOrders_StoreContainsOrders_ReceiveOrders3()
		{
			//------------ Arrange

			//------------ Act

			//var er = new MagentoServiceLowLevel();
			//var res =er.GetProductsAsync();
			//res.Wait();
			//res.Result.items.Count.Should().BeGreaterThan(0);
		}
	}
}