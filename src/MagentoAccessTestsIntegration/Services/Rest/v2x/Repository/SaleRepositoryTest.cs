using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccess.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x.Repository;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	[ Explicit ]
	[ TestFixture ]
	[ Category( "v2LowLevelReadSmoke" ) ]
	internal class SaleRepositoryTest
	{
		private MagentoTimeouts _defaultOperationsTimeouts = new MagentoTimeouts();

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetOrdersAsync_StoreContainsOrders_ReceivPage( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url, _defaultOperationsTimeouts );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass, CancellationToken.None ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( token, testCase.Url, _defaultOperationsTimeouts );
			var itemsPerPage = 5;

			//------------ Act
			var orders = salesOrderRepositoryV1.GetOrdersAsync( new DateTime( 2012, 1, 1 ), DateTime.UtcNow.AddDays( 1 ), new PagingModel( itemsPerPage, 1 ), CancellationToken.None ).WaitResult();

			//------------ Assert
			token.Token.Should().NotBeNullOrWhiteSpace();
			orders.items.Count.Should().BeGreaterThanOrEqualTo( 1 );
			orders.items.Count.Should().BeLessThanOrEqualTo( itemsPerPage );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetOrdersAsync_StoreContainsOrders_ReceiveOrders( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url, _defaultOperationsTimeouts );
			var tokenTask = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass, CancellationToken.None ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask, testCase.Url, _defaultOperationsTimeouts );

			//------------ Act
			var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.UtcNow, CancellationToken.None ).WaitResult();

			//------------ Assert
			tokenTask.Token.Should().NotBeNullOrWhiteSpace();
			items.SelectMany( x => x.items ).Count().Should().Be( items.First().total_count );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetOrdersAsync_StoreContainsOrders_ReceiveOrdersByIdReceivePage( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url, _defaultOperationsTimeouts );
			var tokenTask = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass, CancellationToken.None ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask, testCase.Url, _defaultOperationsTimeouts );
			var itemsPerPage = 5;

			//------------ Act
			var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.UtcNow, new PagingModel( itemsPerPage, 1 ), CancellationToken.None ).WaitResult();
			var items2 = salesOrderRepositoryV1.GetOrdersAsync( items.items.Select( x => x.increment_id ), new PagingModel( itemsPerPage, 1 ), CancellationToken.None ).WaitResult();

			//------------ Assert
			tokenTask.Token.Should().NotBeNullOrWhiteSpace();
			items2.items.Count.Should().BeGreaterThanOrEqualTo( 1 );
			items2.items.Count.Should().Be( items.items.Count );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetOrdersAsync_StoreContainsOrders_ReceiveOrdersByIdReceive( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url, _defaultOperationsTimeouts );
			var tokenTask = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass, CancellationToken.None ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask, testCase.Url, _defaultOperationsTimeouts );

			//------------ Act
			var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.UtcNow, CancellationToken.None ).WaitResult();
			var items2 = salesOrderRepositoryV1.GetOrdersAsync( items.SelectMany( y => y.items ).Select( x => x.increment_id ), CancellationToken.None ).WaitResult();

			//------------ Assert
			tokenTask.Token.Should().NotBeNullOrWhiteSpace();
			items2.SelectMany( y => y.items ).Count().Should().BeGreaterThanOrEqualTo( 1 );
			items2.SelectMany( y => y.items ).Count().Should().Be( items.SelectMany( y => y.items ).Count() );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetOrdersShipmentsAsync_StoreContainsShipments_ReceiveModifiedShipments( RepositoryTestCase credentials )
		{
			var adminRepository = new IntegrationAdminTokenRepository( credentials.Url, new MagentoTimeouts() );
			var token = adminRepository.GetTokenAsync( credentials.MagentoLogin, credentials.MagentoPass, CancellationToken.None ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( token, credentials.Url, new MagentoTimeouts() );

			var page = new PagingModel( 100, 1 );
			var shipments = salesOrderRepositoryV1.GetOrdersShipmentsAsync( DateTime.MinValue, DateTime.UtcNow, page, CancellationToken.None ).WaitResult();

			shipments.Items.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GivenTooSmallTimeout_WhenGetOrdersAsyncIsCalled_ThenTimeoutExceptionIsExcepted( RepositoryTestCase testCase )
		{
			var specificTimeouts = new MagentoTimeouts();
			specificTimeouts.Set( MagentoOperationEnum.GetModifiedOrders, new MagentoOperationTimeout( 10 ) );

			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url, specificTimeouts );
			var tokenTask = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass, CancellationToken.None ).WaitResult();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask, testCase.Url, specificTimeouts );

			var ex = Assert.Throws< AggregateException >( () =>
			{
				var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.UtcNow, CancellationToken.None ).WaitResult();
				var items2 = salesOrderRepositoryV1.GetOrdersAsync( items.SelectMany( y => y.items ).Select( x => x.increment_id ), CancellationToken.None ).WaitResult();
			} );

			ex.Should().NotBeNull();
			ex.InnerException.Should().NotBeNull();
			ex.InnerException.GetType().Should().Be( typeof( TaskCanceledException ) );
		}
	}
}