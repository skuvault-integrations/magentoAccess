using System.Collections.Generic;
using Newtonsoft.Json;

namespace MagentoAccess.Models.Services.Rest.v2x.Products
{
	public class ExtensionAttributes2
	{
	}

	public class ProductLink
	{
		public string id { get; set; }
		public string sku { get; set; }
		public int optionId { get; set; }
		public int qty { get; set; }
		public int position { get; set; }
		public bool isDefault { get; set; }
		public int price { get; set; }
		public int priceType { get; set; }
		public int canChangeQuantity { get; set; }
		public ExtensionAttributes2 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes3
	{
	}

	public class BundleProductOption
	{
		public int optionId { get; set; }
		public string title { get; set; }
		public bool required { get; set; }
		public string type { get; set; }
		public int position { get; set; }
		public string sku { get; set; }
		public List<ProductLink> productLinks { get; set; }
		public ExtensionAttributes3 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes4
	{
	}

	public class LinkFileContent
	{
		public string fileData { get; set; }
		public string name { get; set; }
		public ExtensionAttributes4 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes5
	{
	}

	public class SampleFileContent
	{
		public string fileData { get; set; }
		public string name { get; set; }
		public ExtensionAttributes5 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes6
	{
	}

	public class DownloadableProductLink
	{
		public int id { get; set; }
		public string title { get; set; }
		public int sortOrder { get; set; }
		public int isShareable { get; set; }
		public int price { get; set; }
		public int numberOfDownloads { get; set; }
		public string linkType { get; set; }
		public string linkFile { get; set; }
		public LinkFileContent linkFileContent { get; set; }
		public string linkUrl { get; set; }
		public string sampleType { get; set; }
		public string sampleFile { get; set; }
		public SampleFileContent sampleFileContent { get; set; }
		public string sampleUrl { get; set; }
		public ExtensionAttributes6 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes7
	{
	}

	public class SampleFileContent2
	{
		public string fileData { get; set; }
		public string name { get; set; }
		public ExtensionAttributes7 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes8
	{
	}

	public class DownloadableProductSample
	{
		public int id { get; set; }
		public string title { get; set; }
		public int sortOrder { get; set; }
		public string sampleType { get; set; }
		public string sampleFile { get; set; }
		public SampleFileContent2 sampleFileContent { get; set; }
		public string sampleUrl { get; set; }
		public ExtensionAttributes8 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes9
	{
	}

	public class StockItem
	{
		public int itemId { get; set; }
		public int productId { get; set; }
		public int stockId { get; set; }
		public int qty { get; set; }
		public bool isInStock { get; set; }
		public bool isQtyDecimal { get; set; }
		public bool showDefaultNotificationMessage { get; set; }
		public bool useConfigMinQty { get; set; }
		public int minQty { get; set; }
		public int useConfigMinSaleQty { get; set; }
		public int minSaleQty { get; set; }
		public bool useConfigMaxSaleQty { get; set; }
		public int maxSaleQty { get; set; }
		public bool useConfigBackorders { get; set; }
		public int backorders { get; set; }
		public bool useConfigNotifyStockQty { get; set; }
		public int notifyStockQty { get; set; }
		public bool useConfigQtyIncrements { get; set; }
		public int qtyIncrements { get; set; }
		public bool useConfigEnableQtyInc { get; set; }
		public bool enableQtyIncrements { get; set; }
		public bool useConfigManageStock { get; set; }
		public bool manageStock { get; set; }
		public string lowStockDate { get; set; }
		public bool isDecimalDivided { get; set; }
		public int stockStatusChangedAuto { get; set; }
		public ExtensionAttributes9 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes10
	{
	}

	public class Value
	{
		public int valueIndex { get; set; }
		public ExtensionAttributes10 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes11
	{
	}

	public class ConfigurableProductOption
	{
		public int id { get; set; }
		public string attributeId { get; set; }
		public string label { get; set; }
		public int position { get; set; }
		public bool isUseDefault { get; set; }
		public List<Value> values { get; set; }
		public ExtensionAttributes11 extensionAttributes { get; set; }
		public int productId { get; set; }
	}

