using System;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Logging;
using Netco.Utils;

namespace MagentoAccess.Misc
{
	internal static class ActionPolicies
	{
		private static readonly ActionPolicy _magentoSumbitPolicy = ActionPolicy.Handle< Exception >().Retry( 10, ( ex, i ) =>
		{
			typeof( ActionPolicies ).Log().Trace( ex, "Retrying Magento API submit call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.5 + i ) );
		} );

		private static readonly ActionPolicyAsync _magentoSumbitAsyncPolicy = ActionPolicyAsync.Handle< Exception >()
			.RetryAsync( 10, async ( ex, i ) =>
			{
				typeof( ActionPolicies ).Log().Trace( ex, "Retrying Magento API submit call for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );

		private static readonly ActionPolicy _magentoGetPolicy = ActionPolicy.Handle< Exception >().Retry( 10, ( ex, i ) =>
		{
			typeof( ActionPolicies ).Log().Trace( ex, "Retrying Magento API get call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.5 + i ) );
		} );

		private static readonly ActionPolicyAsync _magentoGetAsyncPolicy = ActionPolicyAsync.Handle< Exception >()
			.RetryAsync( 10, async ( ex, i ) =>
			{
				typeof( ActionPolicies ).Log().Trace( ex, "Retrying Magento API get call for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );

		public static ActionPolicy Submit
		{
			get { return _magentoSumbitPolicy; }
		}

		public static ActionPolicyAsync SubmitAsync
		{
			get { return _magentoSumbitAsyncPolicy; }
		}

		public static ActionPolicy Get
		{
			get { return _magentoGetPolicy; }
		}

		public static ActionPolicyAsync GetAsync
		{
			get { return _magentoGetAsyncPolicy; }
		}
	}
}