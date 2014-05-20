using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MagentoAccess.Models.PutStockItems;

namespace MagentoAccess.Services.Parsers
{
	public class MegentoPutInventoryResponseParser : MagentoBaseResponseParser< PutStockItemsResponse >
	{
		protected override PutStockItemsResponse ParseWithoutExceptionHanding( XElement root )
		{
			XNamespace ns = "";

			var successElement = root.Element( ns + "success" );

			List< StockItem > items = null;

			if( successElement != null )
			{
				var successItemsNodes = successElement.Nodes();

				var successItems = successItemsNodes.Select( x => XElement.Parse( x.ToString() ) ).ToList();

				items = successItems.Select( x =>
				{
					var resultOrder = new StockItem
					{
						Message = GetElementValue( x, ns, "message" ),
						Code = GetElementValue( x, ns, "code" ),
						ItemId = GetElementValue( x, ns, "item_id" )
					};

					return resultOrder;
				} ).ToList();
			}

			return new PutStockItemsResponse { Items = items };
		}
	}
}