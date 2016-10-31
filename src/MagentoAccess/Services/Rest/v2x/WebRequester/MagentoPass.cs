using System;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoPass
	{
		public string Password { get; private set; }

		private MagentoPass( string password )
		{
			this.Password = password;
		}

		public static MagentoPass Create( string token )
		{
			return new MagentoPass( token );
		}

		public override string ToString()
		{
			return this.Password;
		}

		public static MagentoPass Empty { get; } = new MagentoPass( string.Empty );
	}
}