using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.PutStockItems;
using MagentoAccess.Services;

namespace MagentoAccess.Misc
{
	internal static class Extensions
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

		public static string ToSoapParameterString( this DateTime dateTime )
		{
			var strRes = XmlConvert.ToString( dateTime, "yyyy-MM-ddTHH:mm:ss" );
			var result = strRes.Replace( "T", " " );
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

		public static long ToLongOrDefault( this string srcString )
		{
			try
			{
				var dateTime = long.Parse( srcString, CultureInfo.InvariantCulture );
				return dateTime;
			}
			catch
			{
				return default( long );
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

		public static string BuildUrl( this IEnumerable< string > urlParrts, bool escapeUrl = false )
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

		public static string ToJson( this IEnumerable< Inventory > source )
		{
			var inventories = source as IList< Inventory > ?? source.ToList();
			var items = string.Join( ",", inventories.Select( x => string.Format( "{{ItemId:{0},ProductId:{1},Qty:{2},StockId:{3}}}",
				string.IsNullOrWhiteSpace( x.ItemId ) ? PredefinedValues.NotAvailable : x.ItemId,
				string.IsNullOrWhiteSpace( x.ProductId ) ? PredefinedValues.NotAvailable : x.ProductId,
				x.Qty,
				string.IsNullOrWhiteSpace( x.StockId ) ? PredefinedValues.NotAvailable : x.StockId
				) ) );
			var res = string.Format( "{{Count:{0}, Items:[{1}]}}", inventories.Count(), items );
			return res;
		}

		public static string ToJson( this IEnumerable< InventoryBySku > source )
		{
			var inventories = source as IList< InventoryBySku > ?? source.ToList();
			var items = string.Join( ",", inventories.Select( x => string.Format( "{{Sku:{0},StockId:{1},Qty:{2},MinQty:{3}}}",
				string.IsNullOrWhiteSpace( x.Sku ) ? PredefinedValues.NotAvailable : x.Sku,
				string.IsNullOrWhiteSpace( x.StockId ) ? PredefinedValues.NotAvailable : x.StockId,
				x.Qty,
				x.MinQty
				) ) );
			var res = string.Format( "{{Count:{0}, Items:[{1}]}}", inventories.Count(), items );
			return res;
		}

		public static string ToJson( this IEnumerable< Product > source )
		{
			var products = source as IList< Product > ?? source.ToList();
			var items = string.Join( ",", products.Select( x => string.Format( "{{Sku:{0},ProductId:{1},Qty:{2},EntityId:{3}}}",
				string.IsNullOrWhiteSpace( x.Sku ) ? PredefinedValues.NotAvailable : x.Sku,
				string.IsNullOrWhiteSpace( x.ProductId ) ? PredefinedValues.NotAvailable : x.ProductId,
				string.IsNullOrWhiteSpace( x.Qty ) ? PredefinedValues.NotAvailable : x.Qty,
				string.IsNullOrWhiteSpace( x.EntityId ) ? PredefinedValues.NotAvailable : x.EntityId ) ) );

			var res = string.Format( "{{Count:{0}, Items:[{1}]}}", products.Count(), items );
			return res;
		}

		public static string ToJson( this IEnumerable< Order > source )
		{
			var orders = source as IList< Order > ?? source.ToList();
			var items = string.Join( ",", orders.Select( x => string.Format( "{{id:{0},createdAt:{1}}}", string.IsNullOrWhiteSpace( x.OrderIncrementalId ) ? PredefinedValues.NotAvailable : x.OrderIncrementalId, x.CreatedAt ) ) );
			var res = string.Format( "{{Count:{0}, Items:[{1}]}}", orders.Count(), items );
			return res;
		}

		public static string ToJson( this IEnumerable< ResponseStockItem > source )
		{
			var stockItems = source as IList< ResponseStockItem > ?? source.ToList();
			var items = string.Join( ",", stockItems.Select( x => string.Format( "{{Code:{0},ItemId:{1},Message:{2}}}",
				string.IsNullOrWhiteSpace( x.Code ) ? PredefinedValues.NotAvailable : x.Code,
				string.IsNullOrWhiteSpace( x.ItemId ) ? PredefinedValues.NotAvailable : x.ItemId,
				string.IsNullOrWhiteSpace( x.Message ) ? PredefinedValues.NotAvailable : x.Message ) ) );
			var res = string.Format( "{{Count:{0}, Items:[{1}]}}", stockItems.Count(), items );
			return res;
		}

		public static string ToJson( this IEnumerable< PutStockItem > source )
		{
			var stockItems = source as IList< PutStockItem > ?? source.ToList();
			var items = string.Join( ",", stockItems.Select( x => string.Format( "{{Id:{0},qty:{1}",
				string.IsNullOrWhiteSpace( x.Id ) ? PredefinedValues.NotAvailable : x.Id,
				( x.UpdateEntity == null || string.IsNullOrWhiteSpace( x.UpdateEntity.qty ) ) ? PredefinedValues.NotAvailable : x.UpdateEntity.qty ) ) );
			var res = string.Format( "{{Count:{0}, Items:[{1}]}}", stockItems.Count(), items );
			return res;
		}

		public static string ToJson( this IEnumerable< StockItem > source )
		{
			var stockItems = source as IList< StockItem > ?? source.ToList();
			var items = string.Join( ",", stockItems.Select( x => string.Format( "{{ItemId:{0}, ProductId:{1}, Qty:{2}, StockId:{3}}}",
				string.IsNullOrWhiteSpace( x.ItemId ) ? PredefinedValues.NotAvailable : x.ItemId,
				string.IsNullOrWhiteSpace( x.ProductId ) ? PredefinedValues.NotAvailable : x.ProductId,
				x.Qty,
				string.IsNullOrWhiteSpace( x.StockId ) ? PredefinedValues.NotAvailable : x.StockId ) ) );

			var res = string.Format( "{{Count:{0}, Items:[{1}]}}", stockItems.Count(), items );

			return res;
		}

		public static IEnumerable< IEnumerable< T > > Batch< T >(
			this IEnumerable< T > source, int batchSize )
		{
			using( var enumerator = source.GetEnumerator() )
			{
				while( enumerator.MoveNext() )
					yield return YieldBatchElements( enumerator, batchSize - 1 );
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