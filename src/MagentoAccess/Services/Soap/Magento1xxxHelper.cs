using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using Netco.Extensions;
using Category = MagentoAccess.Models.Services.Soap.GetProducts.Category;
using MagentoUrl = MagentoAccess.Models.Services.Soap.GetProducts.MagentoUrl;

namespace MagentoAccess.Services.Soap
{
	internal class Magento1xxxHelper: IMagento1XxxHelper
	{
		private readonly IMagentoServiceLowLevelSoap _magentoServiceLowLevelSoap;

		public Magento1xxxHelper( IMagentoServiceLowLevelSoap magentoServiceLowLevelSoap )
		{
			this._magentoServiceLowLevelSoap = magentoServiceLowLevelSoap;
		}

		public async Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts )
		{
			var productAttributes = this._magentoServiceLowLevelSoap.GetManufacturersInfoAsync( ProductAttributeCodes.Manufacturer );
			var resultProductslist = resultProducts as IList< ProductDetails > ?? resultProducts.ToList();
			var attributes = new string[] { ProductAttributeCodes.Cost, ProductAttributeCodes.Manufacturer, ProductAttributeCodes.Upc };

			var productsInfoTask = resultProductslist.ProcessInBatchAsync( 10, async x => await this._magentoServiceLowLevelSoap.GetProductInfoAsync( new CatalogProductInfoRequest( attributes, x.Sku, x.ProductId ) ).ConfigureAwait( false ) );
			var mediaListResponsesTask = resultProductslist.ProcessInBatchAsync( 10, async x => await this._magentoServiceLowLevelSoap.GetProductAttributeMediaListAsync( new GetProductAttributeMediaListRequest( x.ProductId, x.Sku ) ).ConfigureAwait( false ) );
			var categoriesTreeResponseTask = this._magentoServiceLowLevelSoap.GetCategoriesTreeAsync();
			await Task.WhenAll( productAttributes, productsInfoTask, mediaListResponsesTask, categoriesTreeResponseTask ).ConfigureAwait( false );

			var productsInfo = productsInfoTask.Result;
			var mediaListResponses = mediaListResponsesTask.Result;
			var magentoCategoriesList = categoriesTreeResponseTask.Result.RootCategory == null ? new List< CategoryNode >() : categoriesTreeResponseTask.Result.RootCategory.Flatten();

			Func< IEnumerable< ProductDetails >, IEnumerable< ProductAttributeMediaListResponse >, IEnumerable< ProductDetails > > FillImageUrls = ( prods, mediaLists ) =>
				( from rp in prods
					join pi in mediaLists on rp.ProductId equals pi.ProductId into pairs
					from pair in pairs.DefaultIfEmpty()
					select pair == null ? rp : new ProductDetails( rp, pair.MagentoImages.Select( x => new MagentoUrl( x ) ) ) );

			Func< IEnumerable< ProductDetails >, IEnumerable< CatalogProductInfoResponse >, IEnumerable< ProductDetails > > FillWeightDescriptionShortDescriptionPricev =
				( prods, prodInfos ) => ( from rp in prods
					join pi in prodInfos on rp.ProductId equals pi.ProductId into pairs
					from pair in pairs.DefaultIfEmpty()
					select pair == null ? rp : new ProductDetails( rp, upc : pair.GetUpcAttributeValue(), manufacturer : pair.GetManufacturerAttributeValue(), cost : pair.GetCostAttributeValue().ToDecimalOrDefault(), weight : pair.Weight, shortDescription : pair.ShortDescription, description : pair.Description, specialPrice : pair.SpecialPrice, price : pair.Price, categories : pair.CategoryIds.Select( z => new Category( z ) ) ) );

			Func< IEnumerable< ProductDetails >, CatalogProductAttributeInfoResponse, IEnumerable< ProductDetails > > FillManufactures =
				( prods, prodInfos ) => ( from rp in prods
					join pi in prodInfos != null ? prodInfos.Attributes : new List< ProductAttributeInfo >() on rp.Manufacturer equals pi.Value into pairs
					from pair in pairs.DefaultIfEmpty()
					select pair == null ? rp : new ProductDetails( rp, manufacturer : pair.Label ) );

			Func< IEnumerable< ProductDetails >, IEnumerable< Category >, IEnumerable< ProductDetails > > FillProductsDeepestCategory =
				( prods, categories ) => ( from prod in prods
					let prodCategories = ( from category in ( prod.Categories ?? Enumerable.Empty< Category >() )
						join category2 in categories on category.Id equals category2.Id
						select category2 )
					select new ProductDetails( prod, categories : prodCategories ) );

			resultProducts = FillWeightDescriptionShortDescriptionPricev( resultProductslist, productsInfo ).ToList();
			resultProducts = FillImageUrls( resultProducts, mediaListResponses ).ToList();
			resultProducts = FillManufactures( resultProducts, productAttributes.Result ).ToList();
			resultProducts = FillProductsDeepestCategory( resultProducts, magentoCategoriesList.Select( y => new Category( y ) ).ToList() ).ToList();
			return resultProducts;
		}
	}
}