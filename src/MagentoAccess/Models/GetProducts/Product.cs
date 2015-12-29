using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Misc;

namespace MagentoAccess.Models.GetProducts
{
	public class Product
	{
		public Product( Product rp, IEnumerable< MagentoUrl > images = null, string manufacturer = null, decimal? cost = null, string weight = null, string shortDescription = null, string description = null, string price = null, string specialPrice = null, IEnumerable< Category > categories = null )
		{
			EntityId = rp.EntityId;
			Sku = rp.Sku;
			Name = rp.Name;
			Qty = rp.Qty;
			Categories = categories == null ? rp.Categories : categories.ToArray();

			Images = images ?? rp.Images;
			decimal temp;
			Price = price.ToDecimalOrDefault( out temp ) ? temp : rp.Price;
			SpecialPrice = specialPrice.ToDecimalOrDefault( out temp ) ? temp : rp.SpecialPrice;
			Description = description ?? rp.Description;
			ProductId = rp.ProductId;
			Weight = weight ?? rp.Weight;
			ShortDescription = shortDescription ?? rp.ShortDescription;

			Manufacturer = manufacturer ?? rp.Manufacturer;
			Cost = cost ?? rp.Cost;
		}

		public decimal SpecialPrice { get; set; }

		public Product( string productId, string entityId, string name, string sku, string qty, decimal price, string description )
		{
			ProductId = productId;
			EntityId = entityId;
			Name = name;
			Sku = sku;
			Qty = qty;
			Price = price;
			Description = description;
		}

		public decimal Cost { get; set; }
		public string Manufacturer { get; set; }
		public IEnumerable< MagentoUrl > Images { get; set; }
		public string ShortDescription { get; set; }
		public string Weight { get; set; }
		public string EntityId { get; set; }
		public string Sku { get; set; }
		public decimal Price { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Qty { get; set; }
		public string ProductId { get; set; }
		public Category[] Categories { get; set; }
	}
}