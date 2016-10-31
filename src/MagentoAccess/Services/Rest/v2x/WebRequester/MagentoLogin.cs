using System;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoLogin
	{
		public string Login { get; private set; }

		private MagentoLogin( string login )
		{
			this.Login = login;
		}

		public static MagentoLogin Create( string token )
		{
			return new MagentoLogin( token );
		}

		public override string ToString()
		{
			return this.Login;
		}

		public static MagentoLogin Empty { get; } = new MagentoLogin( String.Empty );
	}
}