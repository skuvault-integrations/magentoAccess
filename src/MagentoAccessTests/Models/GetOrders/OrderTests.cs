using FluentAssertions;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.Services.Soap.GetOrders;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Order = MagentoAccess.Models.GetOrders.Order;

namespace MagentoAccessTests.Models.GetOrders
{
	[ TestFixture ]
	public class OrderTests
	{
		[ Test ]
		public void OrderConstructor_ParameterOrderInfoResponse_GivenNullItems_ShouldReturnEmptyItems()
		{
			var orderInfoResponse = new OrderInfoResponse();

			var result = new Order( orderInfoResponse );

			result.Items.Should().BeEmpty();
		}
	}
		
	public class OrderExtensionsTests
	{
		[ Test ]
		public void ToSvOrderItems_ParameterListOrderItemEntity_GivenNullItems_ShouldReturnEmptyItems()
		{
			List< OrderItemEntity > orderItems = null;

			var result = orderItems.ToSvOrderItems().ToList();

			result.Should().BeEmpty();
		}

		[ Test ]
		public void ToSvOrderItems_ParameterListOrderItemEntity_ShouldReturnCorrectItems()
		{
			var orderItem = new OrderItemEntity
			{
				BaseDiscountAmount = "1.23",
				BaseOriginalPrice = "2.34",
				Sku = "testSku23",
				Name = "Some product",
				BaseTaxAmount = "0.40",
				ItemId = "12345",
				BasePrice = "22.33",
				BaseRowTotal = "34.43",
				DiscountAmount = "43.34",
				OriginalPrice = "5.5",
				Price = "2.22",
				ProductType = "spoon",
				QtyCanceled = "123",
				QtyInvoiced = "3",
				QtyOrdered = "1233",
				QtyShipped = "7",
				QtyRefunded = "1",
				RowTotal = "2.34",
				TaxAmount = "0.01",
				TaxPercent = "6.01"
			};
			var orderItems = new List< OrderItemEntity >
			{
				orderItem,
				new OrderItemEntity()
			};

			var result = orderItems.ToSvOrderItems().ToList();

			result.Count.Should().Be( orderItems.Count );
			var resultFirst = result.First();
			resultFirst.BaseDiscountAmount.ToString().Should().Be( orderItem.BaseDiscountAmount );
			resultFirst.BaseOriginalPrice.ToString().Should().Be( orderItem.BaseOriginalPrice );
			resultFirst.Sku.Should().Be( orderItem.Sku );
			resultFirst.Name.Should().Be( orderItem.Name );
			resultFirst.BaseTaxAmount.ToString().Should().Be( orderItem.BaseTaxAmount );
			resultFirst.ItemId.Should().Be( orderItem.ItemId );
			resultFirst.BasePrice.ToString().Should().Be( orderItem.BasePrice );
			resultFirst.BaseRowTotal.ToString().Should().Be( orderItem.BaseRowTotal );
			resultFirst.DiscountAmount.ToString().Should().Be( orderItem.DiscountAmount );
			resultFirst.OriginalPrice.ToString().Should().Be( orderItem.OriginalPrice );
			resultFirst.Price.ToString().Should().Be( orderItem.Price );
			resultFirst.ProductType.Should().Be( orderItem.ProductType );
			resultFirst.QtyCanceled.ToString().Should().Be( orderItem.QtyCanceled );
			resultFirst.QtyInvoiced.ToString().Should().Be( orderItem.QtyInvoiced );
			resultFirst.QtyOrdered.ToString().Should().Be( orderItem.QtyOrdered );
			resultFirst.QtyShipped.ToString().Should().Be( orderItem.QtyShipped );
			resultFirst.QtyRefunded.ToString().Should().Be( orderItem.QtyRefunded );
			resultFirst.RowTotal.ToString().Should().Be( orderItem.RowTotal );
			resultFirst.TaxAmount.ToString().Should().Be( orderItem.TaxAmount );
			resultFirst.TaxPercent.ToString().Should().Be( orderItem.TaxPercent );
		}
	}
}
