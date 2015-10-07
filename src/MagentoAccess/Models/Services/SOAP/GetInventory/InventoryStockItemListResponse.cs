using System.Collections.Generic;
using System.Linq;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.SOAP.GetInventory
{
	internal class InventoryStockItemListResponse
	{
		public IEnumerable< InventoryStockItem > InventoryStockItems { get; set; }

		public InventoryStockItemListResponse( catalogInventoryStockItemListResponse res )
		{
			this.InventoryStockItems = res.result.Select( x => new InventoryStockItem( x ) );
		}

		public InventoryStockItemListResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogInventoryStockItemListResponse res )
		{
			this.InventoryStockItems = res.result.Select( x => new InventoryStockItem( x ) );
		}
	}

	internal class InventoryStockItem
	{
		public string Sku { get; set; }

		public string Qty { get; set; }

		public string ProductId { get; set; }

		public string IsInStock { get; set; }

		public InventoryStockItem( catalogInventoryStockItemEntity catalogInventoryStockItemEntity )
		{
			this.IsInStock = catalogInventoryStockItemEntity.is_in_stock;
			this.ProductId = catalogInventoryStockItemEntity.product_id;
			this.Qty = catalogInventoryStockItemEntity.qty;
			this.Sku = catalogInventoryStockItemEntity.sku;
		}

		public InventoryStockItem( MagentoSoapServiceReference_v_1_14_1_EE.catalogInventoryStockItemEntity catalogInventoryStockItemEntity )
		{
			this.IsInStock = catalogInventoryStockItemEntity.is_in_stock;
			this.ProductId = catalogInventoryStockItemEntity.product_id;
			this.Qty = catalogInventoryStockItemEntity.qty;
			this.Sku = catalogInventoryStockItemEntity.sku;
		}
	}
}