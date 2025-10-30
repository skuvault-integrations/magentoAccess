using System;
using System.Security.Policy;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoUrl
	{
		public Uri Url { get; }

		private MagentoUrl( string url )
		{
			this.Url = new Uri( url );
		}
		
		/// <summary>
		/// Gets a main Magento url combining base (shopUrl) and relativeUrl
		/// Ex. https://shop-url.com/index.php/rest/V1/ (or https://shop-url.com/rest/V1/ in a case if resource)
		/// has a redirect (ignoring index.php)  
		/// </summary>
		/// <param name="shopBaseUrl"></param>
		/// <param name="relativeUrl"></param>
		/// <returns></returns>
		public static MagentoUrl Create( string shopBaseUrl, string relativeUrl )
		{
			var url = shopBaseUrl?.TrimEnd( '/' ) + relativeUrl;
			return new MagentoUrl( url.Trim() );
		}

		public override string ToString()
		{
			return this.Url.AbsoluteUri;
		}

		public static MagentoUrl SandBox { get; } = new MagentoUrl( "http://127.0.0.1/index.php/rest/V1/" );
	}
}