using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MagentoAccess.Magento2salesOrderRepositoryV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository;

namespace MagentoAccess.Models.Services.Soap.GetOrders
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

		public OrderInfoResponse( MagentoSoapServiceReference_v_1_14_1_EE.salesOrderInfoResponse res )
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
			this.ShippingAddressId = res.result.shipping_address_id;
			this.ShippingAmount = res.result.shipping_amount;
			this.ShippingDescription = res.result.shipping_description;
			this.ShippingFirstname = res.result.shipping_firstname;
			this.ShippingLastname = res.result.shipping_lastname;
			this.ShippingMethod = res.result.shipping_method;
			this.ShippingName = res.result.shipping_name;
			this.State = res.result.state;
			this.Status = res.result.status;

			if( res.result.status_history != null )
			{
				this.StatusHistory = new List< StatusHistoryRecord >(
					res.result.status_history.Select( x => new StatusHistoryRecord( x ) ) );
			}
			this.StoreCurrencyCode = res.result.store_currency_code;
			this.StoreId = res.result.store_id;
			this.StoreName = res.result.store_name;
			this.StoreToBaseRate = res.result.store_to_base_rate;
			this.StoreToOrderRate = res.result.store_to_order_rate;
			this.Subtotal = res.result.subtotal;
			this.TaxAmount = res.result.tax_amount;
			this.TotalCanceled = res.result.total_canceled;
			this.TotalInvoiced = res.result.total_invoiced;
			this.TotalOfflineRefunded = res.result.total_offline_refunded;
			this.TotalOnlineRefunded = res.result.total_online_refunded;
			this.TotalPaid = res.result.total_paid;
			this.TotalQtyOrdered = res.result.total_qty_ordered;
			this.TotalRefunded = res.result.total_refunded;
			this.UpdatedAT = res.result.updated_at;
			this.Weight = res.result.weight;
		}

		public OrderInfoResponse( salesOrderRepositoryV1GetResponse1 response )
		{
			var res = response.salesOrderRepositoryV1GetResponse.result;
			var invariantCulture = CultureInfo.InvariantCulture;

			this.AppliedRuleIds = res.appliedRuleIds;
			this.BaseCurrencyCode = res.baseCurrencyCode;
			this.BaseDiscountAmount = ( res.baseDiscountAmount ?? string.Empty ).ToString( invariantCulture );
			this.BaseGrandTotal = ( res.baseGrandTotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseShippingAmount = ( res.baseShippingAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseSubtotal = ( res.baseSubtotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTaxAmount = ( res.baseTaxAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseToGlobalRate = ( res.baseToGlobalRate ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseToOrderRate = ( res.baseToOrderRate ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalCanceled = ( res.baseTotalCanceled ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalInvoiced = ( res.baseTotalInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalOfflineRefunded = ( res.baseTotalOfflineRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalOnlineRefunded = ( res.baseTotalOnlineRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalPaid = ( res.baseTotalPaid ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalQtyOrdered = ( res.baseTotalQtyOrdered ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalRefunded = ( res.baseTotalRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			if( res.billingAddress != null )
			{
				var billingAddress = new BillingAddress
				{
					//AddressId = res.billingAddress.addressId,
					AddressType = res.billingAddress.addressType,
					City = res.billingAddress.city,
					Company = res.billingAddress.company,
					CountryId = res.billingAddress.countryId,
					//CreatedAt = res.billingAddress.createdAt,
					Fax = res.billingAddress.fax,
					Firstname = res.billingAddress.firstname,
					//IncrementId = res.billingAddress.incrementId,
					//IsActive = res.billingAddress.isActive,
					Lastname = res.billingAddress.lastname,
					ParentId = res.billingAddress.parentId.ToString( CultureInfo.InvariantCulture ),
					Postcode = res.billingAddress.postcode,
					Region = res.billingAddress.region,
					RegionId = res.billingAddress.regionId.ToString( CultureInfo.InvariantCulture ),
					Street = string.Join( "", res.billingAddress.street.ToList() ).ToString( CultureInfo.InvariantCulture ),
					Telephone = res.billingAddress.telephone,
					//UpdatedAt = res.billingAddress.updatedAt,
				};
				this.BillingAddress = billingAddress;
			}
			this.BillingAddressId = res.billingAddressId.ToString( CultureInfo.InvariantCulture );
			//BillingFirstname = res.billingFirstname;
			//BillingLastname = res.billingLastname;
			//BillingName = res.billingName;
			this.CreatedAt = res.createdAt;
			this.CustomerEmail = res.customerEmail;
			this.CustomerFirstname = res.customerFirstname;
			this.CustomerGroupId = ( res.customerGroupId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.CustomerId = ( res.customerId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.CustomerIsGuest = ( res.customerIsGuest ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.CustomerLastname = res.customerLastname;
			this.CustomerNoteNotify = res.customerNoteNotify.ToString( CultureInfo.InvariantCulture );
			this.DiscountAmount = res.discountAmount.ToString( CultureInfo.InvariantCulture );
			this.EmailSent = ( res.emailSent ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			//GiftMessage = res.giftMessage;
			//GiftMessageId = res.giftMessageId;
			this.GlobalCurrencyCode = res.globalCurrencyCode;
			this.GrandTotal = res.grandTotal.ToString( CultureInfo.InvariantCulture );
			this.IncrementId = res.incrementId;
			//IsActive = res.isActive;
			this.IsVirtual = ( res.isVirtual ?? string.Empty ).ToString( CultureInfo.InvariantCulture );

			if( res.items != null )
				this.Items = res.items.Select( x => new OrderItemEntity( x ) );

			this.OrderCurrencyCode = res.orderCurrencyCode;
			this.OrderId = ( res.extOrderId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.ParentId = res.relationParentId;

			if( res.payment != null )
			{
				var payment = new Payment()
				{
					AmountOrdered = res.payment.amountOrdered.ToString( CultureInfo.InvariantCulture ),
					BaseAmountOrdered = res.payment.baseAmountOrdered.ToString( CultureInfo.InvariantCulture ),
					BaseShippingAmount = res.payment.baseShippingAmount.ToString( CultureInfo.InvariantCulture ),
					CcExpMonth = res.payment.ccExpMonth,
					CcExpYear = res.payment.ccExpYear,
					CcLast4 = res.payment.ccLast4,
					CcNumberEnc = res.payment.ccNumberEnc,
					CcOwner = res.payment.ccOwner,
					CcSsStartMonth = res.payment.ccSsStartMonth,
					CcSsStartYear = res.payment.ccSsStartYear,
					CcType = res.payment.ccType,
					//CreatedAt = res.payment.createdAt,
					IncrementId = res.payment.entityId.ToString( CultureInfo.InvariantCulture ),
					//IsActive = res.payment.isActive,
					Method = res.payment.method,
					ParentId = res.payment.parentId.ToString( CultureInfo.InvariantCulture ),
					PaymentId = res.payment.quotePaymentId.ToString( CultureInfo.InvariantCulture ),
					PoNumber = res.payment.poNumber,
					ShippingAmount = res.payment.shippingAmount.ToString( CultureInfo.InvariantCulture ),
					//UpdatedAt = res.payment.updatedAt,
				};
				this.Payment = payment;
			}
			this.QuoteId = ( res.quoteId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.RemoteIp = res.remoteIp;
			//if (res.shippingAddress != null)
			//{
			//	ShippingAddress = new ShippingAddress()
			//	{
			//		//AddressId = res.shippingAddress.addressId,
			//		//AddressType = res.shippingAddress.addressType,
			//		//City = res.shippingAddress.city,
			//		Company = res.shippingAddress.company,
			//		CountryId = res.shippingAddress.countryId,
			//		CreatedAt = res.shippingAddress.createdAt,
			//		Fax = res.shippingAddress.fax,
			//		Firstname = res.shippingAddress.firstname,
			//		IncrementId = res.shippingAddress.incrementId,
			//		IsActive = res.shippingAddress.isActive,
			//		Lastname = res.shippingAddress.lastname,
			//		ParentId = res.shippingAddress.parentId,
			//		Postcode = res.shippingAddress.postcode,
			//		Region = res.shippingAddress.region,
			//		RegionId = res.shippingAddress.regionId,
			//		Street = res.shippingAddress.street,
			//		Telephone = res.shippingAddress.telephone,
			//		UpdatedAt = res.shippingAddress.updatedAt,
			//	};
			//}
			//ShippingAddressId = res.shippingAddressId;
			this.ShippingAmount = ( res.shippingAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.ShippingDescription = res.shippingDescription;
			//ShippingFirstname = res.shippingFirstname;
			//ShippingLastname = res.shippingLastname;
			//ShippingMethod = res.shippingMethod;
			//ShippingName = res.shippingName;
			this.State = res.state;
			this.Status = res.status;

			if( res.statusHistories != null )
			{
				this.StatusHistory = new List< StatusHistoryRecord >(
					res.statusHistories.Select( x => new StatusHistoryRecord( x ) ) );
			}
			this.StoreCurrencyCode = res.storeCurrencyCode;
			this.StoreId = res.storeId.ToString( CultureInfo.InvariantCulture );
			this.StoreName = res.storeName;
			this.StoreToBaseRate = ( res.storeToBaseRate ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.StoreToOrderRate = ( res.storeToOrderRate ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Subtotal = ( res.subtotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TaxAmount = ( res.taxAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalCanceled = ( res.totalCanceled ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalInvoiced = ( res.totalInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalOfflineRefunded = ( res.totalOfflineRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalOnlineRefunded = ( res.totalOnlineRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalPaid = ( res.totalPaid ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalQtyOrdered = ( res.totalQtyOrdered ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalRefunded = ( res.totalRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.UpdatedAT = res.updatedAt;
			this.Weight = ( res.weight ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
		}

		public OrderInfoResponse( Magento2salesOrderRepositoryV1_v_2_1_0_0_CE.salesOrderRepositoryV1GetResponse1 response )
		{
			if( response == null )
				return;

			var res = response.salesOrderRepositoryV1GetResponse.result;
			var invariantCulture = CultureInfo.InvariantCulture;

			this.AppliedRuleIds = res.appliedRuleIds;
			this.BaseCurrencyCode = res.baseCurrencyCode;
			this.BaseDiscountAmount = ( res.baseDiscountAmount ?? string.Empty ).ToString( invariantCulture );
			this.BaseGrandTotal = ( res.baseGrandTotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseShippingAmount = ( res.baseShippingAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseSubtotal = ( res.baseSubtotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTaxAmount = ( res.baseTaxAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseToGlobalRate = ( res.baseToGlobalRate ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseToOrderRate = ( res.baseToOrderRate ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalCanceled = ( res.baseTotalCanceled ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalInvoiced = ( res.baseTotalInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalOfflineRefunded = ( res.baseTotalOfflineRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalOnlineRefunded = ( res.baseTotalOnlineRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalPaid = ( res.baseTotalPaid ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalQtyOrdered = ( res.baseTotalQtyOrdered ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTotalRefunded = ( res.baseTotalRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			if( res.billingAddress != null )
			{
				var billingAddress = new BillingAddress
				{
					//AddressId = res.billingAddress.addressId,
					AddressType = res.billingAddress.addressType,
					City = res.billingAddress.city,
					Company = res.billingAddress.company,
					CountryId = res.billingAddress.countryId,
					//CreatedAt = res.billingAddress.createdAt,
					Fax = res.billingAddress.fax,
					Firstname = res.billingAddress.firstname,
					//IncrementId = res.billingAddress.incrementId,
					//IsActive = res.billingAddress.isActive,
					Lastname = res.billingAddress.lastname,
					ParentId = res.billingAddress.parentId.ToString( CultureInfo.InvariantCulture ),
					Postcode = res.billingAddress.postcode,
					Region = res.billingAddress.region,
					RegionId = res.billingAddress.regionId.ToString( CultureInfo.InvariantCulture ),
					Street = string.Join( "", res.billingAddress.street.ToList() ).ToString( CultureInfo.InvariantCulture ),
					Telephone = res.billingAddress.telephone,
					//UpdatedAt = res.billingAddress.updatedAt,
				};
				this.BillingAddress = billingAddress;
			}
			this.BillingAddressId = res.billingAddressId.ToString( CultureInfo.InvariantCulture );
			//BillingFirstname = res.billingFirstname;
			//BillingLastname = res.billingLastname;
			//BillingName = res.billingName;
			this.CreatedAt = res.createdAt;
			this.CustomerEmail = res.customerEmail;
			this.CustomerFirstname = res.customerFirstname;
			this.CustomerGroupId = ( res.customerGroupId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.CustomerId = ( res.customerId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.CustomerIsGuest = ( res.customerIsGuest ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.CustomerLastname = res.customerLastname;
			this.CustomerNoteNotify = ( res.customerNoteNotify ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.DiscountAmount = ( res.discountAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.EmailSent = ( res.emailSent ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			//GiftMessage = res.giftMessage;
			//GiftMessageId = res.giftMessageId;
			this.GlobalCurrencyCode = res.globalCurrencyCode;
			this.GrandTotal = ( res.grandTotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.IncrementId = res.incrementId;
			//IsActive = res.isActive;
			this.IsVirtual = ( res.isVirtual.ToStringEmptyOnNull( CultureInfo.InvariantCulture ) ?? string.Empty ).ToString( CultureInfo.InvariantCulture );

			if( res.items != null )
				this.Items = res.items.Select( x => new OrderItemEntity( x ) );

			this.OrderCurrencyCode = res.orderCurrencyCode;
			this.OrderId = ( res.extOrderId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.ParentId = res.relationParentId;

			if( res.payment != null )
			{
				var payment = new Payment()
				{
					AmountOrdered = ( res.payment.amountOrdered ?? string.Empty ).ToString( CultureInfo.InvariantCulture ),
					BaseAmountOrdered = ( res.payment.baseAmountOrdered ?? string.Empty ).ToString( CultureInfo.InvariantCulture ),
					BaseShippingAmount = ( res.payment.baseShippingAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture ),
					CcExpMonth = res.payment.ccExpMonth,
					CcExpYear = res.payment.ccExpYear,
					CcLast4 = res.payment.ccLast4,
					CcNumberEnc = res.payment.ccNumberEnc,
					CcOwner = res.payment.ccOwner,
					CcSsStartMonth = res.payment.ccSsStartMonth,
					CcSsStartYear = res.payment.ccSsStartYear,
					CcType = res.payment.ccType,
					//CreatedAt = res.payment.createdAt,
					IncrementId = res.payment.entityId.ToString( CultureInfo.InvariantCulture ),
					//IsActive = res.payment.isActive,
					Method = res.payment.method,
					ParentId = res.payment.parentId.ToString( CultureInfo.InvariantCulture ),
					PaymentId = res.payment.quotePaymentId.ToString( CultureInfo.InvariantCulture ),
					PoNumber = res.payment.poNumber,
					ShippingAmount = res.payment.shippingAmount.ToString( CultureInfo.InvariantCulture ),
					//UpdatedAt = res.payment.updatedAt,
				};
				this.Payment = payment;
			}
			this.QuoteId = ( res.quoteId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.RemoteIp = res.remoteIp;
			//if (res.shippingAddress != null)
			//{
			//	ShippingAddress = new ShippingAddress()
			//	{
			//		//AddressId = res.shippingAddress.addressId,
			//		//AddressType = res.shippingAddress.addressType,
			//		//City = res.shippingAddress.city,
			//		Company = res.shippingAddress.company,
			//		CountryId = res.shippingAddress.countryId,
			//		CreatedAt = res.shippingAddress.createdAt,
			//		Fax = res.shippingAddress.fax,
			//		Firstname = res.shippingAddress.firstname,
			//		IncrementId = res.shippingAddress.incrementId,
			//		IsActive = res.shippingAddress.isActive,
			//		Lastname = res.shippingAddress.lastname,
			//		ParentId = res.shippingAddress.parentId,
			//		Postcode = res.shippingAddress.postcode,
			//		Region = res.shippingAddress.region,
			//		RegionId = res.shippingAddress.regionId,
			//		Street = res.shippingAddress.street,
			//		Telephone = res.shippingAddress.telephone,
			//		UpdatedAt = res.shippingAddress.updatedAt,
			//	};
			//}
			//ShippingAddressId = res.shippingAddressId;
			this.ShippingAmount = ( res.shippingAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.ShippingDescription = res.shippingDescription;
			//ShippingFirstname = res.shippingFirstname;
			//ShippingLastname = res.shippingLastname;
			//ShippingMethod = res.shippingMethod;
			//ShippingName = res.shippingName;
			this.State = res.state;
			this.Status = res.status;

			if( res.statusHistories != null )
			{
				this.StatusHistory = new List< StatusHistoryRecord >(
					res.statusHistories.Select( x => new StatusHistoryRecord( x ) ) );
			}
			this.StoreCurrencyCode = res.storeCurrencyCode;
			this.StoreId = ( res.storeId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.StoreName = res.storeName;
			this.StoreToBaseRate = ( res.storeToBaseRate ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.StoreToOrderRate = ( res.storeToOrderRate ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Subtotal = ( res.subtotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TaxAmount = ( res.taxAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalCanceled = ( res.totalCanceled ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalInvoiced = ( res.totalInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalOfflineRefunded = ( res.totalOfflineRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalOnlineRefunded = ( res.totalOnlineRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalPaid = ( res.totalPaid ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalQtyOrdered = ( res.totalQtyOrdered ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TotalRefunded = ( res.totalRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.UpdatedAT = res.updatedAt;
			this.Weight = ( res.weight ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
		}

		public OrderInfoResponse( Item res )
		{
			if( res == null )
				return;

			var invariantCulture = CultureInfo.InvariantCulture;

			this.AppliedRuleIds = res.applied_rule_ids;
			this.BaseCurrencyCode = res.base_currency_code;
			this.BaseDiscountAmount = res.base_discount_amount.ToStringEmptyOnNull( invariantCulture );
			this.BaseGrandTotal = res.base_grand_total.ToStringEmptyOnNull( invariantCulture );
			this.BaseShippingAmount = res.base_shipping_amount.ToStringEmptyOnNull( invariantCulture );
			this.BaseSubtotal = res.base_subtotal.ToStringEmptyOnNull( invariantCulture );
			this.BaseTaxAmount = res.base_tax_amount.ToStringEmptyOnNull( invariantCulture );
			this.BaseToGlobalRate = res.base_to_global_rate.ToStringEmptyOnNull( invariantCulture );
			this.BaseToOrderRate = res.base_to_order_rate.ToStringEmptyOnNull( invariantCulture );
			this.BaseTotalCanceled = res.base_total_canceled.ToStringEmptyOnNull( invariantCulture );
			this.BaseTotalInvoiced = res.base_total_invoiced.ToStringEmptyOnNull( invariantCulture );
			this.BaseTotalOfflineRefunded = res.base_total_offline_refunded.ToStringEmptyOnNull( invariantCulture );
			this.BaseTotalOnlineRefunded = res.base_total_online_refunded.ToStringEmptyOnNull( invariantCulture );
			this.BaseTotalPaid = res.base_total_paid.ToStringEmptyOnNull( invariantCulture );
			this.BaseTotalQtyOrdered = res.base_total_qty_ordered.ToStringEmptyOnNull( invariantCulture );
			this.BaseTotalRefunded = res.base_total_refunded.ToStringEmptyOnNull( invariantCulture );
			if( res.billing_address != null )
			{
				var billingAddress = new BillingAddress
				{
					AddressId = res.billing_address.customer_address_id.ToStringEmptyOnNull( invariantCulture ), //address_id,
					AddressType = res.billing_address.address_type,
					City = res.billing_address.city,
					Company = res.billing_address.company,
					CountryId = res.billing_address.country_id,
					//CreatedAt = res.billing_address.created_at,
					Fax = res.billing_address.fax,
					Firstname = res.billing_address.firstname,
					//IncrementId = res.billing_address.increment_id,
					//IsActive = res.billing_address.is_active,
					Lastname = res.billing_address.lastname,
					ParentId = res.billing_address.parent_id.ToStringEmptyOnNull( invariantCulture ),
					Postcode = res.billing_address.postcode,
					Region = res.billing_address.region,
					RegionId = res.billing_address.region_id.ToStringEmptyOnNull( invariantCulture ),
					Street = string.Join( ",", res.billing_address.street ),
					Telephone = res.billing_address.telephone,
					//UpdatedAt = res.billing_address.updated_at,
				};
				this.BillingAddress = billingAddress;
			}
			this.BillingAddressId = res.billing_address_id.ToStringEmptyOnNull( invariantCulture );
			this.BillingFirstname = res.customer_firstname; //.billing_firstname;
			this.BillingLastname = res.customer_lastname; // billing_lastname;
			//this.BillingName = res.billing_name;
			this.CreatedAt = res.created_at;
			this.CustomerEmail = res.customer_email;
			this.CustomerFirstname = res.customer_firstname;
			this.CustomerGroupId = res.customer_group_id.ToStringEmptyOnNull( invariantCulture );
			this.CustomerId = res.customer_id.ToStringEmptyOnNull( invariantCulture );
			this.CustomerIsGuest = res.customer_is_guest.ToStringEmptyOnNull( invariantCulture );
			this.CustomerLastname = res.customer_lastname;
			this.CustomerNoteNotify = res.customer_note_notify.ToStringEmptyOnNull( invariantCulture );
			this.DiscountAmount = res.discount_amount.ToStringEmptyOnNull( invariantCulture );
			this.EmailSent = res.email_sent.ToStringEmptyOnNull( invariantCulture );
			//this.GiftMessage = res.gift_message;
			//this.GiftMessageId = res.gift_message_id;
			this.GlobalCurrencyCode = res.global_currency_code;
			this.GrandTotal = res.grand_total.ToStringEmptyOnNull( invariantCulture );
			this.IncrementId = res.increment_id;
			//this.IsActive = res.is_active;
			this.IsVirtual = res.is_virtual.ToStringEmptyOnNull( invariantCulture );

			if( res.items != null )
				this.Items = res.items.Select( x => new OrderItemEntity( x ) );

			this.OrderCurrencyCode = res.order_currency_code;
			this.OrderId = res.ext_order_id; // order_id;
			this.ParentId = res.relation_parent_id; // parent_id;

			if( res.payment != null )
			{
				var payment = new Payment()
				{
					AmountOrdered = res.payment.amount_ordered.ToStringEmptyOnNull( invariantCulture ),
					BaseAmountOrdered = res.payment.base_amount_ordered.ToStringEmptyOnNull( invariantCulture ),
					BaseShippingAmount = res.payment.base_shipping_amount.ToStringEmptyOnNull( invariantCulture ),
					CcExpMonth = res.payment.cc_exp_month,
					CcExpYear = res.payment.cc_exp_year,
					CcLast4 = res.payment.cc_last4,
					CcNumberEnc = res.payment.cc_number_enc,
					CcOwner = res.payment.cc_owner,
					CcSsStartMonth = res.payment.cc_ss_start_month,
					CcSsStartYear = res.payment.cc_ss_start_year,
					CcType = res.payment.cc_type,
					//CreatedAt = res.payment.created_at,
					//IncrementId = res.payment.increment_id,
					//IsActive = res.payment.is_active,
					Method = res.payment.method,
					ParentId = res.payment.parent_id.ToStringEmptyOnNull( invariantCulture ),
					//PaymentId = res.payment.payment_id,
					PoNumber = res.payment.po_number,
					ShippingAmount = res.payment.shipping_amount.ToStringEmptyOnNull( invariantCulture ),
					//UpdatedAt = res.payment.updated_at,
				};
				this.Payment = payment;
			}
			this.QuoteId = res.quote_id.ToStringEmptyOnNull( invariantCulture );
			this.RemoteIp = res.remote_ip;
			//if (res.shipping_address != null)
			//{
			//	ShippingAddress = new ShippingAddress()
			//	{
			//		AddressId = res.shipping_address.address_id,
			//		AddressType = res.shipping_address.address_type,
			//		City = res.shipping_address.city,
			//		Company = res.shipping_address.company,
			//		CountryId = res.shipping_address.country_id,
			//		CreatedAt = res.shipping_address.created_at,
			//		Fax = res.shipping_address.fax,
			//		Firstname = res.shipping_address.firstname,
			//		IncrementId = res.shipping_address.increment_id,
			//		IsActive = res.shipping_address.is_active,
			//		Lastname = res.shipping_address.lastname,
			//		ParentId = res.shipping_address.parent_id,
			//		Postcode = res.shipping_address.postcode,
			//		Region = res.shipping_address.region,
			//		RegionId = res.shipping_address.region_id,
			//		Street = res.shipping_address.street,
			//		Telephone = res.shipping_address.telephone,
			//		UpdatedAt = res.shipping_address.updated_at,
			//	};
			//}
			//ShippingAddressId = res.shipping_address_id;
			//ShippingAmount = res.shipping_amount;
			//ShippingDescription = res.shipping_description;
			//ShippingFirstname = res.shipping_firstname;
			//ShippingLastname = res.shipping_lastname;
			//ShippingMethod = res.shipping_method;
			//ShippingName = res.shipping_name;
			this.State = res.state;
			this.Status = res.status;

			if( res.status_histories != null )
			{
				this.StatusHistory = new List< StatusHistoryRecord >(
					res.status_histories.Select( x => new StatusHistoryRecord( x ) ) );
			}
			this.StoreCurrencyCode = res.store_currency_code;
			this.StoreId = res.store_id.ToStringEmptyOnNull( invariantCulture );
			this.StoreName = res.store_name;
			this.StoreToBaseRate = res.store_to_base_rate.ToStringEmptyOnNull( invariantCulture );
			this.StoreToOrderRate = res.store_to_order_rate.ToStringEmptyOnNull( invariantCulture );
			this.Subtotal = res.subtotal.ToStringEmptyOnNull( invariantCulture );
			this.TaxAmount = res.tax_amount.ToStringEmptyOnNull( invariantCulture );
			this.TotalCanceled = res.total_canceled.ToStringEmptyOnNull( invariantCulture );
			this.TotalInvoiced = res.total_invoiced.ToStringEmptyOnNull( invariantCulture );
			this.TotalOfflineRefunded = res.total_offline_refunded.ToStringEmptyOnNull( invariantCulture );
			this.TotalOnlineRefunded = res.total_online_refunded.ToStringEmptyOnNull( invariantCulture );
			this.TotalPaid = res.total_paid.ToStringEmptyOnNull( invariantCulture );
			this.TotalQtyOrdered = res.total_qty_ordered.ToStringEmptyOnNull( invariantCulture );
			this.TotalRefunded = res.total_refunded.ToStringEmptyOnNull( invariantCulture );
			this.UpdatedAT = res.updated_at;
			this.Weight = res.weight.ToStringEmptyOnNull( invariantCulture );
		}

		public OrderInfoResponse( Order response )
		{
			throw new NotImplementedException();
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

		public OrderItemEntity( MagentoSoapServiceReference_v_1_14_1_EE.salesOrderItemEntity salesOrderItemEntity )
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

		public OrderItemEntity( SalesDataOrderItemInterface salesOrderItemEntity )
		{
			this.AmountRefunded = ( salesOrderItemEntity.amountRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.AppliedRuleIds = ( salesOrderItemEntity.appliedRuleIds ?? string.Empty );
			this.BaseAmountRefunded = ( salesOrderItemEntity.baseAmountRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseDiscountAmount = ( salesOrderItemEntity.baseDiscountAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseDiscountInvoiced = ( salesOrderItemEntity.baseDiscountInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseOriginalPrice = ( salesOrderItemEntity.baseOriginalPrice ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BasePrice = ( salesOrderItemEntity.basePrice ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseRowInvoiced = ( salesOrderItemEntity.baseRowInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseRowTotal = ( salesOrderItemEntity.baseRowTotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTaxAmount = ( salesOrderItemEntity.baseTaxAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTaxBeforeDiscount = ( salesOrderItemEntity.baseTaxBeforeDiscount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTaxInvoiced = ( salesOrderItemEntity.baseTaxInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxAppliedAmount = ( salesOrderItemEntity.baseWeeeTaxAppliedAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxAppliedRowAmount = ( salesOrderItemEntity.weeeTaxAppliedRowAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxDisposition = ( salesOrderItemEntity.baseWeeeTaxDisposition ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxRowDisposition = ( salesOrderItemEntity.baseWeeeTaxRowDisposition ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Cost = ( salesOrderItemEntity.baseCost ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.CreatedAt = ( salesOrderItemEntity.createdAt ?? string.Empty );
			this.DiscountAmount = ( salesOrderItemEntity.discountAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.DiscountInvoiced = ( salesOrderItemEntity.discountInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.DiscountPercent = ( salesOrderItemEntity.discountPercent ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.FreeShipping = ( salesOrderItemEntity.freeShipping ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			//GiftMessage = salesOrderItemEntity.giftMessage;
			//GiftMessageAvailable = salesOrderItemEntity.giftMessageAvailable;
			//GiftMessageId = salesOrderItemEntity.giftMessageId;
			this.IsQtyDecimal = ( salesOrderItemEntity.isQtyDecimal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.IsVirtual = ( salesOrderItemEntity.isVirtual ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.ItemId = ( salesOrderItemEntity.itemId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Name = ( salesOrderItemEntity.name ?? string.Empty );
			this.NoDiscount = ( salesOrderItemEntity.noDiscount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.OrderId = ( salesOrderItemEntity.orderId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.OriginalPrice = ( salesOrderItemEntity.originalPrice ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Price = ( salesOrderItemEntity.price ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.ProductId = ( salesOrderItemEntity.productId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			//ProductOptions = salesOrderItemEntity.productOptions;
			this.ProductType = ( salesOrderItemEntity.productType ?? string.Empty );
			this.QtyCanceled = ( salesOrderItemEntity.qtyCanceled ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QtyInvoiced = ( salesOrderItemEntity.qtyInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QtyOrdered = ( salesOrderItemEntity.qtyOrdered ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QtyRefunded = ( salesOrderItemEntity.qtyRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QtyShipped = ( salesOrderItemEntity.qtyShipped ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QuoteItemId = ( salesOrderItemEntity.quoteItemId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.RowInvoiced = ( salesOrderItemEntity.rowInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.RowTotal = ( salesOrderItemEntity.rowTotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.RowWeight = ( salesOrderItemEntity.rowWeight ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Sku = ( salesOrderItemEntity.sku ?? string.Empty );
			this.TaxAmount = ( salesOrderItemEntity.taxAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TaxBeforeDiscount = ( salesOrderItemEntity.taxBeforeDiscount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TaxInvoiced = ( salesOrderItemEntity.taxInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TaxPercent = ( salesOrderItemEntity.taxPercent ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.UpdatedAt = ( salesOrderItemEntity.updatedAt ?? string.Empty );
			this.WeeeTaxApplied = ( salesOrderItemEntity.weeeTaxApplied ?? string.Empty );
			this.WeeeTaxAppliedAmount = ( salesOrderItemEntity.weeeTaxAppliedAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.WeeeTaxAppliedRowAmount = ( salesOrderItemEntity.weeeTaxAppliedRowAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.WeeeTaxDisposition = ( salesOrderItemEntity.weeeTaxDisposition ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.WeeeTaxRowDisposition = ( salesOrderItemEntity.weeeTaxRowDisposition ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Weight = ( salesOrderItemEntity.weight ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
		}

		public OrderItemEntity( Magento2salesOrderRepositoryV1_v_2_1_0_0_CE.SalesDataOrderItemInterface salesOrderItemEntity )
		{
			this.AmountRefunded = ( salesOrderItemEntity.amountRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.AppliedRuleIds = ( salesOrderItemEntity.appliedRuleIds ?? string.Empty );
			this.BaseAmountRefunded = ( salesOrderItemEntity.baseAmountRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseDiscountAmount = ( salesOrderItemEntity.baseDiscountAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseDiscountInvoiced = ( salesOrderItemEntity.baseDiscountInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseOriginalPrice = ( salesOrderItemEntity.baseOriginalPrice ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BasePrice = ( salesOrderItemEntity.basePrice ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseRowInvoiced = ( salesOrderItemEntity.baseRowInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseRowTotal = ( salesOrderItemEntity.baseRowTotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTaxAmount = ( salesOrderItemEntity.baseTaxAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTaxBeforeDiscount = ( salesOrderItemEntity.baseTaxBeforeDiscount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseTaxInvoiced = ( salesOrderItemEntity.baseTaxInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxAppliedAmount = ( salesOrderItemEntity.baseWeeeTaxAppliedAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxAppliedRowAmount = ( salesOrderItemEntity.weeeTaxAppliedRowAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxDisposition = ( salesOrderItemEntity.baseWeeeTaxDisposition ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxRowDisposition = ( salesOrderItemEntity.baseWeeeTaxRowDisposition ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Cost = ( salesOrderItemEntity.baseCost ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.CreatedAt = ( salesOrderItemEntity.createdAt ?? string.Empty );
			this.DiscountAmount = ( salesOrderItemEntity.discountAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.DiscountInvoiced = ( salesOrderItemEntity.discountInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.DiscountPercent = ( salesOrderItemEntity.discountPercent ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.FreeShipping = ( salesOrderItemEntity.freeShipping ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			//this.GiftMessage = salesOrderItemEntity.giftMessage;
			//this.GiftMessageAvailable = salesOrderItemEntity.giftMessageAvailable;
			//this.GiftMessageId = salesOrderItemEntity.giftMessageId;
			this.IsQtyDecimal = ( salesOrderItemEntity.isQtyDecimal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.IsVirtual = ( salesOrderItemEntity.isVirtual ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.ItemId = ( salesOrderItemEntity.itemId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Name = ( salesOrderItemEntity.name ?? string.Empty );
			this.NoDiscount = ( salesOrderItemEntity.noDiscount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.OrderId = ( salesOrderItemEntity.orderId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.OriginalPrice = ( salesOrderItemEntity.originalPrice ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Price = ( salesOrderItemEntity.price ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.ProductId = ( salesOrderItemEntity.productId ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			//this.ProductOptions = salesOrderItemEntity.productOptions;
			this.ProductType = ( salesOrderItemEntity.productType ?? string.Empty );
			this.QtyCanceled = ( salesOrderItemEntity.qtyCanceled ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QtyInvoiced = ( salesOrderItemEntity.qtyInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QtyOrdered = ( salesOrderItemEntity.qtyOrdered ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QtyRefunded = ( salesOrderItemEntity.qtyRefunded ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QtyShipped = ( salesOrderItemEntity.qtyShipped ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.QuoteItemId = salesOrderItemEntity.quoteItemId.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.RowInvoiced = ( salesOrderItemEntity.rowInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.RowTotal = ( salesOrderItemEntity.rowTotal ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.RowWeight = ( salesOrderItemEntity.rowWeight ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Sku = ( salesOrderItemEntity.sku ?? string.Empty );
			this.TaxAmount = ( salesOrderItemEntity.taxAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TaxBeforeDiscount = ( salesOrderItemEntity.taxBeforeDiscount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TaxInvoiced = ( salesOrderItemEntity.taxInvoiced ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.TaxPercent = ( salesOrderItemEntity.taxPercent ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.UpdatedAt = ( salesOrderItemEntity.updatedAt ?? string.Empty );
			this.WeeeTaxApplied = ( salesOrderItemEntity.weeeTaxApplied ?? string.Empty );
			this.WeeeTaxAppliedAmount = ( salesOrderItemEntity.weeeTaxAppliedAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.WeeeTaxAppliedRowAmount = ( salesOrderItemEntity.weeeTaxAppliedRowAmount ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.WeeeTaxDisposition = ( salesOrderItemEntity.weeeTaxDisposition ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.WeeeTaxRowDisposition = ( salesOrderItemEntity.weeeTaxRowDisposition ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
			this.Weight = ( salesOrderItemEntity.weight ?? string.Empty ).ToString( CultureInfo.InvariantCulture );
		}

		public OrderItemEntity( Item2 salesOrderItemEntity )
		{
			this.AmountRefunded = salesOrderItemEntity.amount_refunded.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.AppliedRuleIds = salesOrderItemEntity.applied_rule_ids;
			this.BaseAmountRefunded = salesOrderItemEntity.base_amount_refunded.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseDiscountAmount = salesOrderItemEntity.base_discount_amount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseDiscountInvoiced = salesOrderItemEntity.base_discount_invoiced.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseOriginalPrice = salesOrderItemEntity.base_original_price.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BasePrice = salesOrderItemEntity.base_price.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseRowInvoiced = salesOrderItemEntity.base_row_invoiced.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseRowTotal = salesOrderItemEntity.base_row_total.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseTaxAmount = salesOrderItemEntity.base_tax_amount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseTaxBeforeDiscount = salesOrderItemEntity.base_tax_before_discount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseTaxInvoiced = salesOrderItemEntity.base_tax_invoiced.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxAppliedAmount = salesOrderItemEntity.base_weee_tax_applied_amount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxAppliedRowAmount = salesOrderItemEntity.base_weee_tax_applied_row_amnt.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxDisposition = salesOrderItemEntity.base_weee_tax_disposition.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.BaseWeeeTaxRowDisposition = salesOrderItemEntity.base_weee_tax_row_disposition.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.Cost = salesOrderItemEntity.base_cost.ToStringEmptyOnNull( CultureInfo.InvariantCulture ); //.cost;
			this.CreatedAt = salesOrderItemEntity.created_at;
			this.DiscountAmount = salesOrderItemEntity.discount_amount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.DiscountInvoiced = salesOrderItemEntity.discount_invoiced.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.DiscountPercent = salesOrderItemEntity.discount_percent.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.FreeShipping = salesOrderItemEntity.free_shipping.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			//this.GiftMessage = salesOrderItemEntity.gift_message;
			//this.GiftMessageAvailable = salesOrderItemEntity.gift_message_available;
			//this.GiftMessageId = salesOrderItemEntity.gift_message_id;
			this.IsQtyDecimal = salesOrderItemEntity.is_qty_decimal.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.IsVirtual = salesOrderItemEntity.is_virtual.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.ItemId = salesOrderItemEntity.item_id.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.Name = salesOrderItemEntity.name;
			this.NoDiscount = salesOrderItemEntity.no_discount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.OrderId = salesOrderItemEntity.order_id.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.OriginalPrice = salesOrderItemEntity.original_price.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.Price = salesOrderItemEntity.price.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.ProductId = salesOrderItemEntity.product_id.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			//this.ProductOptions = salesOrderItemEntity.product_options;
			this.ProductType = salesOrderItemEntity.product_type;
			this.QtyCanceled = salesOrderItemEntity.qty_canceled.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.QtyInvoiced = salesOrderItemEntity.qty_invoiced.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.QtyOrdered = salesOrderItemEntity.qty_ordered.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.QtyRefunded = salesOrderItemEntity.qty_refunded.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.QtyShipped = salesOrderItemEntity.qty_shipped.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.QuoteItemId = salesOrderItemEntity.quote_item_id.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.RowInvoiced = salesOrderItemEntity.row_invoiced.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.RowTotal = salesOrderItemEntity.row_total.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.RowWeight = salesOrderItemEntity.row_weight.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.Sku = salesOrderItemEntity.sku;
			this.TaxAmount = salesOrderItemEntity.tax_amount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.TaxBeforeDiscount = salesOrderItemEntity.tax_before_discount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.TaxInvoiced = salesOrderItemEntity.tax_invoiced.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.TaxPercent = salesOrderItemEntity.tax_percent.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.UpdatedAt = salesOrderItemEntity.updated_at;
			this.WeeeTaxApplied = salesOrderItemEntity.weee_tax_applied;
			this.WeeeTaxAppliedAmount = salesOrderItemEntity.weee_tax_applied_amount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.WeeeTaxAppliedRowAmount = salesOrderItemEntity.weee_tax_applied_row_amount.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.WeeeTaxDisposition = salesOrderItemEntity.weee_tax_disposition.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.WeeeTaxRowDisposition = salesOrderItemEntity.weee_tax_row_disposition.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.Weight = salesOrderItemEntity.weight.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
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

		public StatusHistoryRecord( MagentoSoapServiceReference_v_1_14_1_EE.salesOrderStatusHistoryEntity salesOrderStatusHistoryEntity )
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

		public StatusHistoryRecord( SalesDataOrderStatusHistoryInterface salesOrderStatusHistoryEntity )
		{
			Comment = salesOrderStatusHistoryEntity.comment;
			CreatedAt = salesOrderStatusHistoryEntity.createdAt;
			IncrementId = salesOrderStatusHistoryEntity.entityId.ToString( CultureInfo.InvariantCulture );
			//IsActive = salesOrderStatusHistoryEntity.isActive;
			IsCustomerNotified = salesOrderStatusHistoryEntity.isCustomerNotified.ToString( CultureInfo.InvariantCulture );
			ParentId = salesOrderStatusHistoryEntity.parentId.ToString( CultureInfo.InvariantCulture );
			Status = salesOrderStatusHistoryEntity.status;
			//UpdatedAT = salesOrderStatusHistoryEntity.updatedAt;
		}

		public StatusHistoryRecord( Magento2salesOrderRepositoryV1_v_2_1_0_0_CE.SalesDataOrderStatusHistoryInterface salesOrderStatusHistoryEntity )
		{
			this.Comment = salesOrderStatusHistoryEntity.comment;
			this.CreatedAt = salesOrderStatusHistoryEntity.createdAt;
			this.IncrementId = salesOrderStatusHistoryEntity.entityId.ToString( CultureInfo.InvariantCulture );
			//IsActive = salesOrderStatusHistoryEntity.isActive;
			this.IsCustomerNotified = salesOrderStatusHistoryEntity.isCustomerNotified.ToString( CultureInfo.InvariantCulture );
			this.ParentId = salesOrderStatusHistoryEntity.parentId.ToString( CultureInfo.InvariantCulture );
			this.Status = salesOrderStatusHistoryEntity.status;
			//UpdatedAT = salesOrderStatusHistoryEntity.updatedAt;
		}

		public StatusHistoryRecord( StatusHistory salesOrderStatusHistoryEntity )
		{
			this.Comment = salesOrderStatusHistoryEntity.comment;
			this.CreatedAt = salesOrderStatusHistoryEntity.created_at;
			this.IncrementId = salesOrderStatusHistoryEntity.entity_id.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			//IsActive = salesOrderStatusHistoryEntity.isActive;
			this.IsCustomerNotified = salesOrderStatusHistoryEntity.is_customer_notified.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.ParentId = salesOrderStatusHistoryEntity.parent_id.ToStringEmptyOnNull( CultureInfo.InvariantCulture );
			this.Status = salesOrderStatusHistoryEntity.status;
			//UpdatedAT = salesOrderStatusHistoryEntity.updatedAt;
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