using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml;
using MagentoAccess.Models.GetOrders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagentoAccess.Misc
{
	internal static class Extensions
	{
		public static T WaitResult< T >( this Task< T > t )
		{
			t.Wait();
			return t.Result;
		}

		public static string ToStringEmptyOnNull( this int? integer, CultureInfo culture )
		{
			return integer?.ToString( culture ) ?? string.Empty;
		}

		public static string ToStringEmptyOnNull( this int? integer )
		{
			return ToStringEmptyOnNull( integer, CultureInfo.InvariantCulture );
		}
		public static string ToStringEmptyOnNull( this string str, CultureInfo culture )
		{
			return ( str ?? string.Empty ).ToString( culture );
		}
		public static string ToStringUtcIso8601( this DateTime dateTime )
		{
			var universalTime = dateTime.ToUniversalTime();
			var result = XmlConvert.ToString( universalTime, XmlDateTimeSerializationMode.RoundtripKind );
			return result;
		}

		public static string ToUrlParameterString( this DateTime dateTime )
		{
			var strRes = XmlConvert.ToString( dateTime, "yyyy-MM-ddTHH:mm:ss" );
			var result = strRes.Replace( "T", "%20" );
			return result;
		}

		public static string ToSoapParameterString( this DateTime dateTime )
		{
			var strRes = XmlConvert.ToString( dateTime, "yyyy-MM-ddTHH:mm:ss" );
			var result = strRes.Replace( "T", " " );
			return result;
		}

		public static string ToRestParameterString( this DateTime dateTime )
		{
			var strRes = dateTime.ToString( "yyyy-MM-dd HH:mm:ss" );
			return strRes;
		}

		public static DateTime ToDateTimeOrDefault( this string srcString )
		{
			try
			{
				if( string.IsNullOrWhiteSpace( srcString ) )
					return default(DateTime);
				var dateTime = DateTime.Parse( srcString, CultureInfo.InvariantCulture );
				return dateTime;
			}
			catch
			{
				return default(DateTime);
			}
		}

		public static long ToLongOrDefault( this string srcString )
		{
			try
			{
				var dateTime = long.Parse( srcString, CultureInfo.InvariantCulture );
				return dateTime;
			}
			catch
			{
				return default(long);
			}
		}

		public static decimal ToDecimalOrDefault( this string srcString )
		{
			if( string.IsNullOrWhiteSpace( srcString ) )
				return default(decimal);

			var parsedNumber = default(decimal);

			if( decimal.TryParse( srcString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out parsedNumber ) )
				return parsedNumber;
			else if( decimal.TryParse( srcString.Replace( ",", "." ), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out parsedNumber ) )
				return parsedNumber;

			return parsedNumber;
		}

		public static bool ToDecimalOrDefault( this string srcString, out decimal parsedNumber )
		{
			parsedNumber = default(decimal);
			if( string.IsNullOrWhiteSpace( srcString ) )
				return false;

			if( decimal.TryParse( srcString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out parsedNumber ) )
				return true;
			else if( decimal.TryParse( srcString.Replace( ",", "." ), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out parsedNumber ) )
				return true;

			return false;
		}

		public static double ToDoubleOrDefault( this string srcString )
		{
			if( string.IsNullOrWhiteSpace( srcString ) )
				return default(double);

			double parsedNumber;

			if( double.TryParse( srcString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out parsedNumber ) )
				return parsedNumber;
			else if( double.TryParse( srcString.Replace( ",", "." ), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out parsedNumber ) )
				return parsedNumber;

			return parsedNumber;
		}

		public static T DeepClone< T >( this T obj )
		{
			using( var ms = new MemoryStream() )
			{
				var formstter = new BinaryFormatter();
				formstter.Serialize( ms, obj );
				ms.Position = 0;
				return ( T )formstter.Deserialize( ms );
			}
		}

		public static string BuildUrl( this IEnumerable< string > urlParrts, bool escapeUrl = false, bool trimTailsSlash = false )
		{
			var resultUrl = string.Empty;
			try
			{
				resultUrl = urlParrts.Aggregate( ( ac, x ) =>
				{
					string result;

					if( !string.IsNullOrWhiteSpace( ac ) )
						ac = ac.EndsWith( "/" ) ? ac : ac + "/";

					if( !string.IsNullOrWhiteSpace( x ) )
					{
						x = x.EndsWith( "/" ) ? x : x + "/";
						x = x.StartsWith( "/" ) ? x.TrimStart( '/' ) : x;

						if( escapeUrl )
							result = string.IsNullOrWhiteSpace( ac ) ? new Uri( x ).AbsoluteUri : new Uri( new Uri( ac ), x ).AbsoluteUri;
						else
							result = string.IsNullOrWhiteSpace( ac ) ? x : string.Format( "{0}{1}", ac, x ); // new Uri(new Uri(ac), x).AbsoluteUri;
					}
					else
					{
						if( escapeUrl )
							result = string.IsNullOrWhiteSpace( ac ) ? string.Empty : new Uri( ac ).AbsoluteUri;
						else
							result = string.IsNullOrWhiteSpace( ac ) ? string.Empty : ac;
					}

					if( trimTailsSlash )
						result = result.TrimEnd( '/' );
					return result;
				} );
			}
			catch
			{
			}

			return resultUrl;
		}

		public static List< List< T > > SplitToChunks< T >( this List< T > source, int chunkSize )
		{
			var i = 0;
			var chunks = new List< List< T > >();
			while( i < source.Count() )
			{
				var temp = source.Skip( i ).Take( chunkSize ).ToList();
				chunks.Add( temp );
				i += chunkSize;
			}
			return chunks;
		}

		public static string ToJsonAsParallel( this IEnumerable< Order > source, int from, int take )
		{
			var orders = source as IList< Order > ?? source.ToList();
			var objects = orders.Skip( from ).Take( take ).AsParallel().Select( x => string.Format( "{{id:{0},createdAt:{1}}}", string.IsNullOrWhiteSpace( x.OrderIncrementalId ) ? PredefinedValues.NotAvailable : x.OrderIncrementalId, x.CreatedAt ) );
			var items = string.Join( ",", objects );
			var res = $"{{Count:{objects.Count()}, Items:[{items}]}}";
			return res;
		}

		public static string ToJson( this object source )
		{
			try
			{
				if( source == null )
					return PredefinedValues.EmptyJsonObject;
				else
				{
					var sourceConverted = source as IManualSerializable;
					if( sourceConverted != null )
						return sourceConverted.SerializeToJson();
					else
					{
						var serialized = JsonConvert.SerializeObject( source, new IsoDateTimeConverter() );
						return serialized;
					}
				}
			}
			catch( Exception ex)
			{
				return $"{{can't serialize to Json {ex} }}";
			}
		}

		public static IEnumerable< IEnumerable< T > > Batch< T >(
			this IEnumerable< T > source, int batchSize )
		{
			using( var enumerator = source.GetEnumerator() )
			{
				while( enumerator.MoveNext() )
					yield return YieldBatchElements( enumerator, batchSize - 1 ).ToList();
			}
		}

		private static IEnumerable< T > YieldBatchElements< T >(
			IEnumerator< T > source, int batchSize )
		{
			yield return source.Current;
			for( var i = 0; i < batchSize && source.MoveNext(); i++ )
			{
				yield return source.Current;
			}
		}
	}

}