using System.Collections.Generic;

namespace MagentoAccess.Models.CreateOrders
{
	public class CreateOrderModel
	{
		//"0"
		public string StoreId { get; set; }
		// "max",
		public string CustomerFirstName { get; set; }
		// "qwe@qwe.com",
		public string CustomerMail { get; set; }
		//  "kits"
		public string CustomerLastName { get; set; }
		public IEnumerable<string> ProductIds { get; set; }
	}
}