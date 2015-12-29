using System.Collections.Generic;
using System.Linq;
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
			Weight = catalogProductInfoResponse.result.weight;
			ProductId = catalogProductInfoResponse.result.product_id;
			CategoryIds = catalogProductInfoResponse.result.category_ids;

			if( catalogProductInfoResponse.result.additional_attributes != null && catalogProductInfoResponse.result.additional_attributes.Any() )
				Attributes = catalogProductInfoResponse.result.additional_attributes.Select( x => new ProductAttribute( x.key, x.value ) ).ToList();
		}

		public List< ProductAttribute > Attributes { get; set; }

		public CatalogProductInfoResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductInfoResponse catalogProductInfoResponse )
		{
			Description = catalogProductInfoResponse.result.description;
			ShortDescription = catalogProductInfoResponse.result.short_description;
			Price = catalogProductInfoResponse.result.price;
			Weight = catalogProductInfoResponse.result.weight;
			ProductId = catalogProductInfoResponse.result.product_id;
			CategoryIds = catalogProductInfoResponse.result.category_ids;

			if( catalogProductInfoResponse.result.additional_attributes != null && catalogProductInfoResponse.result.additional_attributes.Any() )
				Attributes = catalogProductInfoResponse.result.additional_attributes.Select( x => new ProductAttribute( x.key, x.value ) ).ToList();
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

		private class ProductAttributeCodes
		{
			public const string Cost = "cost";
			public const string Manufacturer = "manufacturer";
		}
	}
}