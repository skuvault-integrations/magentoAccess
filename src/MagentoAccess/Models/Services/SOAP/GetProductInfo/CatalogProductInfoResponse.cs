using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using MagentoAccess.Magento2catalogProductRepositoryV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProductInfo
{
	internal class CatalogProductInfoResponse: ResponseWithExceptions
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

		public string SpecialPrice{ get; set; }

		public List< ProductAttribute > Attributes{ get; set; }

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
			if( catalogProductInfoResponse == null
			    || catalogProductInfoResponse.catalogProductRepositoryV1GetResponse == null
			    || catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result == null )
				return;

			var catalogDataProductInterface = catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result;

			this.ShortDescription = string.Empty;
			if( !string.IsNullOrWhiteSpace( catalogDataProductInterface.price ) )
				this.Price = catalogDataProductInterface.price.ToString( CultureInfo.InvariantCulture );
			if( !string.IsNullOrWhiteSpace( catalogDataProductInterface.weight ) )
				this.Weight = catalogDataProductInterface.weight.ToString( CultureInfo.InvariantCulture );
			this.ProductId = catalogDataProductInterface.id.ToString( CultureInfo.InvariantCulture );

			if( catalogDataProductInterface.customAttributes != null
			    && catalogDataProductInterface.customAttributes.Any() )
			{
				this.Description = GetSimpleStringCustomAttribute( catalogDataProductInterface, "description" );
				this.SpecialPrice = GetSimpleStringCustomAttribute( catalogDataProductInterface, "special_price" );
				this.CategoryIds = GetArrayOfStringCustomAttribute( catalogDataProductInterface, "category_ids" );

				this.Attributes = catalogDataProductInterface.customAttributes.Select( x => new ProductAttribute( x.attributeCode, GetCustomAttribute( catalogDataProductInterface, x.attributeCode ) ) ).ToList();
			}
		}

		public CatalogProductInfoResponse( Exception catalogProductInfoResponse )
		{
			this.Exc = catalogProductInfoResponse;
		}

		public CatalogProductInfoResponse( Magento2catalogProductRepositoryV1_v_2_1_0_0_CE.catalogProductRepositoryV1GetResponse1 catalogProductInfoResponse )
		{
			if( catalogProductInfoResponse == null
			    || catalogProductInfoResponse.catalogProductRepositoryV1GetResponse == null
			    || catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result == null )
				return;

			var catalogDataProductInterface = catalogProductInfoResponse.catalogProductRepositoryV1GetResponse.result;

			this.ShortDescription = string.Empty;
			this.Price = catalogDataProductInterface.price.ToString( CultureInfo.InvariantCulture );
			this.Weight = catalogDataProductInterface.weight.ToString( CultureInfo.InvariantCulture );
			this.ProductId = catalogDataProductInterface.id.ToString( CultureInfo.InvariantCulture );

			if( catalogDataProductInterface.customAttributes != null
			    && catalogDataProductInterface.customAttributes.Any() )
			{
				this.Description = GetSimpleStringCustomAttribute( catalogDataProductInterface, "description" );
				this.SpecialPrice = GetSimpleStringCustomAttribute( catalogDataProductInterface, "special_price" );
				this.CategoryIds = GetArrayOfStringCustomAttribute( catalogDataProductInterface, "category_ids" );

				this.Attributes = catalogDataProductInterface.customAttributes.Select( x => new ProductAttribute( x.attributeCode, GetCustomAttribute( catalogDataProductInterface, x.attributeCode ) ) ).ToList();
			}
		}

		private string GetCustomAttribute( Magento2catalogProductRepositoryV1_v_2_1_0_0_CE.CatalogDataProductInterface catalogDataProductInterface, string attributesCode )
		{
			var descriptionNodes = ( catalogDataProductInterface.customAttributes.FirstOrDefault( x => string.Equals( x.attributeCode, attributesCode, StringComparison.InvariantCultureIgnoreCase ) ) ?? new Magento2catalogProductRepositoryV1_v_2_1_0_0_CE.FrameworkAttributeInterface() ).value;
			if( descriptionNodes is XmlNode[] )
			{
				var nodeValue = descriptionNodes as XmlNode[];
				var temp = new List< string >();
				if( nodeValue.Length > 0 )
					temp.AddRange( from XmlNode xmlNode in nodeValue where xmlNode != null select xmlNode.InnerText );
				return string.Join( ",", temp.ToArray() );
			}
			else if( descriptionNodes is XmlNode )
			{
				var nodeValue = descriptionNodes as XmlNode;
				string temp = nodeValue.InnerText;
				return temp;
			}
			else
				return null;
		}

		private static string GetCustomAttribute( CatalogDataProductInterface catalogDataProductInterface, string attributesCode )
		{
			var descriptionNodes = ( catalogDataProductInterface.customAttributes.FirstOrDefault( x => string.Equals( x.attributeCode, attributesCode, StringComparison.InvariantCultureIgnoreCase ) ) ?? new FrameworkAttributeInterface() ).value;
			if( descriptionNodes is XmlNode[] )
			{
				var nodeValue = descriptionNodes as XmlNode[];
				var temp = new List< string >();
				if( nodeValue.Length > 0 )
					temp.AddRange( from XmlNode xmlNode in nodeValue where xmlNode != null select xmlNode.InnerText );
				return string.Join( ",", temp.ToArray() );
			}
			else if( descriptionNodes is XmlNode )
			{
				var nodeValue = descriptionNodes as XmlNode;
				string temp = nodeValue.InnerText;
				return temp;
			}
			else
				return null;
		}

		private static string[] GetArrayOfStringCustomAttribute( CatalogDataProductInterface catalogDataProductInterface, string attributesCode )
		{
			var frameworkAttributeInterfaces = catalogDataProductInterface.customAttributes;
			var firstOrDefault = frameworkAttributeInterfaces.FirstOrDefault( x => string.Equals( x.attributeCode, attributesCode, StringComparison.InvariantCultureIgnoreCase ) );
			var frameworkAttributeInterface = firstOrDefault != null ? firstOrDefault : new FrameworkAttributeInterface();
			var descriptionNodes = frameworkAttributeInterface.value as XmlNode[];
			var temp = new List< string >();
			if( descriptionNodes != null && descriptionNodes.Length > 0 )
				temp.AddRange( from XmlNode node in descriptionNodes where node != null select node.InnerText );
			return temp.ToArray();
		}

		private string[] GetArrayOfStringCustomAttribute( Magento2catalogProductRepositoryV1_v_2_1_0_0_CE.CatalogDataProductInterface catalogDataProductInterface, string attributesCode )
		{
			var frameworkAttributeInterfaces = catalogDataProductInterface.customAttributes;
			var firstOrDefault = frameworkAttributeInterfaces.FirstOrDefault( x => string.Equals( x.attributeCode, attributesCode, StringComparison.InvariantCultureIgnoreCase ) );
			var frameworkAttributeInterface = firstOrDefault != null ? firstOrDefault : new Magento2catalogProductRepositoryV1_v_2_1_0_0_CE.FrameworkAttributeInterface();
			var descriptionNodes = frameworkAttributeInterface.value as XmlNode[];
			var temp = new List< string >();
			if( descriptionNodes != null && descriptionNodes.Length > 0 )
				temp.AddRange( from XmlNode node in descriptionNodes where node != null select node.InnerText );
			return temp.ToArray();
		}

		private static string GetSimpleStringCustomAttribute( CatalogDataProductInterface catalogDataProductInterface, string attributesCode )
		{
			var frameworkAttributeInterface = catalogDataProductInterface.customAttributes.FirstOrDefault( x => string.Equals( x.attributeCode, attributesCode, StringComparison.InvariantCultureIgnoreCase ) ) ?? new FrameworkAttributeInterface();
			var descriptionNodes = frameworkAttributeInterface.value as XmlNode[];
			string temp = null;
			if( descriptionNodes != null && descriptionNodes.Length > 0 )
				temp = string.Join( "", descriptionNodes.Where( x => x != null ).Select( x => x.InnerText ) );
			return temp;
		}

		private string GetSimpleStringCustomAttribute( Magento2catalogProductRepositoryV1_v_2_1_0_0_CE.CatalogDataProductInterface catalogDataProductInterface, string attributesCode )
		{
			var frameworkAttributeInterface = catalogDataProductInterface.customAttributes.FirstOrDefault( x => string.Equals( x.attributeCode, attributesCode, StringComparison.InvariantCultureIgnoreCase ) ) ?? new Magento2catalogProductRepositoryV1_v_2_1_0_0_CE.FrameworkAttributeInterface();
			var descriptionNodes = frameworkAttributeInterface.value as XmlNode[];
			string temp = null;
			if( descriptionNodes != null && descriptionNodes.Length > 0 )
				temp = string.Join( "", descriptionNodes.Where( x => x != null ).Select( x => x.InnerText ) );
			return temp;
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

		public string[] CategoryIds{ set; get; }

		public string ProductId{ get; set; }

		public string Weight{ get; set; }

		public string Price{ get; set; }

		public string ShortDescription{ get; set; }

		public string Description{ get; set; }
	}
}