﻿using System;
using System.Threading.Tasks;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public abstract class BaseRepository
	{
		private DateTime _lastNetworkActivityTime = DateTime.UtcNow;

		/// <summary>
		///	This property can be used by the client to monitor the last access library's network activity time.
		/// </summary>
		public DateTime LastNetworkActivityTime
		{
			get { return _lastNetworkActivityTime; }
		}

		protected async Task< T > TrackNetworkActivityTime< T >( Func< Task< T > > funcToTrack )
		{
			T result;

			try
			{
				// before API call
				RefreshLastNetworkActivityTime();
				result = await funcToTrack().ConfigureAwait( false );
			}
			finally
			{
				// after getting response or exception
				RefreshLastNetworkActivityTime();
			}

			return result;
		}

		/// <summary>
		///	This method is used to update service's last network activity time.
		///	It's called every time before making API request to server or after handling the response.
		/// </summary>
		private  void RefreshLastNetworkActivityTime()
		{
			this._lastNetworkActivityTime = DateTime.UtcNow;
		}
	}
}
