using System;

namespace MagentoAccess.Models.Services.GetOrders
{
	[ Serializable ]
	public class Comment
	{
		public string IsCustomerNotified { get; set; }
		public string IsVisibleOnFront { get; set; }
		public string CommentText { get; set; }
		public string Status { get; set; }
	}
}