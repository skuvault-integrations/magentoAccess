using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x.Repository;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	[ TestFixture ]
	internal class SaleRepositoryTest
	{
		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public async Task GetOrdersAsync_StoreContainsOrders_ReceivPage( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetToken( testCase.MagentoLogin, testCase.MagentoPass );
			tokenTask.Wait();
			var productRepository = new SalesOrderRepositoryV1( tokenTask.Result, testCase.Url );
			//------------ Act

			var products = productRepository.GetOrdersAsync( DateTime.MinValue, DateTime.MaxValue, new PagingModel( 5, 1 ) );
			products.Wait();

			tokenTask.Result.Token.Should().NotBeNullOrWhiteSpace();
			products.Result.items.Count.Should().BeGreaterOrEqualTo( 0 );
			products.Result.items.Count.Should().BeLessOrEqualTo( 5 );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public async Task GetOrdersAsync_StoreContainsOrders_ReceiveOrders( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetToken( testCase.MagentoLogin, testCase.MagentoPass );
			tokenTask.Wait();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask.Result, testCase.Url );
			//------------ Act

			var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.MaxValue );
			items.Wait();

			tokenTask.Result.Token.Should().NotBeNullOrWhiteSpace();
			items.Result.SelectMany( x => x.items ).Count().Should().Be( items.Result.First().total_count );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public async Task GetOrdersAsync_StoreContainsOrders_ReceiveOrdersByIdReceivePage( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetToken( testCase.MagentoLogin, testCase.MagentoPass );
			tokenTask.Wait();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask.Result, testCase.Url );
			//------------ Act

			var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.MaxValue, new PagingModel( 5, 1 ) );
			items.Wait();

			var items2 = salesOrderRepositoryV1.GetOrdersAsync( items.Result.items.Select( x => x.increment_id ), new PagingModel( 5, 1 ) );
			items2.Wait();

			tokenTask.Result.Token.Should().NotBeNullOrWhiteSpace();
			items2.Result.items.Count.Should().BeGreaterOrEqualTo( 0 );
			items2.Result.items.Count.Should().Be( items.Result.items.Count );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public async Task GetOrdersAsync_StoreContainsOrders_ReceiveOrdersByIdReceive( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetToken( testCase.MagentoLogin, testCase.MagentoPass );
			tokenTask.Wait();
			var salesOrderRepositoryV1 = new SalesOrderRepositoryV1( tokenTask.Result, testCase.Url );
			//------------ Act

			var items = salesOrderRepositoryV1.GetOrdersAsync( DateTime.MinValue, DateTime.UtcNow );
			items.Wait();

			var items2 = salesOrderRepositoryV1.GetOrdersAsync( items.Result.SelectMany( y => y.items ).Select( x => x.increment_id ) );
			items2.Wait();

			tokenTask.Result.Token.Should().NotBeNullOrWhiteSpace();
			items2.Result.SelectMany( y => y.items ).Count().Should().BeGreaterOrEqualTo( 0 );
			items2.Result.SelectMany( y => y.items ).Count().Should().Be( items.Result.SelectMany( y => y.items ).Count() );
		}
	}
}