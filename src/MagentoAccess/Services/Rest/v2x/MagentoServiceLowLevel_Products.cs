using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Rest.v2x.Products;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Services.Soap;
using Netco.Extensions;

namespace MagentoAccess.Services.Rest.v2x
{
	internal partial class MagentoServiceLowLevel : IMagentoServiceLowLevelSoap
	{
		private const string ImagePath = "/pub/media/catalog/product";
		
		public async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, Mark mark = null )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				var products = await this.ProductRepository.GetProductsAsync( updatedFrom ?? DateTime.MinValue, productType, productTypeShouldBeExcluded, mark ).ConfigureAwait( false );
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
			throw new NotImplementedException(); //not need for this version
		}

		public Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, IReadOnlyCollection< string > skus )
		{
			throw new NotImplementedException();
		}

		public Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, bool throwException = true )
		{
			return null;
		}

		public async Task< IEnumerable< Product > > GetProductsByRest( bool includeDetails, string productType, bool productTypeShouldBeExcluded, IEnumerable< int > scopes, DateTime? updatedFrom, IEnumerable< string > skus, bool stockItemsOnly, Mark mark = null )
		{
			const int stockItemsListMaxChunkSize = 1000;
			IEnumerable< Product > resultProducts = new List< Product >();

			IEnumerable<Item> items;
			if( skus != null && skus.Any() )
			{
				items = await skus.ProcessInBatchAsync( 10, async sku => await this.ProductRepository.GetProductAsync( sku ).ConfigureAwait( false ) ).ConfigureAwait( false );
			}
			else
			{
				var productPages = await this.ProductRepository.GetProductsAsync( updatedFrom ?? DateTime.MinValue, productType, productTypeShouldBeExcluded, mark ).ConfigureAwait( false );
				items = productPages.SelectMany( x => x.items );
			}

			if( !items.Any() )
				return resultProducts;

			var productsDevidedByChunks = items.Batch( stockItemsListMaxChunkSize );

			var getStockItemsAsync = new List< InventoryStockItem >();
			foreach( var productsDevidedByChunk in productsDevidedByChunks )
			{
				var catalogInventoryStockItemListResponse = await this.GetStockItemsAsync( productsDevidedByChunk.Select( x => x.sku ).ToList(), scopes, mark ).ConfigureAwait( false );
				getStockItemsAsync.AddRange( catalogInventoryStockItemListResponse.InventoryStockItems.ToList() );
			}
			var stockItems = getStockItemsAsync.ToList();

			if( stockItemsOnly )
				resultProducts = from stockItemEntity in stockItems join productEntity in items on stockItemEntity.ProductId equals productEntity.id.ToString() select new Product( productEntity.id.ToString(), productEntity.id.ToString(), productEntity.name, productEntity.sku, stockItemEntity.Qty, productEntity.price, null, productEntity.typeId, productEntity.updatedAt ) { Weight = productEntity.weight?.ToString( CultureInfo.InvariantCulture ) };
			else
				resultProducts = from productEntity in items join stockItemEntity in stockItems on productEntity.id.ToString() equals stockItemEntity.ProductId into productsList from stockItemEntity in productsList.DefaultIfEmpty() select new Product( productEntity.id.ToString(), productEntity.id.ToString(), productEntity.name, productEntity.sku, stockItemEntity == null ? "0" : stockItemEntity.Qty, productEntity.price, null, productEntity.typeId, productEntity.updatedAt ) { Weight = productEntity.weight?.ToString( CultureInfo.InvariantCulture ) };
			resultProducts = resultProducts.Where( p => !string.IsNullOrWhiteSpace( p.Sku ) ).ToList();

			if( !includeDetails )
				return resultProducts;
			
			foreach( var product in resultProducts )
			{
				var item = items.First( i => i.id.ToString() == product.ProductId );
				var description = item.customAttributes.FirstOrDefault( att => att.attributeCode == "description" );
				if( description != null )
					product.Description = description.value ?? string.Empty;
				
				var shortDescription = item.customAttributes.FirstOrDefault( att => att.attributeCode == "short_description" );
				if( shortDescription != null )
					product.ShortDescription = shortDescription.value ?? string.Empty;
				
				var upcAttribute = item.customAttributes.FirstOrDefault( att => att.attributeCode == "upc" );
				if( upcAttribute != null )
					product.Upc = upcAttribute.value ?? string.Empty;
				
				var specialPriceAttribute = item.customAttributes.FirstOrDefault( att => att.attributeCode == "special_price" );
				if( specialPriceAttribute != null )
				{
					decimal specialPrice;
					if( decimal.TryParse( specialPriceAttribute.value, out specialPrice ) )
						product.SpecialPrice = specialPrice;
				}
				
				var costAttribute = item.customAttributes.FirstOrDefault( att => att.attributeCode == "cost" );
				if( costAttribute != null )
				{
					decimal cost;
					if( decimal.TryParse( costAttribute.value, out cost ) )
						product.Cost = cost;
				}

				product.Images = ( item.customAttributes ?? new List<CustomAttribute>() ).Where( IsImageUrlAttribute ).Select( x => new Models.GetProducts.MagentoUrl( new MagentoImage( x.attributeCode, this.Store + ImagePath + x.value ) ) );
			}
			
			// images
			return resultProducts;
		}

		private static bool IsImageUrlAttribute( CustomAttribute x )
		{
			return string.Compare( x.attributeCode, "swatch_image", StringComparison.CurrentCultureIgnoreCase ) == 0
			       || string.Compare( x.attributeCode, "thumbnail", StringComparison.CurrentCultureIgnoreCase ) == 0
			       || string.Compare( x.attributeCode, "small_image", StringComparison.CurrentCultureIgnoreCase ) == 0
			       || string.Compare( x.attributeCode, "image", StringComparison.CurrentCultureIgnoreCase ) == 0;
		}
	}
}