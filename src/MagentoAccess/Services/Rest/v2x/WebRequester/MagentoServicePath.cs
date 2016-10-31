namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoServicePath
	{
		public string Repository { get; private set; }

		private MagentoServicePath( string repository )
		{
			this.Repository = repository;
		}

		public override string ToString()
		{
			return this.Repository;
		}

		public static MagentoServicePath Products { get; } = new MagentoServicePath( "products" );
		public static MagentoServicePath IntegrationAdmin { get; } = new MagentoServicePath("integration/admin/token");
		public static MagentoServicePath Orders { get; } = new MagentoServicePath( "orders" );
	}
}