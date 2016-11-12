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
		public async Task GetOrdersAsync_StoreContainsOrders_ReceiveOrders( RepositoryTestCase testCase )
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
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public async Task GetOrdersAsync_StoreContainsOrders_ReceiveOrdersById( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetToken( testCase.MagentoLogin, testCase.MagentoPass );
			tokenTask.Wait();
			var productRepository = new SalesOrderRepositoryV1( tokenTask.Result, testCase.Url );
			//------------ Act

			var products = productRepository.GetOrdersAsync( DateTime.MinValue, DateTime.MaxValue, new PagingModel( 5, 1 ) );
			products.Wait();

			var products2 = productRepository.GetOrdersAsync( products.Result.items.Select( x => x.increment_id ), new PagingModel( 5, 1 ) );
			products2.Wait();

			tokenTask.Result.Token.Should().NotBeNullOrWhiteSpace();
			products2.Result.items.Count.Should().BeGreaterOrEqualTo( 0 );
			products2.Result.items.Count.Should().Be( products.Result.items.Count );
		}
	}
}