	public class ExtensionAttributes
	{
		public List<BundleProductOption> bundleProductOptions { get; set; }
		public List<DownloadableProductLink> downloadableProductLinks { get; set; }
		public List<DownloadableProductSample> downloadableProductSamples { get; set; }
		public StockItem stockItem { get; set; }
		public List<ConfigurableProductOption> configurableProductOptions { get; set; }
		public List<int> configurableProductLinks { get; set; }
	}

	public class ExtensionAttributes12
	{
		public int qty { get; set; }
	}

	public class ProductLink2
	{
		public string sku { get; set; }
		public string linkType { get; set; }
		public string linkedProductSku { get; set; }
		public string linkedProductType { get; set; }
		public int position { get; set; }
		public ExtensionAttributes12 extensionAttributes { get; set; }
	}

	public class Value2
	{
		public string title { get; set; }
		public int sortOrder { get; set; }
		public int price { get; set; }
		public string priceType { get; set; }
		public string sku { get; set; }
		public int optionTypeId { get; set; }
	}

	public class ExtensionAttributes13
	{
	}

	public class Option
	{
		public string productSku { get; set; }
		public int optionId { get; set; }
		public string title { get; set; }
		public string type { get; set; }
		public int sortOrder { get; set; }
		public bool isRequire { get; set; }
		public int price { get; set; }
		public string priceType { get; set; }
		public string sku { get; set; }
		public string fileExtension { get; set; }
		public int maxCharacters { get; set; }
		public int imageSizeX { get; set; }
		public int imageSizeY { get; set; }
		public List<Value2> values { get; set; }
		public ExtensionAttributes13 extensionAttributes { get; set; }
	}

	public class Content
	{
		public string base64EncodedData { get; set; }
		public string type { get; set; }
		public string name { get; set; }
	}

	public class VideoContent
	{
		public string mediaType { get; set; }
		public string videoProvider { get; set; }
		public string videoUrl { get; set; }
		public string videoTitle { get; set; }
		public string videoDescription { get; set; }
		public string videoMetadata { get; set; }
	}

	public class ExtensionAttributes14
	{
		public VideoContent videoContent { get; set; }
	}

	public class MediaGalleryEntry
	{
		public int id { get; set; }
		public string mediaType { get; set; }
		public string label { get; set; }
		public int position { get; set; }
		public bool disabled { get; set; }
		public List<string> types { get; set; }
		public string file { get; set; }
		public Content content { get; set; }
		public ExtensionAttributes14 extensionAttributes { get; set; }
	}

	public class ExtensionAttributes15
	{
	}

	public class TierPrice
	{
		public int customerGroupId { get; set; }
		public int qty { get; set; }
		public int value { get; set; }
		public ExtensionAttributes15 extensionAttributes { get; set; }
	}

	public class CustomAttribute
	{
		public string attributeCode { get; set; }
		public string value { get; set; }
	}

	[JsonConverter(typeof(LaxPropertyNameMatchingConverter))]
	public class Item
	{
		public int id { get; set; }
		public string sku { get; set; }
		public string name { get; set; }
		public int attributeSetId { get; set; }
		public int price { get; set; }
		public int status { get; set; }
		public int visibility { get; set; }
		public string typeId { get; set; }
		public string createdAt { get; set; }
		public string updatedAt { get; set; }
		public int weight { get; set; }
		public ExtensionAttributes extensionAttributes { get; set; }
		public List<ProductLink2> productLinks { get; set; }
		public List<Option> options { get; set; }
		public List<MediaGalleryEntry> mediaGalleryEntries { get; set; }
		public List<TierPrice> tierPrices { get; set; }
		public List<CustomAttribute> customAttributes { get; set; }
	}

	public class RootObject
	{
		public List<Item> items { get; set; }
		public SearchCriteria searchCriteria { get; set; }
		public int totalCount { get; set; }
	}
}