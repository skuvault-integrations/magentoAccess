using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MagentoAccess.Misc;

namespace MagentoAccess.Services
{
	internal interface IWebRequestServices
	{
		Stream GetResponseStream( WebRequest webRequest );

		Task< Stream > GetResponseStreamAsync( WebRequest webRequest, Mark mark = null );

		WebRequest CreateServiceGetRequest( string serviceUrl, Dictionary< string, string > rawUrlParameters );

		Task< WebRequest > CreateServiceGetRequestAsync( string serviceUrl, string body, Dictionary< string, string > rawHeaders );

		void PopulateRequestByBody( string body, HttpWebRequest webRequest );
	}
}