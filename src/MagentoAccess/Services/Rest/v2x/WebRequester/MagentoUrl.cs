using System.Security.Policy;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoUrl
	{
		public Url Url { get; private set; }

		private MagentoUrl( string url )
		{
			this.Url = new Url( url );
		}

		public static MagentoUrl Create( string url )
		{
			return new MagentoUrl( url );
		}

		public override string ToString()
		{
			return this.Url.Value;
		}

		public static MagentoUrl SandBox { get; } = new MagentoUrl( "http://127.0.0.1/" );
	}
}