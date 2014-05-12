using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;

namespace MagentoAccess.Services.Parsers
{
	public class MagentoProductsResponseParser : MagentoBaseResponseParser< GetProductsResponse >
	{
		protected override GetProductsResponse ParseWithWxceptionHanding(XElement root)
		{
			var ns = "";

			var dataItemsNodes = root.Nodes();

			var orderDataItems = dataItemsNodes.Select(x => XElement.Parse(x.ToString())).ToList();

			var products = orderDataItems.Select(x =>
			{
				var resultProduct = new Product
				{
					EntityId = GetElementValue(x, ns, "entity_id"),
					Sku = GetElementValue(x, ns, "sku"),
					Price = GetElementValue(x, ns, "price").ToDecimalDotOrComaSeparated(),
					Name = GetElementValue(x, ns, "name"),
					Description = GetElementValue(x, ns, "description")
				};

				return resultProduct;
			}).ToList();

			return new GetProductsResponse { Products = products };
		}
	}
}