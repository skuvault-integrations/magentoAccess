using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;
using Netco.Extensions;
using NUnit.Framework;

namespace MagentoAccessTests.Services.Soap._1_7_0_1_ce_1_9_0_1_ce
{
	[ TestFixture ]
	internal class MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE_Test
	{
		[ Test ]
		public void GetSessionId_CalledSimultaneously_Only1ApiCallRequested()
		{
			//A
			var apiCallsCount = 0;
			var magentoServiceLowLevelSoapVFrom17To19Ce = new MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE( "qwe", "qwe", "https://magento.com", "1", 300000, 30, true )
			{
				PullSessionId = async ctx=>
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
			var getSessionTasks = list.ProcessInBatchAsync( tasksCount, async x => await magentoServiceLowLevelSoapVFrom17To19Ce.GetSessionId( CancellationToken.None, false ).ConfigureAwait( false ) );
			getSessionTasks.Wait();

			//A
			apiCallsCount.Should().Be( 1 );
		}
	}
}