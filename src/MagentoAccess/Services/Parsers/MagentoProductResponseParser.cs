using System.Xml.Linq;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.GetProduct;

namespace MagentoAccess.Services.Parsers
{
	internal class MagentoProductResponseParser : MagentoBaseResponseParser<GetProductResponse>
	{
		protected override GetProductResponse ParseWithoutExceptionHanding( XElement root )
		{
			var ns = "";

			var resultProduct = new GetProductResponse();

			resultProduct.EntityId = GetElementValue( root, ns, "entity_id" );
			resultProduct.Sku = GetElementValue( root, ns, "sku" );
			resultProduct.Name = GetElementValue( root, ns, "name" );
			resultProduct.Price = GetElementValue( root, ns, "price" );
			resultProduct.Description = GetElementValue( root, ns, "description" );

			var stockData = root.Element( ns + "stock_data" );
			if( stockData != null )
			{
				resultProduct.StockData = new StockData();

				resultProduct.StockData.Qty = GetElementValue( stockData, ns, "qty" ).ToDecimalOrDefault();
				resultProduct.StockData.MinQty = GetElementValue( stockData, ns, "min_qty" ).ToDecimalOrDefault();
				resultProduct.StockData.MinSaleQty = GetElementValue( stockData, ns, "min_sale_qty" ).ToDecimalOrDefault();
				resultProduct.StockData.MaxSaleQty = GetElementValue( stockData, ns, "max_sale_qty" ).ToDecimalOrDefault();
				resultProduct.StockData.IsInStock = GetElementValue( stockData, ns, "is_in_stock" ).ToDecimalOrDefault();
			}

			return resultProduct;
		}
	}
}