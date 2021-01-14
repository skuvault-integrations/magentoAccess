using System.Linq;
using FluentAssertions;
using MagentoAccess.Models.Services.Soap.GetOrders;
using NUnit.Framework;
using MagentoAccess.TsZoey_v_1_9_0_1_CE;

namespace MagentoAccessTests.Models.GetOrders.Zoey_1_7_to_1_9_CE
{
	[ TestFixture ]
	public class ZoeyOrdersExtensionsTests
	{
		[ Test ]
		public void ToOrderItemEntities_ThenOrderItemCreated()
		{
			var orderInvoiceDiscount = "1.23";
			var sku = "testSku1";
			var qty = "3";
			var price = "11.23";
			var taxAmount = "2.34";
			salesOrderInvoiceEntity[] orderInvoices = new []
			{
				new salesOrderInvoiceEntity
				{
					items = new []
					{
						new salesOrderInvoiceItemEntity 
						{ 
							sku = sku, 
							qty = qty,
							price = price,
							tax_amount = taxAmount
						}
					},
					discount_amount = orderInvoiceDiscount
				},
				new salesOrderInvoiceEntity
				{
					items = new [] { new salesOrderInvoiceItemEntity() }
				}
			};

			var results = orderInvoices.ToOrderItemEntities();

			results.Count().Should().Be( orderInvoices.Length );
			var resultsFirst = results.First();
			resultsFirst.Sku.Should().Be( sku );
			resultsFirst.QtyOrdered.Should().Be( qty );
			resultsFirst.Price.Should().Be( price );
			resultsFirst.DiscountAmount.Should().Be( orderInvoiceDiscount );
			resultsFirst.TaxAmount.Should().Be( taxAmount );
		}

		[ Test ]
		public void ToOrderItemEntities_WhenOrderInvoicesIsNull_ThenReturnsEmpty()
		{ 
			salesOrderInvoiceEntity[] orderInvoices = null;

			var results = orderInvoices.ToOrderItemEntities();

			results.Should().BeEmpty();
		}

		[ Test ]
		public void ToOrderItemEntities_WhenMultipleInvoicesItemsPerInvoice_ThenItemDiscountIsEmptyString()
		{ 
			salesOrderInvoiceEntity[] orderInvoices = new []
			{
				new salesOrderInvoiceEntity
				{
					items = new []
					{
						new salesOrderInvoiceItemEntity { sku = "testSku1" },
						new salesOrderInvoiceItemEntity { sku = "testSku2" }
					},
					discount_amount = "1.23"
				}
			};

			var results = orderInvoices.ToOrderItemEntities();

			results.First().DiscountAmount.Should().BeEmpty();
		}
	}
}
