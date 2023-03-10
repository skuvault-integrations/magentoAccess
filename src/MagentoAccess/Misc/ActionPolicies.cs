﻿using System;
using System.Threading.Tasks;
using MagentoAccess.Exceptions;
using Netco.ActionPolicyServices;
using Netco.Logging;
using Netco.Utils;

namespace MagentoAccess.Misc
{
	internal static class ActionPolicies
	{
		/// <summary>
		///	Returns true if request with provided exception (all but PermanentRedirectException) should be retried
		/// </summary>
		private static readonly ExceptionHandler _exceptionHandler = delegate ( Exception x )
		{
			return !( x is PermanentRedirectException );
		};

		private static readonly ActionPolicyAsync _magentoSumbitAsyncPolicy = ActionPolicyAsync.With( _exceptionHandler )
			.RetryAsync( 7, async ( ex, i ) =>
			{
				MagentoLogger.Log().Trace( ex, "Retrying Magento API submit call for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );

		private static readonly ActionPolicy _magentoGetPolicy = ActionPolicy.With( _exceptionHandler ).Retry( 7, ( ex, i ) =>
		{
			MagentoLogger.Log().Trace( ex, "Retrying Magento API get call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.5 + i ) );
		} );

		private static readonly ActionPolicyAsync _magentoGetAsyncPolicy = ActionPolicyAsync.With( _exceptionHandler )
			.RetryAsync( 7, async ( ex, i ) =>
			{
				MagentoLogger.Log().Trace( ex, "Retrying Magento API get call for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );

		private static readonly Func< Mark, ActionPolicyAsync > _magentoGetPolicyWithMarkAsync = mark => ActionPolicyAsync.With( _exceptionHandler )
			.RetryAsync( 7, async ( ex, i ) =>
			{
				MagentoLogger.Log().Trace( ex, "[magento]\t[{0}]\t[Retrying]\t[mark:{1}] Retrying Magento API get call for the {2} time.", MagentoLogger.FileVersion, mark ?? Mark.Blank(), i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );

		public static ActionPolicyAsync RepeatOnChannelProblemAsync { get; } = ActionPolicyAsync.With( _exceptionHandler )
			.RetryAsync( 7, async ( ex, i ) =>
			{
				MagentoLogger.Log().Trace( ex, "Retrying Magento API call due to channel problem for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
			} );

		public static ActionPolicy Submit { get; } = ActionPolicy.With( _exceptionHandler ).Retry( 7, ( ex, i ) =>
		{
			MagentoLogger.Log().Trace( ex, "Retrying Magento API submit call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.5 + i ) );
		} );

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

		public static ActionPolicyAsync GetWithMarkAsync( Mark mark )
		{
			return _magentoGetPolicyWithMarkAsync( mark );
		}
	}
}