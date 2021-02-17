using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Netco.Logging;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	[ TestFixture ]
	[ Category( "v2LowLevelReadSmoke" ) ]
	internal class ProductRepositoryTest
	{
		private static AuthorizationToken GetToken( RepositoryTestCase testCase )
		{
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			return adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetProducts_StoreContainsProducts_ReceiveProducts( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var token = GetToken( testCase );
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
			var token = GetToken( testCase );
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
			var token = GetToken( testCase );
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

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetProductsByDefaultFilter_StoreContainsProducts_ReceiveProducts( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var token = GetToken( testCase );
			var productRepository = new ProductRepository( token, testCase.Url );

			//------------ Act
			var productPages = productRepository.GetProductsAsync( DateTime.MinValue, null, false ).WaitResult();

			//------------ Assert
			token.Token.Should().NotBeNullOrWhiteSpace();
			productPages.Count.Should().BeGreaterOrEqualTo( 0 );
			productPages.Any( page => page.items.Any( i => i.sku == null ) ).Should().BeFalse();
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetProductsManufacturers( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var token = GetToken( testCase );
			var productRepository = new ProductRepository( token, testCase.Url );

			//------------ Act
			var productsManufacturers = productRepository.GetManufacturersAsync().WaitResult();

			//------------ Assert
			token.Token.Should().NotBeNullOrWhiteSpace();
			productsManufacturers.Should().NotBeNull();
			productsManufacturers.options.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void GetProductsBySkusAsync( RepositoryTestCase testCase )
		{
			var productRepository = new ProductRepository( GetToken( testCase ), testCase.Url );
			var skus = new List< string >{ "testsku1", "testsku2" };
			const int batchesCount = 1;

			var result = productRepository.GetProductsBySkusAsync( skus, Mark.Blank() ).Result.ToList();

			result.Count.Should().Be( batchesCount );
			result.First().items.Count.Should().Be( skus.Count );
			result.First().items.First().sku.Should().Be( skus.First() );
		}
	}
}