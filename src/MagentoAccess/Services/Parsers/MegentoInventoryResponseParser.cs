using System.Linq;
using System.Xml.Linq;
using MagentoAccess.Models.GetInventory;

namespace MagentoAccess.Services.Parsers
{
	public class MegentoInventoryResponseParser : MagentoBaseResponseParser< GetInventoryResponse >
	{
		protected override GetInventoryResponse ParseWithWxceptionHanding( XElement root )
		{
			XNamespace ns = "";

			var dataItemsNodes = root.Nodes();

			var inventoryItems = dataItemsNodes.Select( x => XElement.Parse( x.ToString() ) ).ToList();

			var orders = inventoryItems.Select( x =>
			{
				var resultOrder = new InventoryItem
				{
					ItemId = GetElementValue( x, ns, "item_id" ),
					Qty = GetElementValue( x, ns, "qty" ),
					BackOrders = GetElementValue( x, ns, "backorders" )
				};

				return resultOrder;
			} ).ToList();

			return new GetInventoryResponse { Items = orders };
		}
	}
}