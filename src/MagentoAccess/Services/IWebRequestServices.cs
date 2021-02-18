using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MagentoAccess.Services
{
	internal interface IWebRequestServices
	{
		Task< Stream > GetResponseStreamAsync( string method, string url, string authorizationToken, CancellationToken cancellationToken, string body = null, int? operationTimeout = null, Action< string > logClientHeaders = null );
	}
}