using System;

namespace MagentoAccess.Models.GetOrders
{
	[ Serializable ]
	public enum OrderStatusesEnum
	{
		unknown = 0,
		canceled = 1,
		closed = 2,
		complete = 3,
		fraud = 4,
		holded = 5,
		payment_review = 6,
		paypal_canceled_reversal = 7,
		paypal_reversed = 8,
		pending = 9,
		pending_payment = 10,
		pending_paypal = 11,
		processing = 12,
	}
}