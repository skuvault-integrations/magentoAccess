using System;
using System.Linq;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccess.Services.Rest.v2x.Repository;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	[ TestFixture ]
	[ Category( "v2LowLevelReadSmoke" ) ]
	internal class ProductRepositoryTest
	{
		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetProducts_StoreContainsProducts_ReceiveProducts( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
			var productRepository = new ProductRepository( token, testCase.Url );

			//------------ Act
			var products = productRepository.GetProductsAsync().WaitResult();

			//------------ Assert
			token.Token.Should().NotBeNullOrWhiteSpace();
			products.Count.Should().BeGreaterOrEqualTo( 0 );
			products.SelectMany( x => x.items ).Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetProductsByUpdatedAt_StoreContainsProducts_ReceiveProducts( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
			var productRepository = new ProductRepository( token, testCase.Url );
			var products = productRepository.GetProductsAsync().WaitResult();
			var allproductsSortedByDate = products.SelectMany( x => x.items ).OrderBy( y => y.updatedAt );
			var date = allproductsSortedByDate.Skip( allproductsSortedByDate.Count() / 2 ).First().updatedAt;

			//------------ Act
			var productsUpdatedAt = productRepository.GetProductsAsync( date.ToDateTimeOrDefault() ).WaitResult();

			//------------ Assert
			token.Token.Should().NotBeNullOrWhiteSpace();
			products.Count.Should().BeGreaterOrEqualTo( 0 );
			products.Count.Should().BeGreaterOrEqualTo( productsUpdatedAt.Count );
			products.SelectMany( x => x.items ).Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetProductsByType_StoreContainsProducts_ReceiveProducts( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
			var productRepository = new ProductRepository( token, testCase.Url );

			//------------ Act
			var simpleProducts = productRepository.GetProductsAsync( DateTime.MinValue, "simple" ).WaitResult();
			var bundleProducts = productRepository.GetProductsAsync( DateTime.MinValue, "bundle" ).WaitResult();

			//------------ Assert
			token.Token.Should().NotBeNullOrWhiteSpace();
			simpleProducts.SelectMany( x => x.items ).Count().Should().BeGreaterThan( 0 );
			bundleProducts.SelectMany( x => x.items ).Count().Should().BeGreaterThan( 0 );
			simpleProducts.SelectMany( x => x.items ).Should().OnlyContain( x => x.typeId == "simple" );
			bundleProducts.SelectMany( x => x.items ).Should().OnlyContain( x => x.typeId == "bundle" );
		}
	}
}