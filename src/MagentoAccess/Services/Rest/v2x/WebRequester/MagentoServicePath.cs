namespace MagentoAccess.Services.Rest.v2x.WebRequester
{
	public class MagentoServicePath
	{
		public const string CategoriesPath = "categories";
		public const string ProductsPath = "products";
		public const string StockItemsPath = "stockItems";
		public const string OrdersPath = "orders";
		public const string ManufacturersPath = "products/attributes/manufacturer";
		public const string IntegrationPath = "integration/admin/token";
		public const string ShipmentsServicePath = "shipments";

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

		public MagentoServicePath AddCatalog(string src)
		{
			this.RepositoryPath = this.RepositoryPath + "/" + src.Trim( '/' ).Trim( '\\' );
			return this;
		}
		
		public static MagentoServicePath CreateCategoriesPath()
		{
			return Create( CategoriesPath );
		}
		
		public static MagentoServicePath CreateProductsServicePath()
		{
			return Create( ProductsPath );
		}
		
		public static MagentoServicePath CreateStockItemsServicePath()
		{
			return Create( StockItemsPath );
		}
		
		public static MagentoServicePath CreateOrdersServicePath()
		{
			return Create( OrdersPath );
		}

		public static MagentoServicePath CreateManufacturersServicePath()
		{
			return Create( ManufacturersPath );
		}
		
		public static MagentoServicePath CreateIntegrationServicePath()
		{
			return Create( IntegrationPath );
		}

		public static MagentoServicePath CreateShipmentsServicePath()
		{
			return Create( ShipmentsServicePath );
		}
	}
}