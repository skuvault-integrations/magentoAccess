using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using MagentoAccess.Models.Services.Rest.v1x.BaseResponse;

namespace MagentoAccess.Services.Parsers
{
	internal class MagentoBaseResponseParser< TParseResult > : IMagentoBaseResponseParser< TParseResult >
	{
		protected static string GetElementValue( XElement x, XNamespace ns, params string[] elementName )
		{
			var parsedElement = string.Empty;

			if( elementName.Length <= 0 )
				return parsedElement;

			var element = x.Element( ns + elementName[ 0 ] );
			if( element == null )
				return parsedElement;

			return elementName.Length > 1 ? GetElementValue( element, ns, elementName.Skip( 1 ).ToArray() ) : element.Value;
		}

		protected string GetElementAttribute( string attributeName, XElement xElement, XNamespace ns, params string[] elementName )
		{
			var elementAttribute = string.Empty;

			if( elementName.Length <= 0 )
				return xElement.Attribute( attributeName ).Value;

			var element = xElement.Element( ns + elementName[ 0 ] );
			if( element == null )
				return elementAttribute;

			return elementName.Length > 1 ? this.GetElementAttribute( attributeName, element, ns, elementName.Skip( 1 ).ToArray() ) : element.Attribute( attributeName ).Value;
		}

		protected ResponseError ResponseContainsErrors( XElement root, XNamespace ns )
		{
			var messages = root.Element( ns + "messages" );

			if( messages == null )
				return null;

			var errorCode = GetElementValue( messages, ns, "error", "data_item", "code" );
			var errorMessage = GetElementValue( messages, ns, "error", "data_item", "message" );

			var ResponseError = new ResponseError { Code = errorCode, Message = errorMessage };
			return ResponseError;
		}

		public TParseResult Parse( WebResponse response )
		{
			var result = default( TParseResult );
			using( var responseStream = response.GetResponseStream() )
			{
				if( responseStream != null )
				{
					using( var memStream = new MemoryStream() )
					{
						responseStream.CopyTo( memStream, 0x100 );
						result = this.Parse( memStream );
					}
				}
			}

			return result;
		}

		public TParseResult Parse( string str )
		{
			var stream = new MemoryStream();
			var streamWriter = new StreamWriter( stream );
			streamWriter.Write( str );
			streamWriter.Flush();
			stream.Position = 0;

			using( stream )
				return Parse( stream );
		}

		protected virtual TParseResult ParseWithoutExceptionHanding( XElement root )
		{
			return default( TParseResult );
		}

		public virtual TParseResult Parse( Stream stream, bool keepStremPosition = true )
		{
			var streamStartPos = stream.Position;

			try
			{
				var root = XElement.Load( stream );
				return this.ParseWithoutExceptionHanding( root );
			}
			catch( Exception ex )
			{
				var buffer = new byte[ stream.Length ];
				stream.Read( buffer, 0, ( int )stream.Length );
				var utf8Encoding = new UTF8Encoding();
				var bufferStr = utf8Encoding.GetString( buffer );
				throw new Exception( "Can't parse: " + bufferStr, ex );
			}
			finally
			{
				if( keepStremPosition )
					stream.Position = streamStartPos;
			}
		}
	}
}