using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Rest.v2x.Products;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Services.Soap;
using Netco.Extensions;
using Netco.Logging;
using Newtonsoft.Json;

namespace MagentoAccess.Services.Rest.v2x
{
	internal partial class MagentoServiceLowLevel : IMagentoServiceLowLevelSoap, IMagentoServiceLowLevelSoapFillProductsDetails
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
			var productsList = resultProducts.ToList();
			
			if( !productsList.Any() )
				return productsList;

			var items = await productsList.Select( p => p.Sku ).ProcessInBatchAsync( 10, async sku => await this.ProductRepository.GetProductAsync( sku ).ConfigureAwait( false ) ).ConfigureAwait( false );
			if( !items.Any() )
				return productsList;

			var allCategories = ( await this.ProductRepository.GetCategoriesTreeAsync().ConfigureAwait( false ) ).Flatten();
			var allManufacturers = await this.ProductRepository.GetManufacturersAsync().ConfigureAwait( false );
			foreach( var product in productsList )
			{
				var item = items.FirstOrDefault( i => i.id.ToString() == product.ProductId );
				if (item == null)
					continue;
				product.Price = item.price;
				product.Weight = item.weight?.ToString();
				
				var description = item.customAttributes.FirstOrDefault( att => att.attributeCode == "description" );
				if( description != null )
					product.Description = description.value ?? string.Empty;
				
				var shortDescription = item.customAttributes.FirstOrDefault( att => att.attributeCode == "short_description" );
				if( shortDescription != null )
					product.ShortDescription = shortDescription.value ?? string.Empty;
				
				var manufacturer = item.customAttributes.FirstOrDefault( att => att.attributeCode == "manufacturer" );
				if( manufacturer != null )
				{
					product.Manufacturer = manufacturer.value ?? string.Empty;

					if ( allManufacturers.options != null && allManufacturers.options.Any() )
					{
						var manufacturerOption = allManufacturers.options.Where( o => o.value != null && o.value.Equals( product.Manufacturer ) ).FirstOrDefault();
						if ( manufacturerOption != null )
						{
							product.Manufacturer = manufacturerOption.label;
						}
					}
				}
				
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

				product.Images = ( item.customAttributes ?? new List<CustomAttribute>() ).Where( IsImageUrlAttribute ).Select( x => new Models.Services.Soap.GetProducts.MagentoUrl( new MagentoImage( x.attributeCode, this.Store + ImagePath + x.value ) ) ).ToArray();

				var categoriesAttributes = item.customAttributes.FirstOrDefault( att => att.attributeCode == "category_ids" );
				if( allCategories.Any() && categoriesAttributes != null )
				{
					var categories = JsonConvert.DeserializeObject< int[] >( categoriesAttributes.value ?? string.Empty );
					product.Categories = allCategories.Join( categories, a => a.id, b => b, ( node, i ) => new Models.Services.Soap.GetProducts.Category( node ) ).ToArray();
				}
			}
			
			return productsList;
		}

		public Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, bool throwException = true )
		{
			return null;
		}

		private static bool IsImageUrlAttribute( CustomAttribute x )
		{
			return string.Compare( x.attributeCode, "swatch_image", StringComparison.OrdinalIgnoreCase ) == 0
			       || string.Compare( x.attributeCode, "thumbnail", StringComparison.OrdinalIgnoreCase ) == 0
			       || string.Compare( x.attributeCode, "small_image", StringComparison.OrdinalIgnoreCase ) == 0
			       || string.Compare( x.attributeCode, "image", StringComparison.OrdinalIgnoreCase ) == 0;
		}
	}
}