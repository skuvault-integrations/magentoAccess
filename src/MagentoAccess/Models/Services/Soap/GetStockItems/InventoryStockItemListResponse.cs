using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MagentoAccess.M2catalogInventoryStockRegistryV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;

namespace MagentoAccess.Models.Services.Soap.GetStockItems
{
	internal class InventoryStockItemListResponse
	{
		public IEnumerable< InventoryStockItem > InventoryStockItems{ get; set; }

		public InventoryStockItemListResponse( catalogInventoryStockItemListResponse res )
		{
			this.InventoryStockItems = res.result.Select( x => new InventoryStockItem( x ) );
		}

		public InventoryStockItemListResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogInventoryStockItemListResponse res )
		{
			this.InventoryStockItems = res.result.Select( x => new InventoryStockItem( x ) );
		}

		public InventoryStockItemListResponse( IEnumerable< Tuple< string, CatalogInventoryDataStockItemInterface > > responses )
		{
			this.InventoryStockItems = responses.Select( x => new InventoryStockItem( x.Item1, x.Item2 ) );
		}

		public InventoryStockItemListResponse( IEnumerable< Tuple< string, M2catalogInventoryStockRegistryV1_v_2_1_0_0_CE.CatalogInventoryDataStockItemInterface > > responses )
		{
			this.InventoryStockItems = responses.Select(x => new InventoryStockItem(x.Item1, x.Item2));
		}
	}

	internal class InventoryStockItem
	{
		public string Sku{ get; set; }

		public string Qty{ get; set; }

		public string ProductId{ get; set; }

		public string IsInStock{ get; set; }

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

		public InventoryStockItem( string sku, CatalogInventoryDataStockItemInterface catalogInventoryStockItemEntity )
		{
			this.IsInStock = catalogInventoryStockItemEntity.isInStock.ToString( CultureInfo.InvariantCulture );
			this.ProductId = catalogInventoryStockItemEntity.productId.ToString( CultureInfo.InvariantCulture );
			this.Qty = catalogInventoryStockItemEntity.qty.ToDecimalOrDefault().ToString( CultureInfo.InvariantCulture );
			this.Sku = sku;
		}

		public InventoryStockItem( string sku, M2catalogInventoryStockRegistryV1_v_2_1_0_0_CE.CatalogInventoryDataStockItemInterface catalogInventoryStockItemEntity )
		{
			this.IsInStock = catalogInventoryStockItemEntity.isInStock.ToString( CultureInfo.InvariantCulture );
			this.ProductId = catalogInventoryStockItemEntity.productId.ToString( CultureInfo.InvariantCulture );
			this.Qty = catalogInventoryStockItemEntity.qty.ToString( CultureInfo.InvariantCulture );
			this.Sku = sku;
		}
	}
}