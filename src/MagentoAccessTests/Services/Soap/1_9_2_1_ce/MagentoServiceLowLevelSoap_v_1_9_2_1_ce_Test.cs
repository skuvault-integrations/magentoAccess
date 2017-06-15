using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess.Services.Soap._1_9_2_1_ce;
using Netco.Extensions;
using NUnit.Framework;

namespace MagentoAccessTests.Services.Soap._1_9_2_1_ce
{
	[ TestFixture ]
	internal class MagentoServiceLowLevelSoap_v_1_9_2_1_ce_Test
	{
		[ Test ]
		public void GetSessionId_CalledSimultaneously_Only1ApiCallRequested()
		{
			//A
			var apiCallsCount = 0;
			var magentoServiceLowLevelSoapV1921Ce = new MagentoServiceLowLevelSoap_v_1_9_2_1_ce( "qwe", "qwe", "https://magento.com", "1", 30, true, 300000 )
			{
				PullSessionId = async ctx =>
				{
					await Task.Delay( 1000 ).ConfigureAwait(false);
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
			var getSessionTasks = list.ProcessInBatchAsync( tasksCount, async x => await magentoServiceLowLevelSoapV1921Ce.GetSessionId( CancellationToken.None, false ).ConfigureAwait( false ) );
			getSessionTasks.Wait();

			//A
			apiCallsCount.Should().Be( 1 );
		}
	}
}