using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.GetOrders;

namespace MagentoAccess.Services.Parsers
{
	internal class MegentoOrdersResponseParser : MagentoBaseResponseParser< GetOrdersResponse >
	{
		protected override GetOrdersResponse ParseWithoutExceptionHanding( XElement root )
		{
			XNamespace ns = "";

			if( ( ( XElement )root.FirstNode ).Name.ToString() != "data_item" )
			{
				var tempOrder = ConvertToOrder( root, ns );
				return new GetOrdersResponse { Orders = new List< Order >() { tempOrder } };
			}

			var dataItemsNodes = root.Nodes();

			var orderDataItems = dataItemsNodes.Select( x => XElement.Parse( x.ToString() ) ).ToList();

			var orders = orderDataItems.Select( x => ConvertToOrder( x, ns ) ).ToList();

			return new GetOrdersResponse { Orders = orders };
		}

		private static Order ConvertToOrder( XElement x, XNamespace ns )
		{
			var resultOrder = new Order();

			resultOrder.OrderId = GetElementValue( x, ns, "increment_id" );

			OrderStatusesEnum temp;
			resultOrder.Status = Enum.TryParse( GetElementValue( x, ns, "status" ), out temp ) ? temp : OrderStatusesEnum.unknown;

			resultOrder.Customer = GetElementValue( x, ns, "customer_id" );

			resultOrder.BaseDiscount = GetElementValue( x, ns, "base_discount_amount" ).ToDoubleOrDefault();

			resultOrder.BaseGrandTotal = GetElementValue( x, ns, "base_grand_total" ).ToDecimalOrDefault();

			resultOrder.BaseShippingAmount = GetElementValue( x, ns, "base_shipping_amount" ).ToDecimalOrDefault();

			resultOrder.BaseShippingTaxAmount = GetElementValue( x, ns, "base_shipping_tax_amount" ).ToDecimalOrDefault();

			resultOrder.BaseSubtotal = GetElementValue( x, ns, "base_subtotal" ).ToDecimalOrDefault();

			resultOrder.BaseTaxAmount = GetElementValue( x, ns, "base_tax_amount" ).ToDecimalOrDefault();

			resultOrder.BaseTotalPaid = GetElementValue( x, ns, "base_total_paid" ).ToDecimalOrDefault();

			resultOrder.BaseTotalRefunded = GetElementValue( x, ns, "base_total_refunded" ).ToDecimalOrDefault();

			resultOrder.DiscountAmount = GetElementValue( x, ns, "discount_amount" ).ToDoubleOrDefault();

			resultOrder.GrandTotal = GetElementValue( x, ns, "grand_total" ).ToDecimalOrDefault();

			resultOrder.ShippingAmount = GetElementValue( x, ns, "shipping_amount" ).ToDecimalOrDefault();

			resultOrder.ShippingTaxAmount = GetElementValue( x, ns, "shipping_tax_amount" ).ToDecimalOrDefault();

			resultOrder.StoreToOrderRate = GetElementValue( x, ns, "store_to_order_rate" ).ToDecimalOrDefault();

			resultOrder.Subtotal = GetElementValue( x, ns, "subtotal" ).ToDecimalOrDefault();

			resultOrder.TaxAmount = GetElementValue( x, ns, "tax_amount" ).ToDecimalOrDefault();

			resultOrder.TotalPaid = GetElementValue( x, ns, "total_paid" ).ToDecimalOrDefault();

			resultOrder.TotalRefunded = GetElementValue( x, ns, "total_refunded" ).ToDecimalOrDefault();

			resultOrder.BaseShippingDiscountAmount = GetElementValue( x, ns, "base_shipping_discount_amount" ).ToDecimalOrDefault();

			resultOrder.BaseSubtotalInclTax = GetElementValue( x, ns, "base_subtotal_incl_tax" ).ToDecimalOrDefault();

			resultOrder.BaseTotalDue = GetElementValue( x, ns, "base_total_due" ).ToDecimalOrDefault();

			resultOrder.ShippingDiscountAmount = GetElementValue( x, ns, "shipping_discount_amount" ).ToDecimalOrDefault();

			resultOrder.SubtotalInclTax = GetElementValue( x, ns, "subtotal_incl_tax" ).ToDecimalOrDefault();

			resultOrder.TotalDue = GetElementValue( x, ns, "total_due" ).ToDecimalOrDefault();

			//todo: to enum
			resultOrder.BaseCurrencyCode = GetElementValue( x, ns, "base_currency_code" );

			resultOrder.StoreName = GetElementValue( x, ns, "store_name" );

			resultOrder.CreatedAt = GetElementValue( x, ns, "created_at" ).ToDateTimeOrDefault();

			resultOrder.ShippingInclTax = GetElementValue( x, ns, "shipping_incl_tax" ).ToDecimalOrDefault();

			//todo: to enum
			resultOrder.PaymentMethod = GetElementValue( x, ns, "payment_method" );

			var addresses = x.Element( ns + "addresses" );
			if( addresses != null )
			{
				var addressDataItems = addresses.Descendants( ns + "data_item" ).ToList();
				resultOrder.Addresses = addressDataItems.Select( addr =>
				{
					var address = new Address();
					address.Region = GetElementValue( addr, ns, "region" );
					address.Postcode = GetElementValue( addr, ns, "postcode" );
					address.Lastname = GetElementValue( addr, ns, "lastname" );
					address.Street = GetElementValue( addr, ns, "Street" );
					address.City = GetElementValue( addr, ns, "city" );
					address.Email = GetElementValue( addr, ns, "email" );
					address.Telephone = GetElementValue( addr, ns, "telephone" );
					address.CountryId = GetElementValue( addr, ns, "country_id" );
					address.Firstname = GetElementValue( addr, ns, "firstname" );
					//todo: to enum
					address.AddressType = GetElementValue( addr, ns, "address_type" );
					address.Prefix = GetElementValue( addr, ns, "prefix" );
					address.Middlename = GetElementValue( addr, ns, "Middlename" );
					address.Suffix = GetElementValue( addr, ns, "suffix" );
					address.Company = GetElementValue( addr, ns, "company" );
					return address;
				} ).ToList();
			}

			var orderItems = x.Element( ns + "order_items" );
			if( orderItems != null )
			{
				var orderItemsDataItems = orderItems.Nodes().Select( y => XElement.Parse( y.ToString() ) ).ToList();

				resultOrder.Items = orderItemsDataItems.Select( addr =>
				{
					var order = new Item();

					order.ItemId = GetElementValue( addr, ns, "item_id" );

					order.ParentItemId = GetElementValue( addr, ns, "parent_item_id" );

					order.Sku = GetElementValue( addr, ns, "sku" );

					order.Name = GetElementValue( addr, ns, "name" );

					order.QtyCanceled = GetElementValue( addr, ns, "qty_canceled" ).ToDecimalOrDefault();

					order.QtyInvoiced = GetElementValue( addr, ns, "qty_invoiced" ).ToDecimalOrDefault();

					order.QtyOrdered = GetElementValue( addr, ns, "qty_ordered" ).ToDecimalOrDefault();

					order.QtyRefunded = GetElementValue( addr, ns, "qty_refunded" ).ToDecimalOrDefault();

					order.QtyShipped = GetElementValue( addr, ns, "qty_shipped" ).ToDecimalOrDefault();

					order.Price = GetElementValue( addr, ns, "price" ).ToDecimalOrDefault();

					order.BasePrice = GetElementValue( addr, ns, "base_price" ).ToDecimalOrDefault();

					order.OriginalPrice = GetElementValue( addr, ns, "original_price" ).ToDecimalOrDefault();

					order.BaseOriginalPrice = GetElementValue( addr, ns, "base_original_price" ).ToDecimalOrDefault();

					order.TaxPercent = GetElementValue( addr, ns, "tax_percent" ).ToDecimalOrDefault();

					order.TaxAmount = GetElementValue( addr, ns, "tax_amount" ).ToDecimalOrDefault();

					order.BaseTaxAmount = GetElementValue( addr, ns, "base_tax_amount" ).ToDecimalOrDefault();

					order.DscountAmount = GetElementValue( addr, ns, "discount_amount" ).ToDecimalOrDefault();

					order.BaseDiscountAmount = GetElementValue( addr, ns, "base_discount_amount" ).ToDecimalOrDefault();

					order.RowTotal = GetElementValue( addr, ns, "row_total" ).ToDecimalOrDefault();

					order.BaseRowTotal = GetElementValue( addr, ns, "base_row_total" ).ToDecimalOrDefault();

					order.PriceInclTax = GetElementValue( addr, ns, "price_incl_tax" ).ToDecimalOrDefault();

					order.BasePriceInclTax = GetElementValue( addr, ns, "base_price_incl_tax" ).ToDecimalOrDefault();

					order.RawTotalInclTax = GetElementValue( addr, ns, "row_total_incl_tax" ).ToDecimalOrDefault();

					order.BaseRowTotalInclTax = GetElementValue( addr, ns, "base_row_total_incl_tax" ).ToDecimalOrDefault();

					return order;
				} ).ToList();
			}

			var orderComments = x.Element( ns + "order_comments" );
			if( orderComments != null )
			{
				var orderCommentsDataItems = orderComments.Descendants( ns + "data_item" );
				resultOrder.Comments = orderCommentsDataItems.Select( addr =>
				{
					var comment = new Comment();
					comment.IsCustomerNotified = GetElementValue( addr, ns, "is_customer_notified" );
					comment.IsVisibleOnFront = GetElementValue( addr, ns, "is_visible_on_front" );
					comment.CommentText = GetElementValue( addr, ns, "comment" );
					comment.Status = GetElementValue( addr, ns, "status" );

					return comment;
				} ).ToList();
			}

			return resultOrder;
		}
	}
}