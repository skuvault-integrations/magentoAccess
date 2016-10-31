using System;

namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class AuthorizationToken
	{
		public string Token { get; private set; }

		private AuthorizationToken( string token )
		{
			token = token.Trim( '"' );
			this.Token = token;
		}

		public static AuthorizationToken Create( string token )
		{
			return new AuthorizationToken( token );
		}

		public override string ToString()
		{
			return this.Token;
		}

		public static AuthorizationToken Empty { get; } = new AuthorizationToken( String.Empty );
	}
}