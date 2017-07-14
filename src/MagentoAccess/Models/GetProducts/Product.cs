using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetOrders;

namespace MagentoAccess.Models.GetProducts
{
	[ Serializable ]
	public class Product : IEquatable< Product >
	{
		public Product( Product rp, IEnumerable< MagentoUrl > images = null, string upc = null, string manufacturer = null, decimal? cost = null, string weight = null, string shortDescription = null, string description = null, string price = null, string specialPrice = null, IEnumerable< Category > categories = null, string productType = null )
		{
			this.EntityId = rp.EntityId;
			this.Sku = rp.Sku;
			this.Name = rp.Name;
			this.Qty = rp.Qty;
			this.UpdatedAt = rp.UpdatedAt;
			this.Categories = categories == null ? rp.Categories : categories.ToArray();

			this.Images = images ?? rp.Images;
			decimal temp;
			this.Price = price.ToDecimalOrDefault( out temp ) ? temp : rp.Price;
			this.SpecialPrice = specialPrice.ToDecimalOrDefault( out temp ) ? temp : rp.SpecialPrice;
			this.Description = description ?? rp.Description;
			this.ProductId = rp.ProductId;
			this.Weight = weight ?? rp.Weight;
			this.ShortDescription = shortDescription ?? rp.ShortDescription;

			this.Manufacturer = manufacturer ?? rp.Manufacturer;
			this.Cost = cost ?? rp.Cost;
			this.Upc = upc ?? rp.Upc;
			this.ProductType = productType ?? rp.ProductType;
		}

		public Product( string productId, string entityId, string name, string sku, string qty, decimal price, string description, string productType, string updatedAt )
		{
			this.ProductId = productId;
			this.EntityId = entityId;
			this.Name = name;
			this.Sku = sku;
			this.Qty = qty;
			this.UpdatedAt = updatedAt;
			this.Price = price;
			this.Description = description;
			this.ProductType = productType;
		}

		public string Upc{ get; set; }
		public decimal SpecialPrice{ get; set; }
		public decimal Cost{ get; set; }
		public string Manufacturer{ get; set; }
		public IEnumerable< MagentoUrl > Images{ get; set; } //imagento2
		public string ShortDescription{ get; set; }
		public string Weight{ get; set; } //imagento2
		public string EntityId{ get; set; } //id
		public string Sku{ get; set; } //imagento2
		public decimal Price{ get; set; } //imagento2
		public string Name{ get; set; } //imagento2
		public string Description{ get; set; } //custom attributes
		public string Qty{ get; set; }
		public string ProductId{ get; set; } //id
		public Category[] Categories{ get; set; }
		public string ProductType{ get; set; }
		public string UpdatedAt { get; set; }

		//category_ids have many
		public bool Equals( Product other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			
			return string.Equals( this.Upc, other.Upc ) && 
			       this.SpecialPrice == other.SpecialPrice && 
			       this.Cost == other.Cost && 
			       string.Equals( this.Manufacturer, other.Manufacturer ) && 
			       Equals( this.Images, other.Images ) && 
			       string.Equals( this.ShortDescription, other.ShortDescription ) && 
			       string.Equals( this.Weight, other.Weight ) && 
			       string.Equals( this.EntityId, other.EntityId ) && 
			       string.Equals( this.Sku, other.Sku ) && 
			       this.Price == other.Price && 
			       string.Equals( this.Name, other.Name ) && 
			       string.Equals( this.Description, other.Description ) && 
			       string.Equals( this.Qty, other.Qty ) && 
			       string.Equals( this.ProductId, other.ProductId ) && 
			       Equals( this.Categories, other.Categories ) && 
			       string.Equals( this.ProductType, other.ProductType ) && 
			       string.Equals( this.UpdatedAt, other.UpdatedAt );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return Equals( ( Product )obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ( this.Upc != null ? this.Upc.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ this.SpecialPrice.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.Cost.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ ( this.Manufacturer != null ? this.Manufacturer.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.Images != null ? this.Images.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.ShortDescription != null ? this.ShortDescription.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.Weight != null ? this.Weight.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.EntityId != null ? this.EntityId.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.Sku != null ? this.Sku.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ this.Price.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ ( this.Name != null ? this.Name.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.Description != null ? this.Description.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.Qty != null ? this.Qty.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.ProductId != null ? this.ProductId.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.Categories != null ? this.Categories.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.ProductType != null ? this.ProductType.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.UpdatedAt != null ? this.UpdatedAt.GetHashCode() : 0 );
				return hashCode;
			}
		}
	}
}