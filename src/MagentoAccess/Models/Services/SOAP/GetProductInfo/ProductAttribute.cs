namespace MagentoAccess.Models.Services.Soap.GetProductInfo
{
	internal class ProductAttribute
	{
		public string Key { get; set; }
		public string Value { get; set; }

		public ProductAttribute( string key, string value )
		{
			Key = key;
			Value = value;
		}
	}
}