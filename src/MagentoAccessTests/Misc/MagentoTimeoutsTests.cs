using FluentAssertions;
using MagentoAccess.Misc;
using NUnit.Framework;

namespace MagentoAccessTests.Misc
{
	[ TestFixture ]
	public class MagentoTimeoutsTests
	{
		[ Test ]
		public void GivenSpecificTimeoutsAreNotSet_WhenGetTimeoutIsCalled_ThenDefaultTimeoutIsReturned()
		{
			var operationsTimeouts = new MagentoTimeouts();

			operationsTimeouts[ MagentoOperationEnum.GetModifiedOrders ].Should().Be( operationsTimeouts.DefaultOperationTimeout.TimeoutInMs );
		}

		[ Test ]
		public void GivenOwnDefaultTimeoutValue_WhenGetTimeoutIsCalled_ThenOverridenDefaultTimeoutIsReturned()
		{
			var newDefaultTimeoutInMs = 10 * 60 * 1000;
			var operationsTimeouts = new MagentoTimeouts( newDefaultTimeoutInMs );

			operationsTimeouts[ MagentoOperationEnum.GetProductBySku ].Should().Be( newDefaultTimeoutInMs );
		}

		[ Test ]
		public void GivenGetModifiedOrdersTimeoutIsSet_WhenGetTimeoutIsCalled_ThenSpecificTimeoutIsReturned()
		{
			var operationsTimeouts = new MagentoTimeouts();
			var specificTimeoutInMs = 10 * 60 * 1000;
			operationsTimeouts.Set( MagentoOperationEnum.GetModifiedOrders, new MagentoOperationTimeout( specificTimeoutInMs ) );

			operationsTimeouts[ MagentoOperationEnum.GetModifiedOrders ].Should().Be( specificTimeoutInMs );
			operationsTimeouts[ MagentoOperationEnum.GetProductBySku ].Should().Be( operationsTimeouts.DefaultOperationTimeout.TimeoutInMs );
		}

		[ Test ]
		public void GivenGetModifiedOrdersTimeoutIsSetTwice_WhenGetTimeoutIsCalled_ThenSpecificTimeoutIsReturned()
		{
			var operationsTimeouts = new MagentoTimeouts();
			var specificTimeoutInMs = 10 * 60 * 1000;
			operationsTimeouts.Set( MagentoOperationEnum.GetModifiedOrders, new MagentoOperationTimeout( specificTimeoutInMs ) );
			operationsTimeouts.Set( MagentoOperationEnum.GetModifiedOrders, new MagentoOperationTimeout( specificTimeoutInMs * 2 ) );

			operationsTimeouts[ MagentoOperationEnum.GetModifiedOrders ].Should().Be( specificTimeoutInMs * 2 );
		}
	}
}