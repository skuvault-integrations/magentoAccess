using System;

namespace MagentoAccess.Models.GetOrders
{
	[ Serializable ]
	public enum OrderStatusesEnum
	{
		unknown,
		canceled,
		closed,
		complete,
		fraud,
		holded,
		payment_review,
		paypal_canceled_reversal,
		paypal_reversed,
		pending,
		pending_payment,
		pending_paypal,
		processing,
	}
}