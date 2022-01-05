using System;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using Netco.ActionPolicyServices;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	internal static class ActionPolicies
	{
		//TODO GUARD-2311 Option 3: Replace all calls to RepeatOnChannelProblemAsync with calls to RepeatOnAuthProblemAsync and delete this file
		public static ActionPolicyAsync RepeatOnChannelProblemAsync { get; } = ActionPolicyAsync.Handle< Exception >()
			.RetryAsync( 7, async ( ex, i ) =>
			{
				MagentoLogger.Log().Trace( ex, "Retrying Magento API call due to channel problem for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );
	}
}