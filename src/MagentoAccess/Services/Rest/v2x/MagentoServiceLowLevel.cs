using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;
using MagentoAccess.Models.Services.Soap.GetMagentoInfo;
using MagentoAccess.Models.Services.Soap.GetOrders;
using MagentoAccess.Models.Services.Soap.GetProductAttributeInfo;
using MagentoAccess.Models.Services.Soap.GetProductAttributeMediaList;
using MagentoAccess.Models.Services.Soap.GetProductInfo;
using MagentoAccess.Models.Services.Soap.GetProducts;
using MagentoAccess.Models.Services.Soap.GetSessionId;
using MagentoAccess.Models.Services.Soap.GetStockItems;
using MagentoAccess.Models.Services.Soap.PutStockItems;
using MagentoAccess.Services.Rest.v2x.Repository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using MagentoAccess.Services.Soap;
using Netco.ActionPolicyServices;
using Netco.Extensions;
using MagentoUrl = MagentoAccess.Services.Rest.v2x.WebRequester.MagentoUrl;

namespace MagentoAccess.Services.Rest.v2x
{
	internal class MagentoServiceLowLevel : IMagentoServiceLowLevelSoap
	{
		public string ApiUser { get; }
		public string ApiKey { get; }
		public string Store { get; }
		public string StoreVersion { get; set; }
		protected IProductRepository ProductRepository { get; set; }
		protected IntegrationAdminTokenRepository IntegrationAdminTokenRepository { get; set; }
		protected ICatalogStockItemRepository CatalogStockItemRepository { get; set; }
		protected ActionPolicyAsync RepeatOnAuthProblemAsync { get; }

		protected SemaphoreSlim _reauthorizeLock = new SemaphoreSlim( 1, 1 );

		protected int reauthorizationsCount = 0;

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

		protected async Task ReauthorizeAsync()
		{
			var newToken = await this.IntegrationAdminTokenRepository.GetTokenAsync( MagentoLogin.Create( this.ApiUser ), MagentoPass.Create( this.ApiKey ) );
			var magentoUrl = MagentoUrl.Create( this.Store );
			this.ProductRepository = new ProductRepository( newToken, magentoUrl );
			this.CatalogStockItemRepository = new CatalogStockItemRepository( newToken, magentoUrl );
		}

		public bool GetStockItemsWithoutSkuImplementedWithPages
		{
			get { return false; }
		}

		public bool GetOrdersUsesEntityInsteadOfIncrementId => true;

		public Task< GetOrdersResponse > GetOrdersAsync( DateTime modifiedFrom, DateTime modifiedTo )
		{
			return null;
		}

		public Task< GetOrdersResponse > GetOrdersAsync( IEnumerable< string > ordersIds )
		{
			return null;
		}

		public async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				var products = await this.ProductRepository.GetProductsAsync( updatedFrom ?? DateTime.MinValue, productType, productTypeShouldBeExcluded ).ConfigureAwait( false );
				return new SoapGetProductsResponse( products.SelectMany( x => x.items ).ToList() );
			} );
		}

		public Task< InventoryStockItemListResponse > GetStockItemsAsync( List< string > skusOrIds, IEnumerable<int> scopes )
		{
			return null;
		}

		public Task< OrderInfoResponse > GetOrderAsync( string incrementId )
		{
			return null;
		}

		public Task< OrderInfoResponse > GetOrderAsync( Order order )
		{
			return null;
		}

		public async Task< bool > PutStockItemsAsync( List< PutStockItem > stockItems, Mark markForLog )
		{
			return await this.RepeatOnAuthProblemAsync.Get( async () =>
			{
				var products = await this.CatalogStockItemRepository.PutStockItemsAsync(
					stockItems.Select( x => Tuple.Create( x.Sku, x.ItemId, new RootObject() { stockItem = new StockItem { qty = x.Qty, minQty = x.MinQty } } ) ) ).ConfigureAwait( false );
				return products.All( x => x );
			} );
		}

		public Task< GetMagentoInfoResponse > GetMagentoInfoAsync( bool suppressException )
		{
			return null;
		}

		public string ToJsonSoapInfo()
		{
			return null;
		}

		public Task< bool > PutStockItemAsync( PutStockItem putStockItem, Mark markForLog )
		{
			return null;
		}

		public Task< int > CreateProduct( string storeId, string name, string sku, int isInStock, string productType, Mark markForLog = null )
		{
			return null;
		}

		public Task< bool > DeleteProduct( string storeId, int categoryId, string productId, string identiferType )
		{
			return null;
		}

		public Task< int > CreateCart( string storeid )
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

		public Task< string > CreateOrder( int shoppingcartid, string store )
		{
			return null;
		}

		public Task< GetSessionIdResponse > GetSessionId( bool throwException = true )
		{
			return null;
		}

		public Task< CatalogProductInfoResponse > GetProductInfoAsync( CatalogProductInfoRequest catalogProductInfoRequest, bool throwException = true )
		{
			return null;
		}

		public Task< ProductAttributeMediaListResponse > GetProductAttributeMediaListAsync( GetProductAttributeMediaListRequest getProductAttributeMediaListRequest, bool throwException = true )
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

		public Task< IEnumerable< ProductDetails > > FillProductDetails( IEnumerable< ProductDetails > resultProducts )
		{
			return null;
		}

		public Task< InventoryStockItemListResponse > GetStockItemsWithoutSkuAsync( IEnumerable< string > skusOrIds, IEnumerable< int > scopes )
		{
			throw new NotImplementedException();
		}

		public Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, IReadOnlyCollection< string > skus )
		{
			throw new NotImplementedException();
		}
	}
}