using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
		public async Task PutStockItemAsync_StoreContainsProducts_ProductsUpdated( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url );
			var tokenTask = adminRepository.GetToken( testCase.MagentoLogin, testCase.MagentoPass );
			tokenTask.Wait();
			var productRepository = new ProductRepository( tokenTask.Result, testCase.Url );
			var products = productRepository.GetProductsAsync( DateTime.MinValue, new PagingModel( 5, 1 ) );
			products.Wait();

			var stockRepository = new CatalogStockItemRepository( tokenTask.Result, testCase.Url );
			var stockItem = stockRepository.GetStockItemAsync( products.Result.items.First().sku );
			stockItem.Wait();
			//------------ Act
			var stockItem2 = stockRepository.PutStockItemAsync( products.Result.items.First().sku, stockItem.Result.itemId.Value.ToString(), new RootObject { stockItem = new StockItem { qty = 100, isInStock = true } } );
			stockItem2.Wait();
			var stockItem2Updated = stockRepository.GetStockItemAsync( products.Result.items.First().sku );
			stockItem2Updated.Wait();
			var stockItem3 = stockRepository.PutStockItemAsync( products.Result.items.First().sku, stockItem.Result.itemId.Value.ToString(), new RootObject { stockItem = new StockItem { qty = 100500, isInStock = false } } );
			stockItem3.Wait();
			var stockItem3Updated = stockRepository.GetStockItemAsync( products.Result.items.First().sku );
			stockItem3Updated.Wait();
			//------------ Assert
			stockItem2.Result.Should().Be(true);
			stockItem3.Result.Should().Be(true);
			stockItem2Updated.Result.qty.Value.Should().Be( 100 );
			stockItem2Updated.Result.isInStock.Value.Should().Be( true );
			stockItem3Updated.Result.qty.Value.Should().Be( 100500 );
			stockItem3Updated.Result.isInStock.Value.Should().Be( false );
		}
	}
}