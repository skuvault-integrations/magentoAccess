using System;

namespace MagentoAccess.Models.PutInventory
{
	public class Inventory : IEquatable< Inventory >
	{
		public string ItemId{ get; set; }
		public string ProductId{ get; set; }
		public string StockId{ get; set; }
		public long Qty{ get; set; }
		public long MinQty{ get; set; }

		//used in Magento2
		public string Sku{ get; set; }

		public bool Equals( Inventory other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return string.Equals( this.ItemId, other.ItemId ) && string.Equals( this.ProductId, other.ProductId ) && string.Equals( this.StockId, other.StockId ) && this.Qty == other.Qty && this.MinQty == other.MinQty && string.Equals( this.Sku, other.Sku );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return Equals( ( Inventory )obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ( this.ItemId != null ? this.ItemId.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.ProductId != null ? this.ProductId.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.StockId != null ? this.StockId.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ this.Qty.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.MinQty.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ ( this.Sku != null ? this.Sku.GetHashCode() : 0 );
				return hashCode;
			}
		}
	}

	public class InventoryBySku : IEquatable< InventoryBySku >
	{
		public string Sku{ get; set; }
		public string StockId{ get; set; }
		public long Qty{ get; set; }
		public long MinQty{ get; set; }

		public bool Equals( InventoryBySku other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return string.Equals( this.Sku, other.Sku ) && string.Equals( this.StockId, other.StockId ) && this.Qty == other.Qty && this.MinQty == other.MinQty;
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return Equals( ( InventoryBySku )obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ( this.Sku != null ? this.Sku.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ ( this.StockId != null ? this.StockId.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ this.Qty.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.MinQty.GetHashCode();
				return hashCode;
			}
		}
	}
}