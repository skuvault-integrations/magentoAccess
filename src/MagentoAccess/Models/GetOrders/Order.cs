namespace MagentoAccess.Models.GetOrders
{
	public class Order : Services.GetOrders.Order
	{
		public Order( Services.GetOrders.Order order )
			: base( order )
		{
		}
	}
}