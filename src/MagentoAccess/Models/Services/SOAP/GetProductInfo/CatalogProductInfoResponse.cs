using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MagentoAccess.Magento2catalogProductRepositoryV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProductInfo
{
	internal class CatalogProductInfoResponse
	{
		public CatalogProductInfoResponse( catalogProductInfoResponse catalogProductInfoResponse )
		{
			Description = catalogProductInfoResponse.result.description;
			ShortDescription = catalogProductInfoResponse.result.short_description;
			Price = catalogProductInfoResponse.result.price;
			SpecialPrice = catalogProductInfoResponse.result.special_price;
			Weight = catalogProductInfoResponse.result.weight;
			ProductId = catalogProductInfoResponse.result.product_id;
			CategoryIds = catalogProductInfoResponse.result.category_ids;

			if( catalogProductInfoResponse.result.additional_attributes != null && catalogProductInfoResponse.result.additional_attributes.Any() )
				Attributes = catalogProductInfoResponse.result.additional_attributes.Select( x => new ProductAttribute( x.key, x.value ) ).ToList();
		}

		public string SpecialPrice { get; set; }

		public List< ProductAttribute > Attributes { get; set; }

		public CatalogProductInfoResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductInfoResponse catalogProductInfoResponse )
		{
			Description = catalogProductInfoResponse.result.description;
			ShortDescription = catalogProductInfoResponse.result.short_description;
			Price = catalogProductInfoResponse.result.price;
			SpecialPrice = catalogProductInfoResponse.result.special_price;
			Weight = catalogProductInfoResponse.result.weight;
			ProductId = catalogProductInfoResponse.result.product_id;
			CategoryIds = catalogProductInfoResponse.result.category_ids;

			if( catalogProductInfoResponse.result.additional_attributes != null && catalogProductInfoResponse.result.additional_attributes.Any() )
				Attributes = catalogProductInfoResponse.result.additional_attributes.Select( x => new ProductAttribute( x.key, x.value ) ).ToList();
		}

		public CatalogProductInfoResponse( catalogProductRepositoryV1GetResponse1 catalogProductInfoResponse )
		{
			this.ShortDescription = string.Empty;
			this.Price = catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result.price.ToString( CultureInfo.InvariantCulture );
			this.Weight = catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result.weight.ToString( CultureInfo.InvariantCulture );
			this.ProductId = catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result.id.ToString( CultureInfo.InvariantCulture );

			if( catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result.customAttributes != null
			    && catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result.customAttributes.Any() )
			{
				this.Description = ( string )( catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result.customAttributes.FirstOrDefault( x => string.Equals( x.attributeCode, "description", StringComparison.InvariantCultureIgnoreCase ) ) ?? new FrameworkAttributeInterface() ).value;
				this.SpecialPrice = ( string )( catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result.customAttributes.FirstOrDefault( x => x.attributeCode.Equals( "special_price", StringComparison.InvariantCultureIgnoreCase ) ) ?? new FrameworkAttributeInterface() ).value;
				this.CategoryIds = ( string[] )( catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result.customAttributes.FirstOrDefault( x => x.attributeCode.Equals( "category_ids", StringComparison.InvariantCultureIgnoreCase ) ) ?? new FrameworkAttributeInterface() ).value;
				this.Attributes = catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result.customAttributes.Select( x => new ProductAttribute( x.attributeCode, x.value.ToString() ) ).ToList();
			}
		}

		private string GetAttributeValue( string sttributeName )
		{
			if( Attributes != null )
			{
				var attributeValue = Attributes.FirstOrDefault( x => x.Key == sttributeName );
				return attributeValue != null ? attributeValue.Value : null;
			}
			else
				return null;
		}

		public string GetManufacturerAttributeValue()
		{
			return GetAttributeValue( ProductAttributeCodes.Manufacturer );
		}

		public string GetUpcAttributeValue()
		{
			return GetAttributeValue( ProductAttributeCodes.Upc );
		}

		public string GetCostAttributeValue()
		{
			return GetAttributeValue( ProductAttributeCodes.Cost );
		}

		public string[] CategoryIds { set; get; }

		public string ProductId { get; set; }

		public string Weight { get; set; }

		public string Price { get; set; }

		public string ShortDescription { get; set; }

		public string Description { get; set; }
	}
}