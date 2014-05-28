using System;
using System.Collections.Generic;
using MagentoAccess.Misc;

namespace MagentoAccess.Models.Services.GetOrders
{
	[ Serializable ]
	public class Order
	{
		public Order()
		{
		}

		public Order( Order order )
		{
			var t = order.DeepClone();

			this.OrderId = t.OrderId;
			this.Status = t.Status;
			this.Customer = t.Customer;
			this.BaseDiscount = t.BaseDiscount;
			this.BaseGrandTotal = t.BaseGrandTotal;
			this.BaseShippingAmount = t.BaseShippingAmount;
			this.BaseShippingTaxAmount = t.BaseShippingTaxAmount;
			this.BaseSubtotal = t.BaseSubtotal;
			this.BaseTaxAmount = t.BaseTaxAmount;
			this.BaseTotalPaid = t.BaseTotalPaid;
			this.BaseTotalRefunded = t.BaseTotalRefunded;
			this.DiscountAmount = t.DiscountAmount;
			this.GrandTotal = t.GrandTotal;
			this.ShippingAmount = t.ShippingAmount;
			this.ShippingTaxAmount = t.ShippingTaxAmount;
			this.StoreToOrderRate = t.StoreToOrderRate;
			this.Subtotal = t.Subtotal;
			this.TaxAmount = t.TaxAmount;
			this.TotalPaid = t.TotalPaid;
			this.TotalRefunded = t.TotalRefunded;
			this.BaseShippingDiscountAmount = t.BaseShippingDiscountAmount;
			this.BaseSubtotalInclTax = t.BaseSubtotalInclTax;
			this.BaseTotalDue = t.BaseTotalDue;
			this.ShippingDiscountAmount = t.ShippingDiscountAmount;
			this.SubtotalInclTax = t.SubtotalInclTax;
			this.TotalDue = t.TotalDue;
			this.BaseCurrencyCode = t.BaseCurrencyCode;
			this.StoreName = t.StoreName;
			this.CreatedAt = t.CreatedAt;
			this.ShippingInclTax = t.ShippingInclTax;
			this.PaymentMethod = t.PaymentMethod;
			this.Addresses = t.Addresses;
			this.Items = t.Items;
			this.Comments = t.Comments;
		}

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
		public IEnumerable <Address> Addresses { get; set; }
		public IEnumerable< Item > Items { get; set; }
		public IEnumerable< Comment > Comments { get; set; }
	}

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