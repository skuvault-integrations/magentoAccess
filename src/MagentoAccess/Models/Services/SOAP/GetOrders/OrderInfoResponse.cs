using System.Collections.Generic;
using System.Linq;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.SOAP.GetOrders
{
	internal class OrderInfoResponse
	{
		public OrderInfoResponse( salesOrderInfoResponse res )
		{
			AppliedRuleIds = res.result.applied_rule_ids;
			BaseCurrencyCode = res.result.base_currency_code;
			BaseDiscountAmount = res.result.base_discount_amount;
			BaseGrandTotal = res.result.base_grand_total;
			BaseShippingAmount = res.result.base_shipping_amount;
			BaseSubtotal = res.result.base_subtotal;
			BaseTaxAmount = res.result.base_tax_amount;
			BaseToGlobalRate = res.result.base_to_global_rate;
			BaseToOrderRate = res.result.base_to_order_rate;
			BaseTotalCanceled = res.result.base_total_canceled;
			BaseTotalInvoiced = res.result.base_total_invoiced;
			BaseTotalOfflineRefunded = res.result.base_total_offline_refunded;
			BaseTotalOnlineRefunded = res.result.base_total_online_refunded;
			BaseTotalPaid = res.result.base_total_paid;
			BaseTotalQtyOrdered = res.result.base_total_qty_ordered;
			BaseTotalRefunded = res.result.base_total_refunded;
			if( res.result.billing_address != null )
			{
				var billingAddress = new BillingAddress
				{
					AddressId = res.result.billing_address.address_id,
					AddressType = res.result.billing_address.address_type,
					City = res.result.billing_address.city,
					Company = res.result.billing_address.company,
					CountryId = res.result.billing_address.country_id,
					CreatedAt = res.result.billing_address.created_at,
					Fax = res.result.billing_address.fax,
					Firstname = res.result.billing_address.firstname,
					IncrementId = res.result.billing_address.increment_id,
					IsActive = res.result.billing_address.is_active,
					Lastname = res.result.billing_address.lastname,
					ParentId = res.result.billing_address.parent_id,
					Postcode = res.result.billing_address.postcode,
					Region = res.result.billing_address.region,
					RegionId = res.result.billing_address.region_id,
					Street = res.result.billing_address.street,
					Telephone = res.result.billing_address.telephone,
					UpdatedAt = res.result.billing_address.updated_at,
				};
				this.BillingAddress = billingAddress;
			}
			BillingAddressId = res.result.billing_address_id;
			BillingFirstname = res.result.billing_firstname;
			BillingLastname = res.result.billing_lastname;
			BillingName = res.result.billing_name;
			CreatedAt = res.result.created_at;
			CustomerEmail = res.result.customer_email;
			CustomerFirstname = res.result.customer_firstname;
			CustomerGroupId = res.result.customer_group_id;
			CustomerId = res.result.customer_id;
			CustomerIsGuest = res.result.customer_is_guest;
			CustomerLastname = res.result.customer_lastname;
			CustomerNoteNotify = res.result.customer_note_notify;
			DiscountAmount = res.result.discount_amount;
			EmailSent = res.result.email_sent;
			GiftMessage = res.result.gift_message;
			GiftMessageId = res.result.gift_message_id;
			GlobalCurrencyCode = res.result.global_currency_code;
			GrandTotal = res.result.grand_total;
			IncrementId = res.result.increment_id;
			IsActive = res.result.is_active;
			IsVirtual = res.result.is_virtual;

			if( res.result.items != null )
				Items = res.result.items.Select( x => new OrderItemEntity( x ) );

			OrderCurrencyCode = res.result.order_currency_code;
			OrderId = res.result.order_id;
			ParentId = res.result.parent_id;

			if( res.result.payment != null )
			{
				var payment = new Payment()
				{
					AmountOrdered = res.result.payment.amount_ordered,
					BaseAmountOrdered = res.result.payment.base_amount_ordered,
					BaseShippingAmount = res.result.payment.base_shipping_amount,
					CcExpMonth = res.result.payment.cc_exp_month,
					CcExpYear = res.result.payment.cc_exp_year,
					CcLast4 = res.result.payment.cc_last4,
					CcNumberEnc = res.result.payment.cc_number_enc,
					CcOwner = res.result.payment.cc_owner,
					CcSsStartMonth = res.result.payment.cc_ss_start_month,
					CcSsStartYear = res.result.payment.cc_ss_start_year,
					CcType = res.result.payment.cc_type,
					CreatedAt = res.result.payment.created_at,
					IncrementId = res.result.payment.increment_id,
					IsActive = res.result.payment.is_active,
					Method = res.result.payment.method,
					ParentId = res.result.payment.parent_id,
					PaymentId = res.result.payment.payment_id,
					PoNumber = res.result.payment.po_number,
					ShippingAmount = res.result.payment.shipping_amount,
					UpdatedAt = res.result.payment.updated_at,
				};
				Payment = payment;
			}
			QuoteId = res.result.quote_id;
			RemoteIp = res.result.remote_ip;
			if( res.result.shipping_address != null )
			{
				ShippingAddress = new ShippingAddress()
				{
					AddressId = res.result.shipping_address.address_id,
					AddressType = res.result.shipping_address.address_type,
					City = res.result.shipping_address.city,
					Company = res.result.shipping_address.company,
					CountryId = res.result.shipping_address.country_id,
					CreatedAt = res.result.shipping_address.created_at,
					Fax = res.result.shipping_address.fax,
					Firstname = res.result.shipping_address.firstname,
					IncrementId = res.result.shipping_address.increment_id,
					IsActive = res.result.shipping_address.is_active,
					Lastname = res.result.shipping_address.lastname,
					ParentId = res.result.shipping_address.parent_id,
					Postcode = res.result.shipping_address.postcode,
					Region = res.result.shipping_address.region,
					RegionId = res.result.shipping_address.region_id,
					Street = res.result.shipping_address.street,
					Telephone = res.result.shipping_address.telephone,
					UpdatedAt = res.result.shipping_address.updated_at,
				};
			}
			ShippingAddressId = res.result.shipping_address_id;
			ShippingAmount = res.result.shipping_amount;
			ShippingDescription = res.result.shipping_description;
			ShippingFirstname = res.result.shipping_firstname;
			ShippingLastname = res.result.shipping_lastname;
			ShippingMethod = res.result.shipping_method;
			ShippingName = res.result.shipping_name;
			State = res.result.state;
			Status = res.result.status;

			if( res.result.status_history != null )
			{
				StatusHistory = new List< StatusHistoryRecord >(
					res.result.status_history.Select( x => new StatusHistoryRecord( x ) ) );
			}
			StoreCurrencyCode = res.result.store_currency_code;
			StoreId = res.result.store_id;
			StoreName = res.result.store_name;
			StoreToBaseRate = res.result.store_to_base_rate;
			StoreToOrderRate = res.result.store_to_order_rate;
			Subtotal = res.result.subtotal;
			TaxAmount = res.result.tax_amount;
			TotalCanceled = res.result.total_canceled;
			TotalInvoiced = res.result.total_invoiced;
			TotalOfflineRefunded = res.result.total_offline_refunded;
			TotalOnlineRefunded = res.result.total_online_refunded;
			TotalPaid = res.result.total_paid;
			TotalQtyOrdered = res.result.total_qty_ordered;
			TotalRefunded = res.result.total_refunded;
			UpdatedAT = res.result.updated_at;
			Weight = res.result.weight;
		}

		public string AppliedRuleIds { get; private set; }
		public string BaseCurrencyCode { get; private set; }
		public string BaseDiscountAmount { get; private set; }
		public string BaseGrandTotal { get; private set; }
		public string BaseShippingAmount { get; private set; }
		public string BaseSubtotal { get; private set; }
		public string BaseTaxAmount { get; private set; }
		public string BaseToGlobalRate { get; private set; }
		public string BaseToOrderRate { get; private set; }
		public string BaseTotalCanceled { get; private set; }
		public string BaseTotalInvoiced { get; private set; }
		public string BaseTotalOfflineRefunded { get; private set; }
		public string BaseTotalOnlineRefunded { get; private set; }
		public string BaseTotalPaid { get; private set; }
		public string BaseTotalQtyOrdered { get; private set; }
		public string BaseTotalRefunded { get; private set; }
		public BillingAddress BillingAddress { get; private set; }
		public string BillingAddressId { get; private set; }
		public string BillingFirstname { get; private set; }
		public string BillingLastname { get; private set; }
		public string BillingName { get; private set; }
		public string CreatedAt { get; private set; }
		public string CustomerEmail { get; private set; }
		public string CustomerFirstname { get; private set; }
		public string CustomerGroupId { get; private set; }
		public string CustomerId { get; private set; }
		public string CustomerIsGuest { get; private set; }
		public string CustomerLastname { get; private set; }
		public string CustomerNoteNotify { get; private set; }
		public string DiscountAmount { get; private set; }
		public string EmailSent { get; private set; }
		public string GiftMessage { get; private set; }
		public string GiftMessageId { get; private set; }
		public string GlobalCurrencyCode { get; private set; }
		public string GrandTotal { get; private set; }
		public string IncrementId { get; private set; }
		public string IsActive { get; private set; }
		public string IsVirtual { get; private set; }
		public IEnumerable< OrderItemEntity > Items { get; private set; }
		public string OrderCurrencyCode { get; private set; }
		public string OrderId { get; private set; }
		public string ParentId { get; private set; }
		public Payment Payment { get; private set; }
		public string QuoteId { get; private set; }
		public string RemoteIp { get; private set; }
		public ShippingAddress ShippingAddress { get; private set; }
		public string ShippingAddressId { get; private set; }
		public string ShippingAmount { get; private set; }
		public string ShippingDescription { get; private set; }
		public string ShippingFirstname { get; private set; }
		public string ShippingLastname { get; private set; }
		public string ShippingMethod { get; private set; }
		public string ShippingName { get; private set; }
		public string State { get; private set; }
		public string Status { get; private set; }
		public List< StatusHistoryRecord > StatusHistory { get; private set; }
		public string StoreCurrencyCode { get; private set; }
		public string StoreId { get; private set; }
		public string StoreName { get; private set; }
		public string StoreToBaseRate { get; private set; }
		public string StoreToOrderRate { get; private set; }
		public string Subtotal { get; private set; }
		public string TaxAmount { get; private set; }
		public string TotalCanceled { get; private set; }
		public string TotalInvoiced { get; private set; }
		public string TotalOfflineRefunded { get; private set; }
		public string TotalOnlineRefunded { get; private set; }
		public string TotalPaid { get; private set; }
		public string TotalQtyOrdered { get; private set; }
		public string TotalRefunded { get; private set; }
		public string UpdatedAT { get; private set; }
		public string Weight { get; private set; }
	}

	internal class OrderItemEntity
	{
		public OrderItemEntity( salesOrderItemEntity salesOrderItemEntity )
		{
			AmountRefunded = salesOrderItemEntity.amount_refunded;
			AppliedRuleIds = salesOrderItemEntity.applied_rule_ids;
			BaseAmountRefunded = salesOrderItemEntity.base_amount_refunded;
			BaseDiscountAmount = salesOrderItemEntity.base_discount_amount;
			BaseDiscountInvoiced = salesOrderItemEntity.base_discount_invoiced;
			BaseOriginalPrice = salesOrderItemEntity.base_original_price;
			BasePrice = salesOrderItemEntity.base_price;
			BaseRowInvoiced = salesOrderItemEntity.base_row_invoiced;
			BaseRowTotal = salesOrderItemEntity.base_row_total;
			BaseTaxAmount = salesOrderItemEntity.base_tax_amount;
			BaseTaxBeforeDiscount = salesOrderItemEntity.base_tax_before_discount;
			BaseTaxInvoiced = salesOrderItemEntity.base_tax_invoiced;
			BaseWeeeTaxAppliedAmount = salesOrderItemEntity.base_weee_tax_applied_amount;
			BaseWeeeTaxAppliedRowAmount = salesOrderItemEntity.base_weee_tax_applied_row_amount;
			BaseWeeeTaxDisposition = salesOrderItemEntity.base_weee_tax_disposition;
			BaseWeeeTaxRowDisposition = salesOrderItemEntity.base_weee_tax_row_disposition;
			Cost = salesOrderItemEntity.cost;
			CreatedAt = salesOrderItemEntity.created_at;
			DiscountAmount = salesOrderItemEntity.discount_amount;
			DiscountInvoiced = salesOrderItemEntity.discount_invoiced;
			DiscountPercent = salesOrderItemEntity.discount_percent;
			FreeShipping = salesOrderItemEntity.free_shipping;
			GiftMessage = salesOrderItemEntity.gift_message;
			GiftMessageAvailable = salesOrderItemEntity.gift_message_available;
			GiftMessageId = salesOrderItemEntity.gift_message_id;
			IsQtyDecimal = salesOrderItemEntity.is_qty_decimal;
			IsVirtual = salesOrderItemEntity.is_virtual;
			ItemId = salesOrderItemEntity.item_id;
			Name = salesOrderItemEntity.name;
			NoDiscount = salesOrderItemEntity.no_discount;
			this.OrderId = salesOrderItemEntity.order_id;
			OriginalPrice = salesOrderItemEntity.original_price;
			Price = salesOrderItemEntity.price;
			ProductId = salesOrderItemEntity.product_id;
			ProductOptions = salesOrderItemEntity.product_options;
			ProductType = salesOrderItemEntity.product_type;
			QtyCanceled = salesOrderItemEntity.qty_canceled;
			QtyInvoiced = salesOrderItemEntity.qty_invoiced;
			QtyOrdered = salesOrderItemEntity.qty_ordered;
			QtyRefunded = salesOrderItemEntity.qty_refunded;
			QtyShipped = salesOrderItemEntity.qty_shipped;
			QuoteItemId = salesOrderItemEntity.quote_item_id;
			RowInvoiced = salesOrderItemEntity.row_invoiced;
			RowTotal = salesOrderItemEntity.row_total;
			RowWeight = salesOrderItemEntity.row_weight;
			Sku = salesOrderItemEntity.sku;
			TaxAmount = salesOrderItemEntity.tax_amount;
			TaxBeforeDiscount = salesOrderItemEntity.tax_before_discount;
			TaxInvoiced = salesOrderItemEntity.tax_invoiced;
			TaxPercent = salesOrderItemEntity.tax_percent;
			UpdatedAt = salesOrderItemEntity.updated_at;
			WeeeTaxApplied = salesOrderItemEntity.weee_tax_applied;
			WeeeTaxAppliedAmount = salesOrderItemEntity.weee_tax_applied_amount;
			WeeeTaxAppliedRowAmount = salesOrderItemEntity.weee_tax_applied_row_amount;
			WeeeTaxDisposition = salesOrderItemEntity.weee_tax_disposition;
			WeeeTaxRowDisposition = salesOrderItemEntity.weee_tax_row_disposition;
			Weight = salesOrderItemEntity.weight;
		}

		public string OrderId { get; set; }

		public string Weight { get; set; }

		public string WeeeTaxRowDisposition { get; set; }

		public string WeeeTaxDisposition { get; set; }

		public string WeeeTaxAppliedRowAmount { get; set; }

		public string WeeeTaxAppliedAmount { get; set; }

		public string WeeeTaxApplied { get; set; }

		public string UpdatedAt { get; set; }

		public string TaxPercent { get; set; }

		public string TaxInvoiced { get; set; }

		public string TaxBeforeDiscount { get; set; }

		public string TaxAmount { get; set; }

		public string Sku { get; set; }

		public string RowWeight { get; set; }

		public string RowTotal { get; set; }

		public string RowInvoiced { get; set; }

		public string QuoteItemId { get; set; }

		public string QtyShipped { get; set; }

		public string QtyRefunded { get; set; }

		public string QtyOrdered { get; set; }

		public string QtyInvoiced { get; set; }

		public string QtyCanceled { get; set; }

		public string ProductType { get; set; }

		public string ProductOptions { get; set; }

		public string ProductId { get; set; }

		public string Price { get; set; }

		public string OriginalPrice { get; set; }

		public string NoDiscount { get; set; }

		public string Name { get; set; }

		public string ItemId { get; set; }

		public string IsVirtual { get; set; }

		public string IsQtyDecimal { get; set; }

		public string GiftMessageId { get; set; }

		public string GiftMessageAvailable { get; set; }

		public string GiftMessage { get; set; }

		public string FreeShipping { get; set; }

		public string DiscountPercent { get; set; }

		public string DiscountInvoiced { get; set; }

		public string DiscountAmount { get; set; }

		public string CreatedAt { get; set; }

		public string Cost { get; set; }

		public string BaseWeeeTaxRowDisposition { get; set; }

		public string BaseWeeeTaxDisposition { get; set; }

		public string BaseWeeeTaxAppliedRowAmount { get; set; }

		public string BaseWeeeTaxAppliedAmount { get; set; }

		public string BaseTaxInvoiced { get; set; }

		public string BaseTaxBeforeDiscount { get; set; }

		public string BaseTaxAmount { get; set; }

		public string BaseRowTotal { get; set; }

		public string BaseRowInvoiced { get; set; }

		public string BasePrice { get; set; }

		public string BaseOriginalPrice { get; set; }

		public string BaseDiscountInvoiced { get; set; }

		public string BaseDiscountAmount { get; set; }

		public string BaseAmountRefunded { get; set; }

		public string AppliedRuleIds { get; set; }

		public string AmountRefunded { get; set; }
	}

	internal class StatusHistoryRecord
	{
		public string Comment { get; set; }
		public string CreatedAt { get; set; }
		public string IncrementId { get; set; }
		public string IsActive { get; set; }
		public string IsCustomerNotified { get; set; }
		public string ParentId { get; set; }
		public string Status { get; set; }
		public string UpdatedAT { get; set; }

		public StatusHistoryRecord( salesOrderStatusHistoryEntity salesOrderStatusHistoryEntity )
		{
			Comment = salesOrderStatusHistoryEntity.comment;
			CreatedAt = salesOrderStatusHistoryEntity.created_at;
			IncrementId = salesOrderStatusHistoryEntity.increment_id;
			IsActive = salesOrderStatusHistoryEntity.is_active;
			IsCustomerNotified = salesOrderStatusHistoryEntity.is_customer_notified;
			ParentId = salesOrderStatusHistoryEntity.parent_id;
			Status = salesOrderStatusHistoryEntity.status;
			UpdatedAT = salesOrderStatusHistoryEntity.updated_at;
		}

		public object qq { get; set; }
	}

	internal class ShippingAddress
	{
		public string AddressId { get; set; }
		public string AddressType { get; set; }
		public string City { get; set; }
		public string Company { get; set; }
		public string CountryId { get; set; }
		public string CreatedAt { get; set; }
		public string Fax { get; set; }
		public string Firstname { get; set; }
		public string IncrementId { get; set; }
		public string IsActive { get; set; }
		public string Lastname { get; set; }
		public string ParentId { get; set; }
		public string Postcode { get; set; }
		public string Region { get; set; }
		public string RegionId { get; set; }
		public string Street { get; set; }
		public string Telephone { get; set; }
		public string UpdatedAt { get; set; }
	}

	internal class Payment
	{
		public string AmountOrdered { get; set; }
		public string BaseAmountOrdered { get; set; }
		public string BaseShippingAmount { get; set; }
		public string CcExpMonth { get; set; }
		public string CcExpYear { get; set; }
		public string CcLast4 { get; set; }
		public string CcNumberEnc { get; set; }
		public string CcOwner { get; set; }
		public string CcSsStartMonth { get; set; }
		public string CcSsStartYear { get; set; }
		public string CcType { get; set; }
		public string CreatedAt { get; set; }
		public string IncrementId { get; set; }
		public string IsActive { get; set; }
		public string Method { get; set; }
		public string ParentId { get; set; }
		public string PaymentId { get; set; }
		public string PoNumber { get; set; }
		public string ShippingAmount { get; set; }
		public string UpdatedAt { get; set; }
	}

	internal class BillingAddress
	{
		public string AddressId { get; set; }
		public string AddressType { get; set; }
		public string City { get; set; }
		public string Company { get; set; }
		public string CountryId { get; set; }
		public string CreatedAt { get; set; }
		public string Fax { get; set; }
		public string Firstname { get; set; }
		public string IncrementId { get; set; }
		public string IsActive { get; set; }
		public string Lastname { get; set; }
		public string ParentId { get; set; }
		public string Postcode { get; set; }
		public string Region { get; set; }
		public string RegionId { get; set; }
		public string Street { get; set; }
		public string Telephone { get; set; }
		public string UpdatedAt { get; set; }
	}
}