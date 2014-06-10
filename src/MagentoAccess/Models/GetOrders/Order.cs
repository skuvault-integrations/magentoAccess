using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.GetOrders;

namespace MagentoAccess.Models.GetOrders
{
	public class Order : Services.GetOrders.Order
	{
		public Order( Services.GetOrders.Order order )
			: base( order )
		{
		}

		public OrderStateEnum State { get; set; }

		public string OrderIncrementalId { get; set; }

		public Order( salesOrderEntity order )
		{
			this.Addresses = new List< Address >
			{
				new Address
				{
					AddressType = order.billing_address.address_type,
					City = order.billing_address.city,
					Company = order.billing_address.company,
					CountryId = order.billing_address.country_id,
					Firstname = order.billing_address.firstname,
					Lastname = order.billing_address.lastname,
					Postcode = order.billing_address.postcode,
					Region = order.billing_address.region,
					Street = order.billing_address.street,
					Telephone = order.billing_address.telephone,
				}
			};

			this.BaseCurrencyCode = order.base_currency_code;
			this.BaseDiscount = order.base_discount_amount.ToDecimalOrDefault();
			this.BaseGrandTotal = order.base_grand_total.ToDecimalOrDefault();
			this.BaseShippingAmount = order.base_shipping_amount.ToDecimalOrDefault();
			this.BaseSubtotal = order.base_subtotal.ToDecimalOrDefault();
			this.BaseTaxAmount = order.base_tax_amount.ToDecimalOrDefault();
			this.BaseTotalPaid = order.base_total_paid.ToDecimalOrDefault();
			this.BaseTotalRefunded = order.base_total_refunded.ToDecimalOrDefault();
			this.CreatedAt = order.created_at.ToDateTimeOrDefault();
			this.Customer = order.customer_id;
			this.DiscountAmount = order.discount_amount.ToDecimalOrDefault();
			this.GrandTotal = order.grand_total.ToDecimalOrDefault();
			this.OrderIncrementalId = order.order_id;
			this.OrderId = order.increment_id;
			this.Items = order.items.Select( x => new Item
			{
				BaseDiscountAmount = x.base_discount_amount.ToDecimalOrDefault(),
				BaseOriginalPrice = x.base_original_price.ToDecimalOrDefault(),
				Sku = x.sku,
				Name = x.name,
				BaseTaxAmount = x.base_tax_amount.ToDecimalOrDefault(),
				ItemId = x.item_id,
				BasePrice = x.base_price.ToDecimalOrDefault(),
				BaseRowTotal = x.base_row_total.ToDecimalOrDefault(),
				DscountAmount = x.discount_amount.ToDecimalOrDefault(),
				OriginalPrice = x.original_price.ToDecimalOrDefault(),
				Price = x.price.ToDecimalOrDefault(),
				QtyCanceled = x.qty_canceled.ToDecimalOrDefault(),
				QtyInvoiced = x.qty_invoiced.ToDecimalOrDefault(),
				QtyOrdered = x.qty_ordered.ToDecimalOrDefault(),
				QtyShipped = x.qty_shipped.ToDecimalOrDefault(),
				QtyRefunded = x.qty_refunded.ToDecimalOrDefault(),
				RowTotal = x.row_total.ToDecimalOrDefault(),
				TaxAmount = x.tax_amount.ToDecimalOrDefault(),
				TaxPercent = x.tax_percent.ToDecimalOrDefault(),
			}
				).ToList();
			this.PaymentMethod = order.payment.method;
			this.ShippingAmount = order.shipping_amount.ToDecimalOrDefault();
			this.StoreName = order.store_name;
			this.Subtotal = order.subtotal.ToDecimalOrDefault();
			this.TaxAmount = order.tax_amount.ToDecimalOrDefault();
			this.TotalPaid = order.total_paid.ToDecimalOrDefault();
			this.TotalRefunded = order.total_refunded.ToDecimalOrDefault();

			OrderStatusesEnum tempstatus;
			OrderStateEnum tempstate;
			this.Status = Enum.TryParse( order.status, true, out tempstatus ) ? tempstatus : OrderStatusesEnum.unknown;
			this.State = Enum.TryParse( order.state, true, out tempstate ) ? tempstate : OrderStateEnum.unknown;
		}

	}

	public enum OrderStateEnum
	{
		unknown,
		@new,
		pending_payment,
		processing,
		complete,
		closed,
		canceled,
		holded
	}

	public class OrderId
	{
		public OrderId( string id, bool isIncremental )
		{
			this.Id = id;
			this.IsIncrementalId = isIncremental;
		}

		public string Id { get; private set; }

		public bool IsIncrementalId { get; private set; }
	}

	public static class OrderExtensions
	{
		public static bool IsShipped( this Order order )
		{
			return order.Items.ToList().TrueForAll( x => x.IsShipped() );
		}

		public static OrderId GetId( this Order order )
		{
			return !string.IsNullOrWhiteSpace( order.OrderIncrementalId ) ? new OrderId( order.OrderIncrementalId, true ) : new OrderId( order.OrderId, false );
		}
	}
}