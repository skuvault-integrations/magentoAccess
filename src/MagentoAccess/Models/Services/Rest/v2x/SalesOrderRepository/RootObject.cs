using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository
{
	public class ParentItem
	{
	}

	public class FileInfo
	{
		public string base64_encoded_data { get; set; }
		public string type { get; set; }
		public string name { get; set; }
	}

	public class ExtensionAttributes2
	{
		public FileInfo file_info { get; set; }
	}

	public class CustomOption
	{
		public string option_id { get; set; }
		public string option_value { get; set; }
		public ExtensionAttributes2 extension_attributes { get; set; }
	}

	public class ExtensionAttributes3
	{
	}

	public class BundleOption
	{
		public int? option_id { get; set; }
		public int? option_qty { get; set; }
		public List< int? > option_selections { get; set; }
		public ExtensionAttributes3 extension_attributes { get; set; }
	}

	public class DownloadableOption
	{
		public List< int? > downloadable_links { get; set; }
	}

	public class ExtensionAttributes4
	{
	}

	public class ConfigurableItemOption
	{
		public string option_id { get; set; }
		public int? option_value { get; set; }
		public ExtensionAttributes4 extension_attributes { get; set; }
	}

	public class ExtensionAttributes
	{
		public List< CustomOption > custom_options { get; set; }
		public List< BundleOption > bundle_options { get; set; }
		public DownloadableOption downloadable_option { get; set; }
		public List< ConfigurableItemOption > configurable_item_options { get; set; }
	}

	public class ProductOption
	{
		public ExtensionAttributes extension_attributes { get; set; }
	}

	public class ExtensionAttributes6
	{
		public string entity_id { get; set; }
		public string entity_type { get; set; }
	}

	public class GiftMessage
	{
		public int? gift_message_id { get; set; }
		public int? customer_id { get; set; }
		public string sender { get; set; }
		public string recipient { get; set; }
		public string message { get; set; }
		public ExtensionAttributes6 extension_attributes { get; set; }
	}

	public class ExtensionAttributes5
	{
		public GiftMessage gift_message { get; set; }
	}

	public class Item2
	{
		public Item2()
		{
		}

		public string additional_data { get; set; }
		public int? amount_refunded { get; set; }
		public string applied_rule_ids { get; set; }
		public int? base_amount_refunded { get; set; }
		public int? base_cost { get; set; }
		public int? base_discount_amount { get; set; }
		public int? base_discount_invoiced { get; set; }
		public int? base_discount_refunded { get; set; }
		public int? base_discount_tax_compensation_amount { get; set; }
		public int? base_discount_tax_compensation_invoiced { get; set; }
		public int? base_discount_tax_compensation_refunded { get; set; }
		public decimal? base_original_price { get; set; }
		public decimal? base_price { get; set; }
		public decimal? base_price_incl_tax { get; set; }
		public int? base_row_invoiced { get; set; }
		public decimal? base_row_total { get; set; }
		public int? base_row_total_incl_tax { get; set; }
		public int? base_tax_amount { get; set; }
		public int? base_tax_before_discount { get; set; }
		public int? base_tax_invoiced { get; set; }
		public int? base_tax_refunded { get; set; }
		public int? base_weee_tax_applied_amount { get; set; }
		public int? base_weee_tax_applied_row_amnt { get; set; }
		public int? base_weee_tax_disposition { get; set; }
		public int? base_weee_tax_row_disposition { get; set; }
		public string created_at { get; set; }
		public string description { get; set; }
		public int? discount_amount { get; set; }
		public int? discount_invoiced { get; set; }
		public int? discount_percent { get; set; }
		public int? discount_refunded { get; set; }
		public int? event_id { get; set; }
		public string ext_order_item_id { get; set; }
		public int? free_shipping { get; set; }
		public int? gw_base_price { get; set; }
		public int? gw_base_price_invoiced { get; set; }
		public int? gw_base_price_refunded { get; set; }
		public int? gw_base_tax_amount { get; set; }
		public int? gw_base_tax_amount_invoiced { get; set; }
		public int? gw_base_tax_amount_refunded { get; set; }
		public int? gw_id { get; set; }
		public int? gw_price { get; set; }
		public int? gw_price_invoiced { get; set; }
		public int? gw_price_refunded { get; set; }
		public int? gw_tax_amount { get; set; }
		public int? gw_tax_amount_invoiced { get; set; }
		public int? gw_tax_amount_refunded { get; set; }
		public int? discount_tax_compensation_amount { get; set; }
		public int? discount_tax_compensation_canceled { get; set; }
		public int? discount_tax_compensation_invoiced { get; set; }
		public int? discount_tax_compensation_refunded { get; set; }
		public int? is_qty_decimal { get; set; }
		public int? is_virtual { get; set; }
		public int? item_id { get; set; }
		public int? locked_do_invoice { get; set; }
		public int? locked_do_ship { get; set; }
		public string name { get; set; }
		public int? no_discount { get; set; }
		public int? order_id { get; set; }
		public decimal? original_price { get; set; }
		public int? parent_item_id { get; set; }
		public decimal? price { get; set; }
		public decimal? price_incl_tax { get; set; }
		public int? product_id { get; set; }
		public string product_type { get; set; }
		public int? qty_backordered { get; set; }
		public int? qty_canceled { get; set; }
		public int? qty_invoiced { get; set; }
		public int? qty_ordered { get; set; }
		public int? qty_refunded { get; set; }
		public int? qty_returned { get; set; }
		public int? qty_shipped { get; set; }
		public int? quote_item_id { get; set; }
		public int? row_invoiced { get; set; }
		public decimal? row_total { get; set; }
		public decimal? row_total_incl_tax { get; set; }
		public decimal? row_weight { get; set; }
		public string sku { get; set; }
		public int? store_id { get; set; }
		public decimal? tax_amount { get; set; }
		public decimal? tax_before_discount { get; set; }
		public decimal? tax_canceled { get; set; }
		public decimal? tax_invoiced { get; set; }
		public decimal? tax_percent { get; set; }
		public int? tax_refunded { get; set; }
		public string updated_at { get; set; }
		public string weee_tax_applied { get; set; }
		public int? weee_tax_applied_amount { get; set; }
		public int? weee_tax_applied_row_amount { get; set; }
		public int? weee_tax_disposition { get; set; }
		public int? weee_tax_row_disposition { get; set; }
		public int? weight { get; set; }
		public ParentItem parent_item { get; set; }
		public ProductOption product_option { get; set; }
		public ExtensionAttributes5 extension_attributes { get; set; }
	}

	public class ExtensionAttributes7
	{
	}

	public class BillingAddress
	{
		public string address_type { get; set; }
		public string city { get; set; }
		public string company { get; set; }
		public string country_id { get; set; }
		public int? customer_address_id { get; set; }
		public int? customer_id { get; set; }
		public string email { get; set; }
		public int? entity_id { get; set; }
		public string fax { get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public string middlename { get; set; }
		public int? parent_id { get; set; }
		public string postcode { get; set; }
		public string prefix { get; set; }
		public string region { get; set; }
		public string region_code { get; set; }
		public int? region_id { get; set; }
		public List< string > street { get; set; }
		public string suffix { get; set; }
		public string telephone { get; set; }
		public string vat_id { get; set; }
		public int? vat_is_valid { get; set; }
		public string vat_request_date { get; set; }
		public string vat_request_id { get; set; }
		public int? vat_request_success { get; set; }
		public ExtensionAttributes7 extension_attributes { get; set; }
	}

	public class VaultPaymentToken
	{
		public int? entity_id { get; set; }
		public int? customer_id { get; set; }
		public string public_hash { get; set; }
		public string payment_method_code { get; set; }
		public string type { get; set; }
		public string created_at { get; set; }
		public string expires_at { get; set; }
		public string gateway_token { get; set; }
		public string token_details { get; set; }
		public bool is_active { get; set; }
		public bool is_visible { get; set; }
	}

	public class ExtensionAttributes8
	{
		public VaultPaymentToken vault_payment_token { get; set; }
	}

	[ JsonConverter( typeof( LaxPropertyNameMatchingConverter ) ) ]
	public class Payment
	{
		public string account_status { get; set; }
		public string additional_data { get; set; }
		public List< string > additional_information { get; set; }
		public string address_status { get; set; }
		public int? amount_authorized { get; set; }
		public int? amount_canceled { get; set; }
		public int? amount_ordered { get; set; }
		public int? amount_paid { get; set; }
		public int? amount_refunded { get; set; }
		public string anet_trans_method { get; set; }
		public int? base_amount_authorized { get; set; }
		public int? base_amount_canceled { get; set; }
		public int? base_amount_ordered { get; set; }
		public int? base_amount_paid { get; set; }
		public int? base_amount_paid_online { get; set; }
		public int? base_amount_refunded { get; set; }
		public int? base_amount_refunded_online { get; set; }
		public int? base_shipping_amount { get; set; }
		public int? base_shipping_captured { get; set; }
		public int? base_shipping_refunded { get; set; }
		public string cc_approval { get; set; }
		public string cc_avs_status { get; set; }
		public string cc_cid_status { get; set; }
		public string cc_debug_request_body { get; set; }
		public string cc_debug_response_body { get; set; }
		public string cc_debug_response_serialized { get; set; }
		public string cc_exp_month { get; set; }
		public string cc_exp_year { get; set; }
		public string cc_last4 { get; set; }
		public string cc_number_enc { get; set; }
		public string cc_owner { get; set; }
		public string cc_secure_verify { get; set; }
		public string cc_ss_issue { get; set; }
		public string cc_ss_start_month { get; set; }
		public string cc_ss_start_year { get; set; }
		public string cc_status { get; set; }
		public string cc_status_description { get; set; }
		public string cc_trans_id { get; set; }
		public string cc_type { get; set; }
		public string echeck_account_name { get; set; }
		public string echeck_account_type { get; set; }
		public string echeck_bank_name { get; set; }
		public string echeck_routing_number { get; set; }
		public string echeck_type { get; set; }
		public int? entity_id { get; set; }
		public string last_trans_id { get; set; }
		public string method { get; set; }
		public int? parent_id { get; set; }
		public string po_number { get; set; }
		public string protection_eligibility { get; set; }
		public int? quote_payment_id { get; set; }
		public int? shipping_amount { get; set; }
		public int? shipping_captured { get; set; }
		public int? shipping_refunded { get; set; }
		public ExtensionAttributes8 extension_attributes { get; set; }
	}

	public class ExtensionAttributes9
	{
	}

	public class StatusHistory
	{
		public string comment { get; set; }
		public string created_at { get; set; }
		public int? entity_id { get; set; }
		public string entity_name { get; set; }
		public int? is_customer_notified { get; set; }
		public int? is_visible_on_front { get; set; }
		public int? parent_id { get; set; }
		public string status { get; set; }
		public ExtensionAttributes9 extension_attributes { get; set; }
	}

	public class ExtensionAttributes11
	{
	}

	public class Address
	{
		public string address_type { get; set; }
		public string city { get; set; }
		public string company { get; set; }
		public string country_id { get; set; }
		public int? customer_address_id { get; set; }
		public int? customer_id { get; set; }
		public string email { get; set; }
		public int? entity_id { get; set; }
		public string fax { get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public string middlename { get; set; }
		public int? parent_id { get; set; }
		public string postcode { get; set; }
		public string prefix { get; set; }
		public string region { get; set; }
		public string region_code { get; set; }
		public int? region_id { get; set; }
		public List< string > street { get; set; }
		public string suffix { get; set; }
		public string telephone { get; set; }
		public string vat_id { get; set; }
		public int? vat_is_valid { get; set; }
		public string vat_request_date { get; set; }
		public string vat_request_id { get; set; }
		public int? vat_request_success { get; set; }
		public ExtensionAttributes11 extension_attributes { get; set; }
	}

	public class ExtensionAttributes12
	{
	}

	public class Total
	{
		public int? base_shipping_amount { get; set; }
		public int? base_shipping_canceled { get; set; }
		public int? base_shipping_discount_amount { get; set; }
		public int? base_shipping_discount_tax_compensation_amnt { get; set; }
		public int? base_shipping_incl_tax { get; set; }
		public int? base_shipping_invoiced { get; set; }
		public int? base_shipping_refunded { get; set; }
		public int? base_shipping_tax_amount { get; set; }
		public int? base_shipping_tax_refunded { get; set; }
		public int? shipping_amount { get; set; }
		public int? shipping_canceled { get; set; }
		public int? shipping_discount_amount { get; set; }
		public int? shipping_discount_tax_compensation_amount { get; set; }
		public int? shipping_incl_tax { get; set; }
		public int? shipping_invoiced { get; set; }
		public int? shipping_refunded { get; set; }
		public int? shipping_tax_amount { get; set; }
		public int? shipping_tax_refunded { get; set; }
		public ExtensionAttributes12 extension_attributes { get; set; }
	}

	public class ExtensionAttributes13
	{
	}

	public class Shipping
	{
		public Address address { get; set; }
		public string method { get; set; }
		public Total total { get; set; }
		public ExtensionAttributes13 extension_attributes { get; set; }
	}

	public class ParentItem2
	{
	}

	public class FileInfo2
	{
		public string base64_encoded_data { get; set; }
		public string type { get; set; }
		public string name { get; set; }
	}

	public class ExtensionAttributes15
	{
		public FileInfo2 file_info { get; set; }
	}

	public class CustomOption2
	{
		public string option_id { get; set; }
		public string option_value { get; set; }
		public ExtensionAttributes15 extension_attributes { get; set; }
	}

	public class ExtensionAttributes16
	{
	}

	public class BundleOption2
	{
		public int? option_id { get; set; }
		public int? option_qty { get; set; }
		public List< int? > option_selections { get; set; }
		public ExtensionAttributes16 extension_attributes { get; set; }
	}

	public class DownloadableOption2
	{
		public List< int? > downloadable_links { get; set; }
	}

	public class ExtensionAttributes17
	{
	}

	public class ConfigurableItemOption2
	{
		public string option_id { get; set; }
		public int? option_value { get; set; }
		public ExtensionAttributes17 extension_attributes { get; set; }
	}

	public class ExtensionAttributes14
	{
		public List< CustomOption2 > custom_options { get; set; }
		public List< BundleOption2 > bundle_options { get; set; }
		public DownloadableOption2 downloadable_option { get; set; }
		public List< ConfigurableItemOption2 > configurable_item_options { get; set; }
	}

	public class ProductOption2
	{
		public ExtensionAttributes14 extension_attributes { get; set; }
	}

	public class ExtensionAttributes19
	{
		public string entity_id { get; set; }
		public string entity_type { get; set; }
	}

	public class GiftMessage2
	{
		public int? gift_message_id { get; set; }
		public int? customer_id { get; set; }
		public string sender { get; set; }
		public string recipient { get; set; }
		public string message { get; set; }
		public ExtensionAttributes19 extension_attributes { get; set; }
	}

	public class ExtensionAttributes18
	{
		public GiftMessage2 gift_message { get; set; }
	}

	public class Item3
	{
		public string additional_data { get; set; }
		public int? amount_refunded { get; set; }
		public string applied_rule_ids { get; set; }
		public int? base_amount_refunded { get; set; }
		public int? base_cost { get; set; }
		public int? base_discount_amount { get; set; }
		public int? base_discount_invoiced { get; set; }
		public int? base_discount_refunded { get; set; }
		public int? base_discount_tax_compensation_amount { get; set; }
		public int? base_discount_tax_compensation_invoiced { get; set; }
		public int? base_discount_tax_compensation_refunded { get; set; }
		public int? base_original_price { get; set; }
		public int? base_price { get; set; }
		public int? base_price_incl_tax { get; set; }
		public int? base_row_invoiced { get; set; }
		public int? base_row_total { get; set; }
		public int? base_row_total_incl_tax { get; set; }
		public int? base_tax_amount { get; set; }
		public int? base_tax_before_discount { get; set; }
		public int? base_tax_invoiced { get; set; }
		public int? base_tax_refunded { get; set; }
		public int? base_weee_tax_applied_amount { get; set; }
		public int? base_weee_tax_applied_row_amnt { get; set; }
		public int? base_weee_tax_disposition { get; set; }
		public int? base_weee_tax_row_disposition { get; set; }
		public string created_at { get; set; }
		public string description { get; set; }
		public int? discount_amount { get; set; }
		public int? discount_invoiced { get; set; }
		public int? discount_percent { get; set; }
		public int? discount_refunded { get; set; }
		public int? event_id { get; set; }
		public string ext_order_item_id { get; set; }
		public int? free_shipping { get; set; }
		public int? gw_base_price { get; set; }
		public int? gw_base_price_invoiced { get; set; }
		public int? gw_base_price_refunded { get; set; }
		public int? gw_base_tax_amount { get; set; }
		public int? gw_base_tax_amount_invoiced { get; set; }
		public int? gw_base_tax_amount_refunded { get; set; }
		public int? gw_id { get; set; }
		public int? gw_price { get; set; }
		public int? gw_price_invoiced { get; set; }
		public int? gw_price_refunded { get; set; }
		public int? gw_tax_amount { get; set; }
		public int? gw_tax_amount_invoiced { get; set; }
		public int? gw_tax_amount_refunded { get; set; }
		public int? discount_tax_compensation_amount { get; set; }
		public int? discount_tax_compensation_canceled { get; set; }
		public int? discount_tax_compensation_invoiced { get; set; }
		public int? discount_tax_compensation_refunded { get; set; }
		public int? is_qty_decimal { get; set; }
		public int? is_virtual { get; set; }
		public int? item_id { get; set; }
		public int? locked_do_invoice { get; set; }
		public int? locked_do_ship { get; set; }
		public string name { get; set; }
		public int? no_discount { get; set; }
		public int? order_id { get; set; }
		public int? original_price { get; set; }
		public int? parent_item_id { get; set; }
		public int? price { get; set; }
		public int? price_incl_tax { get; set; }
		public int? product_id { get; set; }
		public string product_type { get; set; }
		public int? qty_backordered { get; set; }
		public int? qty_canceled { get; set; }
		public int? qty_invoiced { get; set; }
		public int? qty_ordered { get; set; }
		public int? qty_refunded { get; set; }
		public int? qty_returned { get; set; }
		public int? qty_shipped { get; set; }
		public int? quote_item_id { get; set; }
		public int? row_invoiced { get; set; }
		public int? row_total { get; set; }
		public int? row_total_incl_tax { get; set; }
		public int? row_weight { get; set; }
		public string sku { get; set; }
		public int? store_id { get; set; }
		public int? tax_amount { get; set; }
		public int? tax_before_discount { get; set; }
		public int? tax_canceled { get; set; }
		public int? tax_invoiced { get; set; }
		public int? tax_percent { get; set; }
		public int? tax_refunded { get; set; }
		public string updated_at { get; set; }
		public string weee_tax_applied { get; set; }
		public int? weee_tax_applied_amount { get; set; }
		public int? weee_tax_applied_row_amount { get; set; }
		public int? weee_tax_disposition { get; set; }
		public int? weee_tax_row_disposition { get; set; }
		public int? weight { get; set; }
		public ParentItem2 parent_item { get; set; }
		public ProductOption2 product_option { get; set; }
		public ExtensionAttributes18 extension_attributes { get; set; }
	}

	public class ExtensionAttributes20
	{
	}

	public class ShippingAssignment
	{
		public Shipping shipping { get; set; }
		public List< Item3 > items { get; set; }
		public int? stock_id { get; set; }
		public ExtensionAttributes20 extension_attributes { get; set; }
	}

	public class ExtensionAttributes21
	{
		public string entity_id { get; set; }
		public string entity_type { get; set; }
	}

	public class GiftMessage3
	{
		public int? gift_message_id { get; set; }
		public int? customer_id { get; set; }
		public string sender { get; set; }
		public string recipient { get; set; }
		public string message { get; set; }
		public ExtensionAttributes21 extension_attributes { get; set; }
	}

	public class ExtensionAttributes23
	{
	}

	public class Rate
	{
		public string code { get; set; }
		public string title { get; set; }
		public int? percent { get; set; }
		public ExtensionAttributes23 extension_attributes { get; set; }
	}

	public class ExtensionAttributes22
	{
		public List< Rate > rates { get; set; }
	}

	public class AppliedTax
	{
		public string code { get; set; }
		public string title { get; set; }
		public int? percent { get; set; }
		public int? amount { get; set; }
		public int? base_amount { get; set; }
		public ExtensionAttributes22 extension_attributes { get; set; }
	}

	public class ExtensionAttributes25
	{
	}

	public class Rate2
	{
		public string code { get; set; }
		public string title { get; set; }
		public int? percent { get; set; }
		public ExtensionAttributes25 extension_attributes { get; set; }
	}

	public class ExtensionAttributes24
	{
		public List< Rate2 > rates { get; set; }
	}

	public class AppliedTax2
	{
		public string code { get; set; }
		public string title { get; set; }
		public int? percent { get; set; }
		public int? amount { get; set; }
		public int? base_amount { get; set; }
		public ExtensionAttributes24 extension_attributes { get; set; }
	}

	public class ExtensionAttributes26
	{
	}

	public class ItemAppliedTax
	{
		public string type { get; set; }
		public int? item_id { get; set; }
		public int? associated_item_id { get; set; }
		public List< AppliedTax2 > applied_taxes { get; set; }
		public ExtensionAttributes26 extension_attributes { get; set; }
	}

	public class ExtensionAttributes10
	{
		public List< ShippingAssignment > shipping_assignments { get; set; }
		public GiftMessage3 gift_message { get; set; }
		public List< AppliedTax > applied_taxes { get; set; }
		public List< ItemAppliedTax > item_applied_taxes { get; set; }
		public bool converting_from_quote { get; set; }
	}

	[ JsonConverter( typeof( LaxPropertyNameMatchingConverter ) ) ]
	public class Item
	{
		public int? adjustment_negative { get; set; }
		public int? adjustment_positive { get; set; }
		public string applied_rule_ids { get; set; }
		public int? base_adjustment_negative { get; set; }
		public int? base_adjustment_positive { get; set; }
		public string base_currency_code { get; set; }
		public int? base_discount_amount { get; set; }
		public int? base_discount_canceled { get; set; }
		public int? base_discount_invoiced { get; set; }
		public int? base_discount_refunded { get; set; }
		public decimal? base_grand_total { get; set; }
		public int? base_discount_tax_compensation_amount { get; set; }
		public int? base_discount_tax_compensation_invoiced { get; set; }
		public int? base_discount_tax_compensation_refunded { get; set; }
		public decimal? base_shipping_amount { get; set; }
		public int? base_shipping_canceled { get; set; }
		public int? base_shipping_discount_amount { get; set; }
		public int? base_shipping_discount_tax_compensation_amnt { get; set; }
		public int? base_shipping_incl_tax { get; set; }
		public int? base_shipping_invoiced { get; set; }
		public int? base_shipping_refunded { get; set; }
		public int? base_shipping_tax_amount { get; set; }
		public int? base_shipping_tax_refunded { get; set; }
		public decimal? base_subtotal { get; set; }
		public int? base_subtotal_canceled { get; set; }
		public decimal? base_subtotal_incl_tax { get; set; }
		public int? base_subtotal_invoiced { get; set; }
		public int? base_subtotal_refunded { get; set; }
		public int? base_tax_amount { get; set; }
		public int? base_tax_canceled { get; set; }
		public int? base_tax_invoiced { get; set; }
		public int? base_tax_refunded { get; set; }
		public int? base_total_canceled { get; set; }
		public decimal? base_total_due { get; set; }
		public int? base_total_invoiced { get; set; }
		public int? base_total_invoiced_cost { get; set; }
		public int? base_total_offline_refunded { get; set; }
		public int? base_total_online_refunded { get; set; }
		public decimal? base_total_paid { get; set; }
		public int? base_total_qty_ordered { get; set; }
		public decimal? base_total_refunded { get; set; }
		public int? base_to_global_rate { get; set; }
		public int? base_to_order_rate { get; set; }
		public int? billing_address_id { get; set; }
		public int? can_ship_partially { get; set; }
		public int? can_ship_partially_item { get; set; }
		public string coupon_code { get; set; }
		public string created_at { get; set; }
		public string customer_dob { get; set; }
		public string customer_email { get; set; }
		public string customer_firstname { get; set; }
		public int? customer_gender { get; set; }
		public int? customer_group_id { get; set; }
		public int? customer_id { get; set; }
		public int? customer_is_guest { get; set; }
		public string customer_lastname { get; set; }
		public string customer_middlename { get; set; }
		public string customer_note { get; set; }
		public int? customer_note_notify { get; set; }
		public string customer_prefix { get; set; }
		public string customer_suffix { get; set; }
		public string customer_taxvat { get; set; }
		public int? discount_amount { get; set; }
		public int? discount_canceled { get; set; }
		public string discount_description { get; set; }
		public int? discount_invoiced { get; set; }
		public int? discount_refunded { get; set; }
		public int? edit_increment { get; set; }
		public int? email_sent { get; set; }
		public int? entity_id { get; set; }
		public string ext_customer_id { get; set; }
		public string ext_order_id { get; set; }
		public int? forced_shipment_with_invoice { get; set; }
		public string global_currency_code { get; set; }
		public decimal? grand_total { get; set; }
		public int? discount_tax_compensation_amount { get; set; }
		public int? discount_tax_compensation_invoiced { get; set; }
		public int? discount_tax_compensation_refunded { get; set; }
		public string hold_before_state { get; set; }
		public string hold_before_status { get; set; }
		public string increment_id { get; set; }
		public int? is_virtual { get; set; }
		public string order_currency_code { get; set; }
		public string original_increment_id { get; set; }
		public int? payment_authorization_amount { get; set; }
		public int? payment_auth_expiration { get; set; }
		public string protect_code { get; set; }
		public int? quote_address_id { get; set; }
		public int? quote_id { get; set; }
		public string relation_child_id { get; set; }
		public string relation_child_real_id { get; set; }
		public string relation_parent_id { get; set; }
		public string relation_parent_real_id { get; set; }
		public string remote_ip { get; set; }
		public decimal? shipping_amount { get; set; }
		public int? shipping_canceled { get; set; }
		public string shipping_description { get; set; }
		public int? shipping_discount_amount { get; set; }
		public int? shipping_discount_tax_compensation_amount { get; set; }
		public int? shipping_incl_tax { get; set; }
		public int? shipping_invoiced { get; set; }
		public int? shipping_refunded { get; set; }
		public int? shipping_tax_amount { get; set; }
		public int? shipping_tax_refunded { get; set; }
		public string state { get; set; }
		public string status { get; set; }
		public string store_currency_code { get; set; }
		public int? store_id { get; set; }
		public string store_name { get; set; }
		public int? store_to_base_rate { get; set; }
		public int? store_to_order_rate { get; set; }
		public decimal? subtotal { get; set; }
		public int? subtotal_canceled { get; set; }
		public decimal? subtotal_incl_tax { get; set; }
		public int? subtotal_invoiced { get; set; }
		public int? subtotal_refunded { get; set; }
		public int? tax_amount { get; set; }
		public int? tax_canceled { get; set; }
		public int? tax_invoiced { get; set; }
		public int? tax_refunded { get; set; }
		public int? total_canceled { get; set; }
		public decimal? total_due { get; set; }
		public int? total_invoiced { get; set; }
		public int? total_item_count { get; set; }
		public int? total_offline_refunded { get; set; }
		public int? total_online_refunded { get; set; }
		public decimal? total_paid { get; set; }
		public int? total_qty_ordered { get; set; }
		public decimal? total_refunded { get; set; }
		public string updated_at { get; set; }
		public int? weight { get; set; }
		public string x_forwarded_for { get; set; }
		public List< Item2 > items { get; set; }
		public BillingAddress billing_address { get; set; }
		public Payment payment { get; set; }
		public List< StatusHistory > status_histories { get; set; }
		public ExtensionAttributes10 extension_attributes { get; set; }
	}

	public class RootObject
	{
		public List< Item > items { get; set; }
		public SearchCriteria search_criteria { get; set; }
		public int total_count { get; set; }
	}
}