using System.IO;
using System.Net;

namespace MagentoAccess.Services.Parsers
{
	public interface IMagentoBaseResponseParser< TParseResult >
	{
		TParseResult Parse( WebResponse response );
		TParseResult Parse( string str );
		TParseResult Parse( Stream stream, bool keepStremPosition = true );
	}
}