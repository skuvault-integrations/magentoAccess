namespace MagentoAccess.Models.Services.Soap.GetProductAttributeInfo
{
	internal class ProductAttributeInfo
	{
		public string Label { get; set; }
		public string Value { get; set; }

		public ProductAttributeInfo( string label, string value )
		{
			Label = label;
			Value = value;
		}
	}
}