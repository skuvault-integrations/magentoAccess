using System.Xml.Linq;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProduct;

namespace MagentoAccess.Services.Parsers
{
	public class MagentoProductResponseParser : MagentoBaseResponseParser< GetProductResponse >
	{
		protected override GetProductResponse ParseWithWxceptionHanding( XElement root )
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

				resultProduct.StockData.Qty = GetElementValue( stockData, ns, "qty" ).ToDecimalDotOrComaSeparated();
				resultProduct.StockData.MinQty = GetElementValue( stockData, ns, "min_qty" ).ToDecimalDotOrComaSeparated();
				resultProduct.StockData.MinSaleQty = GetElementValue( stockData, ns, "min_sale_qty" ).ToDecimalDotOrComaSeparated();
				resultProduct.StockData.MaxSaleQty = GetElementValue( stockData, ns, "max_sale_qty" ).ToDecimalDotOrComaSeparated();
				resultProduct.StockData.IsInStock = GetElementValue( stockData, ns, "is_in_stock" ).ToDecimalDotOrComaSeparated();
			}

			return resultProduct;
		}
	}
}