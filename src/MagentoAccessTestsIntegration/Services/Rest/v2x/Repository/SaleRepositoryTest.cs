using System;
using System.Linq;
using FluentAssertions;
using MagentoAccess.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x.Repository;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	[ TestFixture ]
	[ Category( "v2LowLevelReadSmoke" ) ]
	internal class SaleRepositoryTest
	{
		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetOrdersAsync_StoreContainsOrders_ReceivPage( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( token, testCase.Url );
			var itemsPerPage = 5;

			//------------ Act
			var orders = salesOrderRepositoryV1.GetOrdersAsync( new DateTime( 2012, 1, 1 ), DateTime.UtcNow.AddDays( 1 ), new PagingModel( itemsPerPage, 1 ) ).WaitResult();

			//------------ Assert
			token.Token.Should().NotBeNullOrWhiteSpace();
			orders.items.Count.Should().BeGreaterOrEqualTo( 1 );
			orders.items.Count.Should().BeLessOrEqualTo( itemsPerPage );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetOrdersAsync_StoreContainsOrders_ReceiveOrders( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask, testCase.Url );

			//------------ Act
			var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.UtcNow ).WaitResult();

			//------------ Assert
			tokenTask.Token.Should().NotBeNullOrWhiteSpace();
			items.SelectMany( x => x.items ).Count().Should().Be( items.First().total_count );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetOrdersAsync_StoreContainsOrders_ReceiveOrdersByIdReceivePage( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask, testCase.Url );
			var itemsPerPage = 5;

			//------------ Act
			var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.UtcNow, new PagingModel( itemsPerPage, 1 ) ).WaitResult();
			var items2 = salesOrderRepositoryV1.GetOrdersAsync( items.items.Select( x => x.increment_id ), new PagingModel( itemsPerPage, 1 ) ).WaitResult();

			//------------ Assert
			tokenTask.Token.Should().NotBeNullOrWhiteSpace();
			items2.items.Count.Should().BeGreaterOrEqualTo( 1 );
			items2.items.Count.Should().Be( items.items.Count );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetOrdersAsync_StoreContainsOrders_ReceiveOrdersByIdReceive( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask, testCase.Url );

			//------------ Act
			var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.UtcNow ).WaitResult();
			var items2 = salesOrderRepositoryV1.GetOrdersAsync( items.SelectMany( y => y.items ).Select( x => x.increment_id ) ).WaitResult();

			//------------ Assert
			tokenTask.Token.Should().NotBeNullOrWhiteSpace();
			items2.SelectMany( y => y.items ).Count().Should().BeGreaterOrEqualTo( 1 );
			items2.SelectMany( y => y.items ).Count().Should().Be( items.SelectMany( y => y.items ).Count() );
		}
	}
}