using System.Linq;
using System.Xml.Linq;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.GetProducts;

namespace MagentoAccess.Services.Parsers
{
	internal class MagentoProductsResponseParser : MagentoBaseResponseParser< GetProductsResponse >
	{
		protected override GetProductsResponse ParseWithoutExceptionHanding( XElement root )
		{
			var ns = "";

			var dataItemsNodes = root.Nodes();

			var orderDataItems = dataItemsNodes.Select( x => XElement.Parse( x.ToString() ) ).ToList();

			var products = orderDataItems.Select( x =>
			{
				var resultProduct = new Product
				{
					EntityId = GetElementValue( x, ns, "entity_id" ),
					Sku = GetElementValue( x, ns, "sku" ),
					Price = GetElementValue( x, ns, "price" ).ToDecimalOrDefault(),
					Name = GetElementValue( x, ns, "name" ),
					Description = GetElementValue( x, ns, "description" )
				};

				return resultProduct;
			} ).ToList();

			return new GetProductsResponse { Products = products };
		}
	}
}