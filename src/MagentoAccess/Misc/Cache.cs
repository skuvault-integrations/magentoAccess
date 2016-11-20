using System;
using System.Collections.Generic;

namespace MagentoAccess.Misc
{
	internal class Cache< TKey, TVal > where TVal : class
	{
		private readonly int cacheMaxLength;
		protected IDictionary< TKey, CacheRecord< TVal > > CachedDic { get; }

		public Cache( int maxLength = 1000 )
		{
			this.CachedDic = new Dictionary< TKey, CacheRecord< TVal > >();
			this.cacheMaxLength = maxLength;
		}

		public bool Add( TVal value, TKey key, TimeSpan lt )
		{
			if( this.CachedDic.Count < this.cacheMaxLength )
			{
				var cr = new CacheRecord< TVal >( value, lt );
				this.CachedDic[ key ] = cr;
				return true;
			}
			return false;
		}

		public TVal Get( TKey key )
		{
			if( this.CachedDic.ContainsKey( key ) )
			{
				var cacheRecord = this.CachedDic[ key ];
				if( cacheRecord.IsActual() )
					return cacheRecord.Value;
				else
				{
					this.CachedDic.Remove( key );
					return null;
				}
			}
			return null;
		}
	}

	internal class CacheRecord< T >
	{
		public T Value { get; }

		public DateTime Born { get; }
		public TimeSpan Life { get; }

		public CacheRecord( T value, TimeSpan life )
		{
			this.Value = value;
			this.Born = DateTime.UtcNow;
			this.Life = life;
		}

		protected CacheRecord( T value, DateTime born, TimeSpan life )
		{
			this.Value = value;
			this.Born = born;
			this.Life = life;
		}

		public bool IsActual()
		{
			return ( DateTime.UtcNow - this.Born ) < this.Life;
		}
	}
}