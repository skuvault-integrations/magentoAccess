using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccessTestsIntegration.Services.Rest.v2x.Repository;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services
{
	[ TestFixture ]
	internal class ProductRepositoryTest
	{
		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public async Task GetProducts_StoreContainsProducts_ReceiveProducts( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository(testCase.Url);
			var tokenTask = adminRepository.GetToken( testCase.MagentoLogin, testCase.MagentoPass );
			tokenTask.Wait();
			var productRepository = new ProductRepository(tokenTask.Result, testCase.Url );
			//------------ Act

			var products = productRepository.GetProductsAsync();
			products.Wait();

			tokenTask.Result.Token.Should().NotBeNullOrWhiteSpace();
			products.Result.Count.Should().BeGreaterOrEqualTo( 0 );
			products.Result.SelectMany( x => x.items ).Count().Should().BeGreaterThan( 0 );
		}

		[Test]
		[TestCaseSource(typeof(RepositoryTestCases), "TestCases")]
		public async Task GetProductsByUpdatedAt_StoreContainsProducts_ReceiveProducts(RepositoryTestCase testCase)
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository(testCase.Url);
			var tokenTask = adminRepository.GetToken(testCase.MagentoLogin, testCase.MagentoPass);
			tokenTask.Wait();
			var productRepository = new ProductRepository(tokenTask.Result, testCase.Url);
			var products = productRepository.GetProductsAsync();
			products.Wait();
			var allproductsSortedByDate = products.Result.SelectMany(x => x.items).OrderBy(y => y.updatedAt);
			var date = allproductsSortedByDate.Skip(allproductsSortedByDate.Count() / 2).First().updatedAt;
			//------------ Act

			var productsUpdatedAt = productRepository.GetProductsAsync(date.ToDateTimeOrDefault());
			productsUpdatedAt.Wait();

			tokenTask.Result.Token.Should().NotBeNullOrWhiteSpace();
			products.Result.Count.Should().BeGreaterOrEqualTo(0);
			products.Result.SelectMany(x => x.items).Count().Should().BeGreaterThan(0);
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public async Task GetProductsByType_StoreContainsProducts_ReceiveProducts( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetToken( testCase.MagentoLogin, testCase.MagentoPass );
			tokenTask.Wait();
			var productRepository = new ProductRepository( tokenTask.Result, testCase.Url );
			//------------ Act

			var productsUpdatedAt = productRepository.GetProductsAsync( DateTime.MinValue, "bundle" );
			productsUpdatedAt.Wait();
			var productsUpdatedAt2 = productRepository.GetProductsAsync( DateTime.MinValue, "simple" );
			productsUpdatedAt2.Wait();

			tokenTask.Result.Token.Should().NotBeNullOrWhiteSpace();
			productsUpdatedAt.Result.SelectMany( x => x.items ).Count().Should().BeGreaterThan( 0 );
			productsUpdatedAt2.Result.SelectMany( x => x.items ).Count().Should().BeGreaterThan( 0 );
		}
	}
}