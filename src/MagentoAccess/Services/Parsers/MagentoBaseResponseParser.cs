using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace MagentoAccess.Services.Parsers
{
	public class MagentoBaseResponseParser< TParseResult >
	{
		public static string GetElementValue( XElement x, XNamespace ns, params string[] elementName )
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

		public virtual TParseResult Parse( Stream stream, bool keepStremPosition = true )
		{
			return default( TParseResult );
		}

		protected ResponseError ResponseContainsErrors( XElement root, XNamespace ns )
		{
			var isSuccess = root.Element( ns + "messages" );

			var errorCode = GetElementValue( isSuccess, ns, "error", "data_item", "code" );
			var errorMessage = GetElementValue( isSuccess, ns, "error", "data_item", "message" );

			if( string.IsNullOrWhiteSpace( errorCode ) || string.IsNullOrWhiteSpace( errorMessage ) )
			{
				var ResponseError = new ResponseError { Code = errorCode, Message = errorMessage };

				return ResponseError;
			}

			return null;
		}
	}

	public class ResponseError
	{
		public string Message { get; set; }
		public string Code { get; set; }
	}
}