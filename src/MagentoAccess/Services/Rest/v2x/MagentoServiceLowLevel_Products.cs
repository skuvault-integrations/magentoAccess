using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetSessionId;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccess.Services.Soap;
using Netco.ActionPolicyServices;
using Netco.Extensions;
using MagentoUrl = MagentoAccess.Services.Rest.v2x.WebRequester.MagentoUrl;

namespace MagentoAccess.Services.Rest.v2x
{
	internal partial class MagentoServiceLowLevel : IMagentoServiceLowLevelSoap
	{
		public async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				var products = await this.ProductRepository.GetProductsAsync( updatedFrom ?? DateTime.MinValue, productType, productTypeShouldBeExcluded ).ConfigureAwait( false );
				return new SoapGetProductsResponse( products.SelectMany( x => x.items ).ToList() );
			} );
		}

		public Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType, Mark markForLog = null )
		{
			return null;
		}

		public Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType )
		{
			return null;
		}

		public Task< CatalogProductInfoResponse > GetProductInfoAsync( CatalogProductInfoRequest catalogProductInfoRequest, bool throwException = true )
		{
			return null;
		}

		public async Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts )
		{
			throw new NotImplementedException();//not need for this version
			//#if DEBUG
			//			var batchSize = 30;
			//#else
			//			var batchSize = 10;
			//#endif

			//			var productsDetailedInfo = await this.RepeatOnAuthProblemAsync.Get(async () =>
			//			{
			//				var products = await resultProducts.ProcessInBatchAsync(batchSize, async x => await this.ProductRepository.GetProductAsync(x.Sku).ConfigureAwait(false)).ConfigureAwait(false);
			//				return products.ToList();
			//			});

			//			var productsDetailedInfoCast = productsDetailedInfo.Select(Mapper.Map<ProductDetails>).ToList();
			

		}

		public Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, IReadOnlyCollection< string > skus )
		{
			throw new NotImplementedException();
		}

		public Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, bool throwException = true )
		{
			return null;
		}
	}
}