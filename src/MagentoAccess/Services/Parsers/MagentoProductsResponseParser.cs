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
		public override GetProductsResponse Parse(Stream stream, bool keepStremPosition = true)
		{
			try
			{
				//todo: reuse
				XNamespace ns = "";

				var streamStartPos = stream.Position;

				var root = XElement.Load(stream);

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

				if (keepStremPosition)
					stream.Position = streamStartPos;

				return new GetProductsResponse { Products = products };
			}
			catch (Exception ex)
			{
				//todo: reuse
				var buffer = new byte[stream.Length];
				stream.Read(buffer, 0, (int)stream.Length);
				var utf8Encoding = new UTF8Encoding();
				var bufferStr = utf8Encoding.GetString(buffer);
				throw new Exception("Can't parse: " + bufferStr, ex);
			}
		}
	}
}