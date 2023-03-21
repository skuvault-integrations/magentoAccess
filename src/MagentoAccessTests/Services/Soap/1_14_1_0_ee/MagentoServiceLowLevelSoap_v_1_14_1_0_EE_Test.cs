using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Services.Soap._1_14_1_0_ee;
using Netco.Extensions;
using NUnit.Framework;

namespace MagentoAccessTests.Services.Soap._1_14_1_0_ee
{
	[ TestFixture ]
	internal class MagentoServiceLowLevelSoap_v_1_14_1_0_EE_Test
	{
		[ Test ]
		public void GetSessionId_CalledSimultaneously_Only1ApiCallRequested()
		{
			//A
			var apiCallsCount = 0;
			var serviceLowLevelSoapV11410Ee = new MagentoServiceLowLevelSoap_v_1_14_1_0_EE( "qwe", "qwe", "https://magento.com", "", "", 
				300000, true, 30, new MagentoConfig() )
			{
				PullSessionId = async () =>
				{
					await Task.Delay( 1000 );
					Interlocked.Increment( ref apiCallsCount );
					return Tuple.Create( "qwe", DateTime.UtcNow );
				}
			};
			var list = new List< int >();
			const int tasksCount = 3000;
			for( var i = 0; i < tasksCount; list.Add( i++ ) )
			{
			}

			//A
			var getSessionTasks = list.ProcessInBatchAsync( tasksCount, async x => await serviceLowLevelSoapV11410Ee.GetSessionId( CancellationToken.None, false ).ConfigureAwait( false ) );
			getSessionTasks.Wait();

			//A
			apiCallsCount.Should().Be( 1 );
		}
	}
}