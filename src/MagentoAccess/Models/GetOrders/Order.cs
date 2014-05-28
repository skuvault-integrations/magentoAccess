using System;
using System.Collections.Generic;

namespace MagentoAccess.Models.GetOrders
{
	public class Order
	{
		public string OrderId { get; set; }
		public OrderStatusesEnum Status { get; set; }
		public string Customer { get; set; }
		public decimal BaseDiscount { get; set; }
		public decimal BaseGrandTotal { get; set; }
		public decimal BaseShippingAmount { get; set; }
		public decimal BaseShippingTaxAmount { get; set; }
		public decimal BaseSubtotal { get; set; }
		public decimal BaseTaxAmount { get; set; }
		public decimal BaseTotalPaid { get; set; }
		public decimal BaseTotalRefunded { get; set; }
		public decimal DiscountAmount { get; set; }
		public decimal GrandTotal { get; set; }
		public decimal ShippingAmount { get; set; }
		public decimal ShippingTaxAmount { get; set; }
		public decimal StoreToOrderRate { get; set; }
		public decimal Subtotal { get; set; }
		public decimal TaxAmount { get; set; }
		public decimal TotalPaid { get; set; }
		public decimal TotalRefunded { get; set; }
		public decimal BaseShippingDiscountAmount { get; set; }
		public decimal BaseSubtotalInclTax { get; set; }
		public decimal BaseTotalDue { get; set; }
		public decimal ShippingDiscountAmount { get; set; }
		public decimal SubtotalInclTax { get; set; }
		public decimal TotalDue { get; set; }
		public string BaseCurrencyCode { get; set; }
		public string StoreName { get; set; }
		public DateTime CreatedAt { get; set; }
		public decimal ShippingInclTax { get; set; }
		public string PaymentMethod { get; set; }
		public IEnumerable< Address > Addresses { get; set; }
		public IEnumerable< Item > Items { get; set; }
		public IEnumerable< Comment > Comments { get; set; }
	}

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