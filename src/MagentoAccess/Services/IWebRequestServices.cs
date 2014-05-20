using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MagentoAccess.Services
{
	public interface IWebRequestServices
	{
		Stream GetResponseStream( WebRequest webRequest );

		Task< Stream > GetResponseStreamAsync( WebRequest webRequest );

		WebRequest CreateServiceGetRequest( string serviceUrl, Dictionary< string, string > rawUrlParameters );

		Task< WebRequest > CreateServiceGetRequestAsync( string serviceUrl, string body, Dictionary< string, string > rawHeaders );

		void PopulateRequestByBody( string body, HttpWebRequest webRequest );
	}
}