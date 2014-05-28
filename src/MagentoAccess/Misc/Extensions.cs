using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace MagentoAccess.Misc
{
	public static class Extensions
	{
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

		public static DateTime ToDateTimeOrDefault( this string srcString )
		{
			try
			{
				var dateTime = DateTime.Parse( srcString, CultureInfo.InvariantCulture );
				return dateTime;
			}
			catch
			{
				return default( DateTime );
			}
		}

		public static decimal ToDecimalOrDefault( this string srcString )
		{
			decimal parsedNumber;

			try
			{
				parsedNumber = decimal.Parse( srcString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture );
			}
			catch
			{
				try
				{
					parsedNumber = decimal.Parse( srcString, new NumberFormatInfo { NumberDecimalSeparator = "," } );
				}
				catch
				{
					parsedNumber = default( decimal );
				}
			}

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
	}
}