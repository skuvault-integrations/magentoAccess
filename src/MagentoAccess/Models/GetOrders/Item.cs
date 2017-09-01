using System;

namespace MagentoAccess.Models.GetOrders
{
	[ Serializable ]
	public class Item
	{
		public string ItemId { get; set; }
		public string ParentItemId { get; set; }
		public string ProductType { get; set; }
		public string Sku { get; set; }
		public string Name { get; set; }
		public decimal QtyCanceled { get; set; }
		public decimal QtyInvoiced { get; set; }
		public decimal QtyOrdered { get; set; }
		public decimal QtyRefunded { get; set; }
		public decimal QtyShipped { get; set; }
		public decimal Price { get; set; }
		public decimal BasePrice { get; set; }
		public decimal OriginalPrice { get; set; }
		public decimal BaseOriginalPrice { get; set; }
		public decimal TaxPercent { get; set; }
		public decimal TaxAmount { get; set; }
		public decimal BaseTaxAmount { get; set; }
		public decimal DscountAmount { get; set; }
		public decimal BaseDiscountAmount { get; set; }
		public decimal RowTotal { get; set; }
		public decimal BaseRowTotal { get; set; }
		public decimal PriceInclTax { get; set; }
		public decimal BasePriceInclTax { get; set; }
		public decimal RawTotalInclTax { get; set; }
		public decimal BaseRowTotalInclTax { get; set; }

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( this, obj ) )
				return true;

			var item = obj as Item;
			if( item == null )
				return false;
			else
				return this.Equals( item );
		}

		public bool Equals( Item item )
		{
			if( item == null )
				return false;
			var res = Equals( this.ItemId, item.ItemId )
			          && Equals( this.ParentItemId, item.ParentItemId )
			          && Equals( this.ProductType, item.ProductType )
			          && Equals( this.Sku, item.Sku )
			          && Equals( this.Name, item.Name )
			          && Equals( this.QtyCanceled, item.QtyCanceled )
			          && Equals( this.QtyInvoiced, item.QtyInvoiced )
			          && Equals( this.QtyOrdered, item.QtyOrdered )
			          && Equals( this.QtyRefunded, item.QtyRefunded )
			          && Equals( this.QtyShipped, item.QtyShipped )
			          && Equals( this.Price, item.Price )
			          && Equals( this.BasePrice, item.BasePrice )
			          && Equals( this.OriginalPrice, item.OriginalPrice )
			          && Equals( this.BaseOriginalPrice, item.BaseOriginalPrice )
			          && Equals( this.TaxPercent, item.TaxPercent )
			          && Equals( this.TaxAmount, item.TaxAmount )
			          && Equals( this.BaseTaxAmount, item.BaseTaxAmount )
			          && Equals( this.DscountAmount, item.DscountAmount )
			          && Equals( this.BaseDiscountAmount, item.BaseDiscountAmount )
			          && Equals( this.RowTotal, item.RowTotal )
			          && Equals( this.BaseRowTotal, item.BaseRowTotal )
			          && Equals( this.PriceInclTax, item.PriceInclTax )
			          && Equals( this.BasePriceInclTax, item.BasePriceInclTax )
			          && Equals( this.RawTotalInclTax, item.RawTotalInclTax )
			          && Equals( this.BaseRowTotalInclTax, item.BaseRowTotalInclTax );

			return res;
		}
	}

	public static class ItemExtensions
	{
		public static bool IsShipped( this Item item )
		{
			return item.QtyOrdered.CompareTo( item.QtyCanceled + item.QtyShipped ) == 0;
		}
	}
}