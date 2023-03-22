using System;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Logging;

namespace MagentoAccess.Misc
{
	internal static class ActionPolicies
	{
		/// <summary>
		///	Returns true if request with provided exception (all but PermanentRedirectException) should be retried
		/// </summary>
		private static readonly ExceptionHandler _transientExceptionHandler = delegate ( Exception exception )
		{
			return !exception.IsMagentoPermanentRedirectException();
		};

		private static readonly Func< Mark, ActionPolicyAsync > _magentoGetPolicyWithMarkAsync = mark => ActionPolicyAsync
			.With( _transientExceptionHandler )
			.RetryAsync( 7, async ( ex, i ) =>
			{
				MagentoLogger.Log().Trace( ex, "[magento]\t[{0}]\t[Retrying]\t[mark:{1}] Retrying Magento API get call for the {2} time.", 
					MagentoLogger.FileVersion, mark ?? Mark.Blank(), i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );
		
		public static ActionPolicyAsync GetAsync { get; } = ActionPolicyAsync
			.With( _transientExceptionHandler )
			.RetryAsync( 7, async ( ex, i ) =>
			{
				MagentoLogger.Log().Trace( ex, "Retrying Magento API get call for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );

		public static ActionPolicyAsync GetWithMarkAsync( Mark mark )
		{
			return _magentoGetPolicyWithMarkAsync( mark );
		}
	}
}