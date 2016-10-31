using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.Services.Rest.v2x.Repository
{
	[ TestFixture ]
	internal class ProductsRepositoryTest : BaseTest
	{
		[ Test ]
		public async Task GetProductsAsync_StoreContainsProducts_ReceiveProducts()
		{
			//------------ Arrange
			var productRepository = new ProductRepository( AuthorizationToken.Create( "crh23f1vcdp99v9iej06h7khnwaepb94" ), MagentoUrl.Create( "http://xxxxx/" ) );

			//------------ Act
			var res = await productRepository.GetProductsAsync();
			//------------ Assert
			res.Count.Should().BeGreaterThan( 0 );
			res.SelectMany( x => x.items ).Count().Should().BeGreaterThan( 0 );
		}
	}
}