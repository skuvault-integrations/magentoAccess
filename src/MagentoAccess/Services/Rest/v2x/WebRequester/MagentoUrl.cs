using System.Security.Policy;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoUrl
	{
		public Url Url { get; }

		private MagentoUrl( string url )
		{
			this.Url = new Url( url );
		}
		
		public static MagentoUrl Create( string shopBaseUrl, string relativeUrl )
		{
			var url = shopBaseUrl?.TrimEnd( '/' ) + relativeUrl;
			return new MagentoUrl( url.Trim() );
		}

		public override string ToString()
		{
			return this.Url.Value;
		}

		public static MagentoUrl SandBox { get; } = new MagentoUrl( "http://127.0.0.1/index.php/rest/V1/" );
	}
}