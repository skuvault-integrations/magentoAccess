using MagentoAccess.Models.Services.Rest.v2x.Products;
using Newtonsoft.Json;

namespace MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository
{
	public class ExtensionAttributes
	{
	}

	[ JsonConverter( typeof( LaxPropertyNameMatchingConverter ) ) ]
	public class StockItem
	{
		public int? itemId { get; set; }
		public int? productId { get; set; }
		public int? stockId { get; set; }
		public long? qty { get; set; }
		public bool? isInStock { get; set; }
		public bool? isQtyDecimal { get; set; }
		public bool? showDefaultNotificationMessage { get; set; }
		public bool? useConfigMinQty { get; set; }
		public long? minQty { get; set; }
		public int? useConfigMinSaleQty { get; set; }
		public int? minSaleQty { get; set; }
		public bool? useConfigMaxSaleQty { get; set; }
		public int? maxSaleQty { get; set; }
		public bool? useConfigBackorders { get; set; }
		public int? backorders { get; set; }
		public bool? useConfigNotifyStockQty { get; set; }
		public int? notifyStockQty { get; set; }
		public bool? useConfigQtyIncrements { get; set; }
		public int? qtyIncrements { get; set; }
		public bool? useConfigEnableQtyInc { get; set; }
		public bool? enableQtyIncrements { get; set; }
		public bool? useConfigManageStock { get; set; }
		public bool? manageStock { get; set; }
		public string lowStockDate { get; set; }
		public bool? isDecimalDivided { get; set; }
		public int? stockStatusChangedAuto { get; set; }
		public ExtensionAttributes extensionAttributes { get; set; }
	}

	public class RootObject
	{
		public StockItem stockItem { get; set; }
	}
}