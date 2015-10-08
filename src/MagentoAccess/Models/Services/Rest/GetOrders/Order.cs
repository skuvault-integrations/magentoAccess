using System;
using System.Collections.Generic;
using MagentoAccess.Misc;

namespace MagentoAccess.Models.Services.Rest.GetOrders
{
	[ Serializable ]
	public class Order
	{
		public Order()
		{
		}

		public Order( Order order )
		{
			var clone = order.DeepClone();

			this.OrderId = clone.OrderId;
			this.Status = clone.Status;
			this.Customer = clone.Customer;
			this.BaseDiscount = clone.BaseDiscount;
			this.BaseGrandTotal = clone.BaseGrandTotal;
			this.BaseShippingAmount = clone.BaseShippingAmount;
			this.BaseShippingTaxAmount = clone.BaseShippingTaxAmount;
			this.BaseSubtotal = clone.BaseSubtotal;
			this.BaseTaxAmount = clone.BaseTaxAmount;
			this.BaseTotalPaid = clone.BaseTotalPaid;
			this.BaseTotalRefunded = clone.BaseTotalRefunded;
			this.DiscountAmount = clone.DiscountAmount;
			this.GrandTotal = clone.GrandTotal;
			this.ShippingAmount = clone.ShippingAmount;
			this.ShippingTaxAmount = clone.ShippingTaxAmount;
			this.StoreToOrderRate = clone.StoreToOrderRate;
			this.Subtotal = clone.Subtotal;
			this.TaxAmount = clone.TaxAmount;
			this.TotalPaid = clone.TotalPaid;
			this.TotalRefunded = clone.TotalRefunded;
			this.BaseShippingDiscountAmount = clone.BaseShippingDiscountAmount;
			this.BaseSubtotalInclTax = clone.BaseSubtotalInclTax;
			this.BaseTotalDue = clone.BaseTotalDue;
			this.ShippingDiscountAmount = clone.ShippingDiscountAmount;
			this.SubtotalInclTax = clone.SubtotalInclTax;
			this.TotalDue = clone.TotalDue;
			this.BaseCurrencyCode = clone.BaseCurrencyCode;
			this.StoreName = clone.StoreName;
			this.CreatedAt = clone.CreatedAt;
			this.ShippingInclTax = clone.ShippingInclTax;
			this.PaymentMethod = clone.PaymentMethod;
			this.Addresses = clone.Addresses;
			this.Items = clone.Items;
			this.Comments = clone.Comments;
		}

		public string OrderId { get; set; }
		public OrderStatusesEnum Status { get; set; }
		public string Customer { get; set; }
		public double BaseDiscount { get; set; }
		public decimal BaseGrandTotal { get; set; }
		public decimal BaseShippingAmount { get; set; }
		public decimal BaseShippingTaxAmount { get; set; }
		public decimal BaseSubtotal { get; set; }
		public decimal BaseTaxAmount { get; set; }
		public decimal BaseTotalPaid { get; set; }
		public decimal BaseTotalRefunded { get; set; }
		public double DiscountAmount { get; set; }
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
		public List< Address > Addresses { get; set; }
		public List< Item > Items { get; set; }
		public List< Comment > Comments { get; set; }
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