using System;

namespace MagentoAccess.Models.Services.Rest.v1x.GetOrders
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