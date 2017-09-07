using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetSessionId;
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccess.Services.Soap;
using Netco.ActionPolicyServices;
using Netco.Logging;
using MagentoUrl = MagentoAccess.Services.Rest.v2x.WebRequester.MagentoUrl;

namespace MagentoAccess.Services.Rest.v2x
{
	internal partial class MagentoServiceLowLevel : IMagentoServiceLowLevelSoap
	{
		public string ApiUser { get; }
		public string ApiKey { get; }
		public string Store { get; }
		public bool LogRawMessages { get; }
		public string StoreVersion { get; set; }
		protected IProductRepository ProductRepository { get; set; }
		protected IntegrationAdminTokenRepository IntegrationAdminTokenRepository { get; set; }
		protected ICatalogStockItemRepository CatalogStockItemRepository { get; set; }
		protected ISalesOrderRepositoryV1 SalesOrderRepository { get; set; }
		protected ActionPolicyAsync RepeatOnAuthProblemAsync { get; }

		protected SemaphoreSlim _reauthorizeLock = new SemaphoreSlim( 1, 1 );

		protected int reauthorizationsCount = 0;

		public async Task< bool > InitAsync( bool supressExceptions = false )
		{
			try
			{
				if( this.IntegrationAdminTokenRepository != null )
					return true;

				this.IntegrationAdminTokenRepository = new IntegrationAdminTokenRepository( MagentoUrl.Create( this.Store ) );
				await this.ReauthorizeAsync().ConfigureAwait( false );
				return true;
			}
			catch( Exception e )
			{
				if( supressExceptions )
					return false;
				throw;
			}
		}

		public MagentoServiceLowLevel()
		{
			this.RepeatOnAuthProblemAsync = ActionPolicyAsync.From( ( exception =>
			{
				var webException = ( exception as MagentoWebException )?.InnerException as WebException;
				if( webException == null )
					return false;

				switch( webException.Status )
				{
					case WebExceptionStatus.ProtocolError:
						var response = webException.Response as HttpWebResponse;
						if( response == null )
							return false;
						switch( response.StatusCode )
						{
							case HttpStatusCode.Unauthorized:
								return true;
							default:
								return false;
						}
					default:
						return false;
				}
			} ) )
				.RetryAsync( 3, async ( ex, i ) =>
				{
					await this._reauthorizeLock.WaitAsync();
					var reauthorizationsCountPropagation = this.reauthorizationsCount;
					this._reauthorizeLock.Release();
					await this._reauthorizeLock.WaitAsync();
					try
					{
						if( reauthorizationsCountPropagation != this.reauthorizationsCount )
							return;
						Interlocked.Increment( ref this.reauthorizationsCount );
						MagentoLogger.Log().Trace( ex, "Retrying Magento API call due to authorization problem for the {0} time", i );
						await this.ReauthorizeAsync().ConfigureAwait( false );
						await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) ).ConfigureAwait( false );
					}
					finally
					{
						this._reauthorizeLock.Release();
					}
				} );
		}

		public MagentoServiceLowLevel( string soapApiUser, string soapApiKey, string baseMagentoUrl, bool logRawMessages ):this()
		{
			this.ApiUser = soapApiUser;
			this.ApiKey = soapApiKey;
			this.Store = baseMagentoUrl;
			this.LogRawMessages = logRawMessages;
		}

		protected async Task ReauthorizeAsync()
		{
			try
			{
				var newToken = await this.IntegrationAdminTokenRepository.GetTokenAsync( MagentoLogin.Create( this.ApiUser ), MagentoPass.Create( this.ApiKey ) ).ConfigureAwait( false );
				var magentoUrl = MagentoUrl.Create( this.Store );
				this.ProductRepository = new ProductRepository( newToken, magentoUrl );
				this.CatalogStockItemRepository = new CatalogStockItemRepository( newToken, magentoUrl );
				this.SalesOrderRepository = new SalesOrderRepositoryV1( newToken, magentoUrl );
			}
			catch( Exception e )
			{
				throw;
			}
		}

		public bool GetStockItemsWithoutSkuImplementedWithPages => false;

		public bool GetOrderByIdForFullInformation => false;
		public bool GetOrdersUsesEntityInsteadOfIncrementId => true;

		public async Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException, Mark mark = null )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				try
				{
					var task1 = this.ProductRepository.GetProductsAsync( DateTime.UtcNow, mark );
					var task2 = this.SalesOrderRepository.GetOrdersAsync( DateTime.UtcNow.AddMinutes( -1 ), DateTime.UtcNow, new PagingModel( 10, 1 ), mark );
					await Task.WhenAll( task1, task2 ).ConfigureAwait( false );
					return new GetMagentoInfoResponse( "R2.0.0.0", "CE" );
				}
				catch( Exception e )
				{
					if( suppressException )
						return null;
					throw;
				}
			} ).ConfigureAwait( false );
		}

		public string ToJsonSoapInfo()
		{
			return null;
		}

		public Task< bool > ShoppingCartGuestCustomerSet( int shoppingCart, string customerfirstname, string customerMail, string customerlastname, string store )
		{
			return null;
		}

		public Task< bool > ShoppingCartAddressSet( int shoppingCart, string store )
		{
			return null;
		}

		public Task< bool > ShoppingCartAddProduct( int shoppingCartId, string productId, string store )
		{
			return null;
		}

		public Task< bool > ShoppingCartSetShippingMethod( int shoppingCartId, string store )
		{
			return null;
		}

		public Task< bool > ShoppingCartSetPaymentMethod( int shoppingCartId, string store )
		{
			return null;
		}

		public Task< GetSessionIdResponse > GetSessionId( bool throwException = true )
		{
			return null;
		}

		public Task< GetCategoryTreeResponse > GetCategoriesTreeAsync( string rootCategory = "1" )
		{
			return null;
		}

		public Task< CatalogProductAttributeInfoResponse > GetManufacturersInfoAsync( string attribute )
		{
			return null;
		}
	}
}