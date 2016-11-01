namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoServicePath
	{
		public string RepositoryPath { get; private set; }

		private MagentoServicePath( string repositoryPath )
		{
			this.RepositoryPath = repositoryPath;
		}

		public static MagentoServicePath Create( string repositoryPath )
		{
			return new MagentoServicePath( repositoryPath );
		}

		public override string ToString()
		{
			return this.RepositoryPath;
		}

		public static MagentoServicePath Products { get; } = new MagentoServicePath( "products" );
		public static MagentoServicePath CatalogStockItems { get; } = new MagentoServicePath("stockItems");
		public static MagentoServicePath IntegrationAdmin { get; } = new MagentoServicePath( "integration/admin/token" );
		public static MagentoServicePath Orders { get; } = new MagentoServicePath( "orders" );
	}
}