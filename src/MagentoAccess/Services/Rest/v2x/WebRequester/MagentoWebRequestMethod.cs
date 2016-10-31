using System.Net;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoWebRequestMethod
	{
		public string Method { get; private set; }

		private MagentoWebRequestMethod( string method )
		{
			this.Method = method;
		}

		public override string ToString()
		{
			return this.Method.ToString();
		}

		public static MagentoWebRequestMethod Get { get; } = new MagentoWebRequestMethod( WebRequestMethods.Http.Get );
		public static MagentoWebRequestMethod Put { get; } = new MagentoWebRequestMethod( WebRequestMethods.Http.Put );
		public static MagentoWebRequestMethod Post { get; } = new MagentoWebRequestMethod( WebRequestMethods.Http.Post );
	}
}