using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using MagentoAccess.M2catalogInventoryStockRegistryV1_v_2_0_2_0_CE;
using MagentoAccess.M2integrationAdminTokenServiceV1_v_2_0_2_0_CE;
using MagentoAccess.Magento2backendModuleServiceV1_v_2_0_2_0_CE;
using MagentoAccess.Magento2catalogCategoryManagementV1_v_2_0_2_0_CE;
using MagentoAccess.Magento2catalogProductAttributeMediaGalleryManagementV1_v_2_0_2_0_CE;
using MagentoAccess.Magento2catalogProductRepositoryV1_v_2_0_2_0_CE;
using MagentoAccess.Magento2salesOrderRepositoryV1_v_2_0_2_0_CE;
using MagentoAccess.Misc;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce
{
	internal class MagentoServiceSoapClientFactory
	{
		protected const string SoapApiUrlPart1 = "soap/";
		protected const string SoapApiUrlPart2 = "?services=";
		protected const string DefaultStoreCode = "default";

		private readonly string _baseMagentoUrl;
		private readonly bool _logRawMessages;
		private readonly MagentoConfig _config;

		private string _session;

		internal string Session
		{
			get { return this._session; }
			set
			{
				if( !string.Equals( this._session, value ) )
				{
					this._session = value;
					this.ReCreateFactories( this._baseMagentoUrl, this._logRawMessages, this._session, this._config );
				}
			}
		}

		private Magento2xCommonClientFactory< integrationAdminTokenServiceV1PortTypeClient, integrationAdminTokenServiceV1PortType > _adminClientFactory;
		private Magento2xCommonClientFactory< catalogProductAttributeMediaGalleryManagementV1PortTypeClient, catalogProductAttributeMediaGalleryManagementV1PortType > _mediaGalleryFactory;
		private Magento2xCommonClientFactory< salesOrderRepositoryV1PortTypeClient, salesOrderRepositoryV1PortType > _salesOrderRepositoryFactory;
		private Magento2xCommonClientFactory< catalogCategoryManagementV1PortTypeClient, catalogCategoryManagementV1PortType > _categoriesRepositoryFactory;
		private Magento2xCommonClientFactory< catalogProductRepositoryV1PortTypeClient, catalogProductRepositoryV1PortType > _catalogProductRepositoryFactory;
		private Magento2xCommonClientFactory< backendModuleServiceV1PortTypeClient, backendModuleServiceV1PortType > _backendModuleServiceFactory;
		private Magento2xCommonClientFactory< catalogInventoryStockRegistryV1PortTypeClient, catalogInventoryStockRegistryV1PortType > _catalogInventoryStockRegistryFactory;

		public MagentoServiceSoapClientFactory( string baseMagentoUrl, bool logRawMessages, string sessionId, MagentoConfig config )
		{
			this._baseMagentoUrl = baseMagentoUrl;
			this._logRawMessages = logRawMessages;
			this._session = sessionId;
			this._config = config;
			this.ReCreateFactories( baseMagentoUrl, logRawMessages, sessionId, config );
		}

		private void ReCreateFactories( string baseMagentoUrl, bool logRawMessages, string sessionId, MagentoConfig config )
		{
			this._adminClientFactory = new Magento2xCommonClientFactory< integrationAdminTokenServiceV1PortTypeClient, integrationAdminTokenServiceV1PortType >(
				( binding, endpoint ) => new integrationAdminTokenServiceV1PortTypeClient( binding, endpoint ),
				baseMagentoUrl, "integrationAdminTokenServiceV1", MessageVersion.Soap12, logRawMessages, sessionId, config );

			this._mediaGalleryFactory = new Magento2xCommonClientFactory< catalogProductAttributeMediaGalleryManagementV1PortTypeClient, catalogProductAttributeMediaGalleryManagementV1PortType >(
				( binding, endpoint ) => new catalogProductAttributeMediaGalleryManagementV1PortTypeClient( binding, endpoint ),
				baseMagentoUrl, "catalogProductAttributeMediaGalleryManagementV1", MessageVersion.Soap11, logRawMessages, sessionId, config );

			this._salesOrderRepositoryFactory = new Magento2xCommonClientFactory< salesOrderRepositoryV1PortTypeClient, salesOrderRepositoryV1PortType >(
				( binding, endpoint ) => new salesOrderRepositoryV1PortTypeClient( binding, endpoint ),
				baseMagentoUrl, "salesOrderRepositoryV1", MessageVersion.Soap11, logRawMessages, sessionId, config );

			this._categoriesRepositoryFactory = new Magento2xCommonClientFactory< catalogCategoryManagementV1PortTypeClient, catalogCategoryManagementV1PortType >(
				( binding, endpoint ) => new catalogCategoryManagementV1PortTypeClient( binding, endpoint ),
				baseMagentoUrl, "catalogCategoryManagementV1", MessageVersion.Soap11, logRawMessages, sessionId, config );

			this._catalogProductRepositoryFactory = new Magento2xCommonClientFactory< catalogProductRepositoryV1PortTypeClient, catalogProductRepositoryV1PortType >(
				( binding, endpoint ) => new catalogProductRepositoryV1PortTypeClient( binding, endpoint ),
				baseMagentoUrl, "catalogProductRepositoryV1", MessageVersion.Soap12, logRawMessages, sessionId, config );

			this._backendModuleServiceFactory = new Magento2xCommonClientFactory< backendModuleServiceV1PortTypeClient, backendModuleServiceV1PortType >(
				( binding, endpoint ) => new backendModuleServiceV1PortTypeClient( binding, endpoint ),
				baseMagentoUrl, "backendModuleServiceV1", MessageVersion.Soap12, logRawMessages, sessionId, config );

			this._catalogInventoryStockRegistryFactory = new Magento2xCommonClientFactory< catalogInventoryStockRegistryV1PortTypeClient, catalogInventoryStockRegistryV1PortType >(
				( binding, endpoint ) => new catalogInventoryStockRegistryV1PortTypeClient( binding, endpoint ),
				baseMagentoUrl, "catalogInventoryStockRegistryV1", MessageVersion.Soap11, logRawMessages, sessionId, config );
		}

		public T CreateClient< T >() where T : class
		{
			if( typeof( T ) == typeof( integrationAdminTokenServiceV1PortTypeClient ) )
			{
				return this._adminClientFactory.GetClient() as T;
			}
			if( typeof( T ) == typeof( catalogProductAttributeMediaGalleryManagementV1PortTypeClient ) )
			{
				return this._mediaGalleryFactory.GetClient() as T;
			}
			if( typeof( T ) == typeof( salesOrderRepositoryV1PortTypeClient ) )
			{
				return this._salesOrderRepositoryFactory.GetClient() as T;
			}
			if( typeof( T ) == typeof( catalogCategoryManagementV1PortTypeClient ) )
			{
				return this._categoriesRepositoryFactory.GetClient() as T;
			}
			if( typeof( T ) == typeof( catalogProductRepositoryV1PortTypeClient ) )
			{
				return this._catalogProductRepositoryFactory.GetClient() as T;
			}
			if( typeof( T ) == typeof( backendModuleServiceV1PortTypeClient ) )
			{
				return this._backendModuleServiceFactory.GetClient() as T;
			}
			if( typeof( T ) == typeof( catalogInventoryStockRegistryV1PortTypeClient ) )
			{
				return this._catalogInventoryStockRegistryFactory.GetClient() as T;
			}

			return default(T);
		}

		public T RefreshClient< T >( T client ) where T : class
		{
			if( typeof( T ) == typeof( integrationAdminTokenServiceV1PortTypeClient ) )
			{
				return this._adminClientFactory.RefreshClient( client as integrationAdminTokenServiceV1PortTypeClient ) as T;
			}
			if( typeof( T ) == typeof( catalogProductAttributeMediaGalleryManagementV1PortTypeClient ) )
			{
				return this._mediaGalleryFactory.RefreshClient( client as catalogProductAttributeMediaGalleryManagementV1PortTypeClient ) as T;
			}
			if( typeof( T ) == typeof( salesOrderRepositoryV1PortTypeClient ) )
			{
				return this._salesOrderRepositoryFactory.RefreshClient( client as salesOrderRepositoryV1PortTypeClient ) as T;
			}
			if( typeof( T ) == typeof( catalogCategoryManagementV1PortTypeClient ) )
			{
				return this._categoriesRepositoryFactory.RefreshClient( client as catalogCategoryManagementV1PortTypeClient ) as T;
			}
			if( typeof( T ) == typeof( catalogProductRepositoryV1PortTypeClient ) )
			{
				return this._catalogProductRepositoryFactory.RefreshClient( client as catalogProductRepositoryV1PortTypeClient ) as T;
			}
			if( typeof( T ) == typeof( backendModuleServiceV1PortTypeClient ) )
			{
				return this._backendModuleServiceFactory.RefreshClient( client as backendModuleServiceV1PortTypeClient ) as T;
			}
			if( typeof( T ) == typeof( catalogInventoryStockRegistryV1PortTypeClient ) )
			{
				return this._catalogInventoryStockRegistryFactory.RefreshClient( client as catalogInventoryStockRegistryV1PortTypeClient ) as T;
			}

			return default(T);
		}

		public integrationAdminTokenServiceV1PortTypeClient CreateMagentoServiceAdminClient()
		{
			return this.CreateClient< integrationAdminTokenServiceV1PortTypeClient >();
		}

		public catalogProductRepositoryV1PortTypeClient CreateMagentoCatalogProductRepositoryServiceClient()
		{
			return this.CreateClient< catalogProductRepositoryV1PortTypeClient >();
		}

		public catalogProductRepositoryV1PortTypeClient RefreshMagentoCatalogProductRepositoryServiceClient( catalogProductRepositoryV1PortTypeClient client )
		{
			return this.RefreshClient( client );
		}

		public catalogInventoryStockRegistryV1PortTypeClient CreateMagentoCatalogInventoryStockServiceClient()
		{
			return this.CreateClient< catalogInventoryStockRegistryV1PortTypeClient >();
		}

		public catalogInventoryStockRegistryV1PortTypeClient RefreshMagentoCatalogInventoryStockServiceClient( catalogInventoryStockRegistryV1PortTypeClient client )
		{
			return this.RefreshClient( client );
		}

		public backendModuleServiceV1PortTypeClient CreateMagentoBackendModuleServiceV1Client()
		{
			return this.CreateClient< backendModuleServiceV1PortTypeClient >();
		}

		public backendModuleServiceV1PortTypeClient RefreshMagentoBackendModuleServiceV1Client( backendModuleServiceV1PortTypeClient client )
		{
			return this.RefreshClient( client );
		}

		public catalogProductAttributeMediaGalleryManagementV1PortTypeClient CreateMagentocatalogProductAttributeMediaGalleryRepositoryServiceClient()
		{
			return this.CreateClient< catalogProductAttributeMediaGalleryManagementV1PortTypeClient >();
		}

		public catalogProductAttributeMediaGalleryManagementV1PortTypeClient RefreshMagentocatalogProductAttributeMediaGalleryRepositoryServiceClient( catalogProductAttributeMediaGalleryManagementV1PortTypeClient client )
		{
			return this.RefreshClient( client );
		}

		public catalogCategoryManagementV1PortTypeClient CreateMagentoCategoriesRepositoryServiceClient()
		{
			return this.CreateClient< catalogCategoryManagementV1PortTypeClient >();
		}

		public catalogCategoryManagementV1PortTypeClient RefreshMagentoCategoriesRepositoryServiceClient( catalogCategoryManagementV1PortTypeClient client )
		{
			return this.RefreshClient( client );
		}

		public salesOrderRepositoryV1PortTypeClient CreateMagentoSalesOrderRepositoryServiceClient()
		{
			return this.CreateClient< salesOrderRepositoryV1PortTypeClient >();
		}

		public salesOrderRepositoryV1PortTypeClient RefreshMagentoSalesOrderRepositoryServiceClient( salesOrderRepositoryV1PortTypeClient client )
		{
			return this.RefreshClient( client );
		}

		private sealed class Magento2xCommonClientFactory< T, T2 > : BaseMagentoServiceSoapClientFactory< T, T2 > where T : ClientBase< T2 > where T2 : class
		{
			private readonly string _sessionId;

			private readonly Func< CustomBinding, EndpointAddress, T > _clientBuilder;
			private readonly CustomBinding _binding;
			private readonly EndpointAddress _endpointAddress;

			public Magento2xCommonClientFactory( Func< CustomBinding, EndpointAddress, T > clientBuilder, string baseMagentoUrl, string servicesName, MessageVersion messageVersion, bool logRawMessages, string sessionId, MagentoConfig config ) : base( baseMagentoUrl, logRawMessages, config )
			{
				this._sessionId = sessionId;
				this._clientBuilder = clientBuilder;
				this._binding = CustomBinding( this._baseMagentoUrl, messageVersion, this._config.BindingDecompressionEnabled );

				var storeCode = string.IsNullOrWhiteSpace( config.StoreCode ) ? DefaultStoreCode : config.StoreCode;
				var endPoint = new List< string > { this._baseMagentoUrl, SoapApiUrlPart1 + storeCode + SoapApiUrlPart2 + servicesName }.BuildUrl( trimTailsSlash : true );
				this._endpointAddress = new EndpointAddress( endPoint );
			}

			protected override T CreateClient()
			{
				var magentoSoapService = this._clientBuilder( this._binding, this._endpointAddress );
				magentoSoapService.Endpoint.Behaviors.Add( new ChannelBehaviour.CustomBehavior { AccessToken = this._sessionId, LogRawMessages = this._logRawMessages } );
				return magentoSoapService;
			}
		}
	}
}
