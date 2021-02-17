using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using MagentoAccess.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	[ TestFixture ]
	internal class CatalogStockItemsRepositoryTest : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		[ Ignore( "Because there is multiple update tests" ) ]
		public void PutStockItemAsync_StoreContainsProducts_ProductsUpdated( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
			var productRepository = new ProductRepository( token, testCase.Url );
			var stockRepository = new CatalogStockItemRepository( token, testCase.Url );

			var products = productRepository.GetProductsAsync( DateTime.MinValue, new PagingModel( 5, 1 ) ).WaitResult();
			var stockItem = stockRepository.GetStockItemAsync( products.items.First().sku ).WaitResult();
			//------------ Act
			var stockItem2 = stockRepository.PutStockItemAsync( products.items.First().sku, stockItem.itemId.Value.ToString(), new RootObject { stockItem = new StockItem { qty = 100, isInStock = true } } ).WaitResult();
			var stockItem2Updated = stockRepository.GetStockItemAsync( products.items.First().sku ).WaitResult();

			var stockItem3 = stockRepository.PutStockItemAsync( products.items.First().sku, stockItem.itemId.Value.ToString(), new RootObject { stockItem = new StockItem { qty = 100500, isInStock = false } } ).WaitResult();
			var stockItem3Updated = stockRepository.GetStockItemAsync( products.items.First().sku ).WaitResult();
			//------------ Assert
			stockItem2.Should().Be( true );
			stockItem3.Should().Be( true );
			stockItem2Updated.qty.Value.Should().Be( 100 );
			stockItem2Updated.isInStock.Value.Should().Be( true );
			stockItem3Updated.qty.Value.Should().Be( 100500 );
			stockItem3Updated.isInStock.Value.Should().Be( false );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" )]
		[ Ignore( "Because there is multiple update tests" ) ]
		public void PutStockItemsAsync_StoreContainsProducts_ProductsUpdated( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass ).WaitResult();
			var productRepository = new ProductRepository( token, testCase.Url );
			var stockRepository = new CatalogStockItemRepository( token, testCase.Url );

			var products = productRepository.GetProductsAsync( DateTime.MinValue, "simple", new PagingModel( 5, 1 ) ).WaitResult();
			var stockItems = stockRepository.GetStockItemsAsync( products.items.Select( x => x.sku ) ).WaitResult();
			//------------ Act
			var stockItem2 = stockRepository.PutStockItemsAsync( products.items.Select( x => Tuple.Create( x.sku, x.id.ToString(), new RootObject { stockItem = new StockItem { qty = 100, isInStock = true } } ) ) ).WaitResult();
			var stockItem2Updated = stockRepository.GetStockItemsAsync( products.items.Select( x => x.sku ) ).WaitResult();

			var stockItem3 = stockRepository.PutStockItemsAsync( products.items.Select( x => Tuple.Create( x.sku, x.id.ToString(), new RootObject { stockItem = new StockItem { qty = 100500, isInStock = false } } ) ) ).WaitResult();
			var stockItem3Updated = stockRepository.GetStockItemsAsync( products.items.Select( x => x.sku ) ).WaitResult();
			//------------ Assert
			stockItem2.Should().OnlyContain( x => x );
			stockItem3.Should().OnlyContain( x => x );
			stockItem2Updated.Should().OnlyContain( x => x.qty == 100 );
			stockItem2Updated.Should().OnlyContain( x => x.isInStock == true );
			stockItem3Updated.Should().OnlyContain( x => x.qty == 100500 );
			stockItem3Updated.Should().OnlyContain( x => x.isInStock == false );
			products.items.Should().OnlyContain( x => x.typeId == "simple" );
		}
	}
}