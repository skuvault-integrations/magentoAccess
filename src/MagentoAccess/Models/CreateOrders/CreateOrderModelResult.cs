using System.Collections.Generic;

namespace MagentoAccess.Models.CreateOrders
{
	public class CreateOrderModelResult
	{
		public CreateOrderModelResult( CreateOrderModel createOrderModel )
		{
			CustomerFirstName = createOrderModel.CustomerFirstName;
			CustomerLastName = createOrderModel.CustomerLastName;
			CustomerMail = createOrderModel.CustomerMail;
			ProductIds = createOrderModel.ProductIds;
			StoreId = createOrderModel.StoreId;
		}

		public string StoreId { get; set; }

		public IEnumerable< string > ProductIds { get; set; }

		public string CustomerMail { get; set; }

		public string CustomerLastName { get; set; }

		public string CustomerFirstName { get; set; }

		public string OrderId { get; set; }
	}
}