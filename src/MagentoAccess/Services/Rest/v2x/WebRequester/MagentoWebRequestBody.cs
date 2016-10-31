namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoWebRequestBody
	{
		public string Body { get; private set; }

		private MagentoWebRequestBody( string body )
		{
			this.Body = body ?? string.Empty;
		}

		public override string ToString()
		{
			return this.Body.ToString();
		}

		public static MagentoWebRequestBody Create( string url )
		{
			return new MagentoWebRequestBody( url );
		}

		public static MagentoWebRequestBody Empty { get; } = new MagentoWebRequestBody( string.Empty );
	}
}