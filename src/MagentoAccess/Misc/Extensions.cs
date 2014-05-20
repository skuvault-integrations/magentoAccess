using System;
using System.Globalization;
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

		public static DateTime ToDateTime( this string srcString )
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

		public static decimal ToDecimalDotOrComaSeparated( this string srcString )
		{
			decimal parsedNumber;

			if( string.IsNullOrWhiteSpace( srcString ) )
				return default( decimal );

			try
			{
				parsedNumber = decimal.Parse( srcString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture );
			}
			catch( Exception )
			{
				parsedNumber = decimal.Parse( srcString, new NumberFormatInfo { NumberDecimalSeparator = "," } );
			}

			return parsedNumber;
		}
	}
}