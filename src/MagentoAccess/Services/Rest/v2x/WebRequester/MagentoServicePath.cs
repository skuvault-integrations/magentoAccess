namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoServicePath
	{
		public string Repository { get; private set; }

		private MagentoServicePath( string repository )
		{
			this.Repository = repository;
		}

		public static MagentoServicePath Products { get; } = new MagentoServicePath( "products" );
		public static MagentoServicePath Orders { get; } = new MagentoServicePath( "orders" );
	}
}