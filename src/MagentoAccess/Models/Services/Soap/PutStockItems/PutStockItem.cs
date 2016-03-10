using MagentoAccess.Models.PutInventory;

namespace MagentoAccess.Models.Services.Soap.PutStockItems
{
	internal class PutStockItem
	{
		//public PutStockItem( string id, catalogInventoryStockItemUpdateEntity updateEntity )
		//{
		//	this.Id = id;
		//	this.Backorders = updateEntity.backorders;
		//	this.IsInStock = updateEntity.is_in_stock;
		//	this.IsQtyDecimal = updateEntity.is_qty_decimal;
		//	this.ManageStock = updateEntity.manage_stock;
		//	this.MaxSaleQty = updateEntity.max_sale_qty;
		//	this.MinQty = updateEntity.min_qty;
		//	this.MinSaleQty = updateEntity.min_sale_qty;
		//	this.NotifyStockQty = updateEntity.notify_stock_qty;
		//	this.UseConfigBackorders = updateEntity.use_config_backorders;
		//	this.UseConfigManageStock = updateEntity.use_config_manage_stock;
		//	this.UseConfigMaxSaleQty = updateEntity.use_config_max_sale_qty;
		//	this.UseConfigMinQty = updateEntity.use_config_min_qty;
		//	this.UseConfigMinSaleQty = updateEntity.use_config_min_sale_qty;
		//	this.UseConfigNotifyStockQty = updateEntity.use_config_notify_stock_qty;
		//	this.BackordersSpecified = updateEntity.backordersSpecified;
		//	this.IsInStockSpecified = updateEntity.is_in_stockSpecified;
		//	this.IsQtyDecimalSpecified = updateEntity.is_qty_decimalSpecified;
		//	this.ManageStockSpecified = updateEntity.manage_stockSpecified;
		//	this.MaxSaleQtySpecified = updateEntity.max_sale_qtySpecified;
		//	this.MinQtySpecified = updateEntity.min_qtySpecified;
		//	this.MinSaleQtySpecified = updateEntity.min_sale_qtySpecified;
		//	this.NotifyStockQtySpecified = updateEntity.notify_stock_qtySpecified;
		//	this.Qty = updateEntity.qty;
		//	this.UseConfigBackordersSpecified = updateEntity.use_config_backordersSpecified;
		//	this.UseConfigManageStockSpecified = updateEntity.use_config_manage_stockSpecified;
		//	this.UseConfigMaxSaleQtySpecified = updateEntity.use_config_max_sale_qtySpecified;
		//	this.UseConfigMinQtySpecified = updateEntity.use_config_min_qtySpecified;
		//	this.UseConfigMinSaleQtySpecified = updateEntity.use_config_min_sale_qtySpecified;
		//	this.UseConfigNotifyStockQtySpecified = updateEntity.use_config_notify_stock_qtySpecified;
		//}

		//public int Backorders { get; set; }
		//public int IsInStock { get; set; }
		//public int IsQtyDecimal { get; set; }
		//public int ManageStock { get; set; }
		//public int MaxSaleQty { get; set; }
		//public int MinQty { get; set; }
		//public int MinSaleQty { get; set; }
		//public int NotifyStockQty { get; set; }
		//public int UseConfigBackorders { get; set; }
		//public int UseConfigManageStock { get; set; }
		//public int UseConfigMaxSaleQty { get; set; }
		//public int UseConfigMinQty { get; set; }
		//public int UseConfigMinSaleQty { get; set; }
		//public int UseConfigNotifyStockQty { get; set; }
		//public bool BackordersSpecified { get; set; }
		//public bool IsInStockSpecified { get; set; }
		//public bool IsQtyDecimalSpecified { get; set; }
		//public bool ManageStockSpecified { get; set; }
		//public bool MaxSaleQtySpecified { get; set; }
		//public bool MinQtySpecified { get; set; }
		//public bool MinSaleQtySpecified { get; set; }
		//public bool NotifyStockQtySpecified { get; set; }
		//public string Qty { get; set; }
		//public bool UseConfigBackordersSpecified { get; set; }
		//public bool UseConfigManageStockSpecified { get; set; }
		//public bool UseConfigMaxSaleQtySpecified { get; set; }
		//public bool UseConfigMinQtySpecified { get; set; }
		//public bool UseConfigMinSaleQtySpecified { get; set; }
		//public bool UseConfigNotifyStockQtySpecified { get; set; }

		//public string Id { get; set; }

		public PutStockItem( Inventory updateEntity )
		{
			this.ItemId = updateEntity.ItemId;
			this.ProductId = updateEntity.ProductId;
			this.MinQty = updateEntity.MinQty;
			this.Qty = updateEntity.Qty;
			this.StockId = updateEntity.StockId;
			this.Sku = updateEntity.Sku;
		}

		public string Sku{ get; set; }

		public string StockId { get; set; }

		public long Qty { get; set; }

		public long MinQty { get; set; }

		public string ProductId { get; set; }

		public string ItemId { get; set; }
	}
}