using System.Linq;
using MagentoAccess.Models.Services.GetOrders;

namespace MagentoAccess.Models.GetOrders
{
	public class Order : Services.GetOrders.Order
	{
		public Order( Services.GetOrders.Order order )
			: base( order )
		{
		}
	}

	public static class OrderExtensions
	{
		public static bool IsShipped( this Order order )
		{
			return order.Items.ToList().TrueForAll( x => x.IsShipped() );
		}
	}
}