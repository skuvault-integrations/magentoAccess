using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Services.Soap._2_0_2_0_ce;
using Netco.Extensions;
using NUnit.Framework;

namespace MagentoAccessTests.Services.Soap._2_0_2_0_ce
{
	[ TestFixture ]
	internal class MagentoServiceLowLevelSoap_v_2_0_2_0_ce_Test
	{
		[ Test ]
		public void GetSessionId_CalledSimultaneously_Only1ApiCallRequested()
		{
			//A
			var apiCallsCount = 0;
			var magentoServiceLowLevelSoapV2020Ce = new MagentoServiceLowLevelSoap_v_2_0_2_0_ce( "qwe", "qwe", "https://magento.com", "1" )
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
			var getSessionTasks = list.ProcessInBatchAsync( tasksCount, async x => await magentoServiceLowLevelSoapV2020Ce.GetSessionId( false ).ConfigureAwait( false ) );
			getSessionTasks.Wait();

			//A
			apiCallsCount.Should().Be( 1 );
		}
	}
}