using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Misc;

namespace MagentoAccess.Models.GetProducts
{
	[ Serializable ]
	public class Product
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
	}
}