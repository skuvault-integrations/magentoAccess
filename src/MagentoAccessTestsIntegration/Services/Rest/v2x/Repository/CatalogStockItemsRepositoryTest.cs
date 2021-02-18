using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using MagentoAccess.Services.Rest.v2x;
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccessTestsIntegration.TestEnvironment;
using Netco.Extensions;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	[ TestFixture ]
	internal class CatalogStockItemsRepositoryTest : BaseTest
	{
		private MagentoTimeouts _defaultOperationsTimeouts = new MagentoTimeouts();

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		[ Ignore( "Because there is multiple update tests" ) ]
		public void PutStockItemAsync_StoreContainsProducts_ProductsUpdated( RepositoryTestCase testCase )
		{
			//------------ Arrange
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url, _defaultOperationsTimeouts );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass, CancellationToken.None ).WaitResult();
			var productRepository = new ProductRepository( token, testCase.Url, _defaultOperationsTimeouts );
			var stockRepository = new CatalogStockItemRepository( token, testCase.Url, _defaultOperationsTimeouts );

			var products = productRepository.GetProductsAsync( DateTime.MinValue, new PagingModel( 5, 1 ), CancellationToken.None ).WaitResult();
			var stockItem = stockRepository.GetStockItemAsync( products.items.First().sku, CancellationToken.None ).WaitResult();
			//------------ Act
			var stockItem2 = stockRepository.PutStockItemAsync( products.items.First().sku, stockItem.itemId.Value.ToString(), new RootObject { stockItem = new StockItem { qty = 100, isInStock = true } }, CancellationToken.None ).WaitResult();
			var stockItem2Updated = stockRepository.GetStockItemAsync( products.items.First().sku, CancellationToken.None ).WaitResult();

			var stockItem3 = stockRepository.PutStockItemAsync( products.items.First().sku, stockItem.itemId.Value.ToString(), new RootObject { stockItem = new StockItem { qty = 100500, isInStock = false } }, CancellationToken.None ).WaitResult();
			var stockItem3Updated = stockRepository.GetStockItemAsync( products.items.First().sku, CancellationToken.None ).WaitResult();
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
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url, _defaultOperationsTimeouts );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass, CancellationToken.None ).WaitResult();
			var productRepository = new ProductRepository( token, testCase.Url, _defaultOperationsTimeouts );
			var stockRepository = new CatalogStockItemRepository( token, testCase.Url, _defaultOperationsTimeouts );

			var products = productRepository.GetProductsAsync( DateTime.MinValue, "simple", new PagingModel( 5, 1 ), CancellationToken.None ).WaitResult();
			var stockItems = stockRepository.GetStockItemsAsync( products.items.Select( x => x.sku ), CancellationToken.None ).WaitResult();
			//------------ Act
			var stockItem2 = stockRepository.PutStockItemsAsync( products.items.Select( x => Tuple.Create( x.sku, x.id.ToString(), new RootObject { stockItem = new StockItem { qty = 100, isInStock = true } } ) ), CancellationToken.None ).WaitResult();
			var stockItem2Updated = stockRepository.GetStockItemsAsync( products.items.Select( x => x.sku ), CancellationToken.None ).WaitResult();

			var stockItem3 = stockRepository.PutStockItemsAsync( products.items.Select( x => Tuple.Create( x.sku, x.id.ToString(), new RootObject { stockItem = new StockItem { qty = 100500, isInStock = false } } ) ), CancellationToken.None ).WaitResult();
			var stockItem3Updated = stockRepository.GetStockItemsAsync( products.items.Select( x => x.sku ), CancellationToken.None ).WaitResult();
			//------------ Assert
			stockItem2.Should().OnlyContain( x => x );
			stockItem3.Should().OnlyContain( x => x );
			stockItem2Updated.Should().OnlyContain( x => x.qty == 100 );
			stockItem2Updated.Should().OnlyContain( x => x.isInStock == true );
			stockItem3Updated.Should().OnlyContain( x => x.qty == 100500 );
			stockItem3Updated.Should().OnlyContain( x => x.isInStock == false );
			products.items.Should().OnlyContain( x => x.typeId == "simple" );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		public void WhenPutStockItemAsyncIsCalled_ThenStockItemUpdatedQuantityIsExpected( RepositoryTestCase testCase )
		{
			var testSku = "G-788-11504";
			var newTestSkuQuantity = new Random().Next( 1, 100 );
			
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url, _defaultOperationsTimeouts );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass, CancellationToken.None ).WaitResult();
			var productRepository = new ProductRepository( token, testCase.Url, _defaultOperationsTimeouts );
			var stockRepository = new CatalogStockItemRepository( token, testCase.Url, _defaultOperationsTimeouts );

			var testItem = productRepository.GetProductAsync( testSku, CancellationToken.None ).WaitResult();
			testItem.Should().NotBeNull();

			var testItemStock = stockRepository.GetStockItemAsync( testItem.sku, CancellationToken.None ).WaitResult();

			var isUpdateSuccessfull = stockRepository.PutStockItemAsync( testItem.sku, testItemStock.itemId.Value.ToString(), new RootObject { stockItem = new StockItem { qty = newTestSkuQuantity, isInStock = true } }, CancellationToken.None ).WaitResult();
			var updatedTestItemStock = stockRepository.GetStockItemAsync( testItem.sku, CancellationToken.None ).WaitResult();

			isUpdateSuccessfull.Should().Be( true );
			updatedTestItemStock.qty.Value.Should().Be( newTestSkuQuantity );
		}

		[ Test ]
		[ TestCaseSource( typeof( RepositoryTestCases ), "TestCases" ) ]
		[ Ignore( "Because there are tons of update tests" ) ]
		public async Task GivenHugeAmountOfSkusToUpdate_WhenPutStockItemAsyncIsCalled_ThenStockItemUpdatedQuantityIsExpected( RepositoryTestCase testCase )
		{
			var adminRepository = new IntegrationAdminTokenRepository( testCase.Url, _defaultOperationsTimeouts );
			var token = adminRepository.GetTokenAsync( testCase.MagentoLogin, testCase.MagentoPass, CancellationToken.None ).WaitResult();
			var productRepository = new ProductRepository( token, testCase.Url, _defaultOperationsTimeouts );
			var stockRepository = new CatalogStockItemRepository( token, testCase.Url, _defaultOperationsTimeouts );

			var rand = new Random();
			var skus = new List< string >();
			int maxSkusAmount = 500;
			var skuPrefix = "G-788-";
			for( int i = 1; i <= maxSkusAmount; i++ )
			{
				skus.Add( skuPrefix + i.ToString() );
			}

			var results = await skus.ProcessInBatchAsync( 10, async testSku =>
			{
				var testItem = await productRepository.GetProductAsync( testSku, CancellationToken.None );
				if ( testItem == null )
					return false;

				var testItemStock = await stockRepository.GetStockItemAsync( testItem.sku, CancellationToken.None );
				var newQuantity = rand.Next( 1, 100 );
				var isUpdateSuccessfull = await stockRepository.PutStockItemAsync( testItem.sku, testItemStock.itemId.Value.ToString(), new RootObject { stockItem = new StockItem { qty = newQuantity, isInStock = true } }, CancellationToken.None );

				return isUpdateSuccessfull;
			} );

			results.Should().NotContain( false );
		}
	}
}