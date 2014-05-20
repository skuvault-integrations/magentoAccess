using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutStockItems;
using MagentoAccess.Services;

namespace MagentoAccess
{
	public class MagentoService : IMagentoService
	{
		public virtual IMagentoServiceLowLevel MagentoServiceLowLevel { get; set; }

		public delegate void SaveAccessToken( string token, string secret );

		public SaveAccessToken AfterGettingToken { get; set; }

		public MagentoService( MagentoAuthenticatedUserCredentials magentoAuthenticatedUserCredentials )
		{
			this.MagentoServiceLowLevel = new MagentoServiceLowLevel(
				magentoAuthenticatedUserCredentials.ConsumerKey,
				magentoAuthenticatedUserCredentials.ConsumerSckretKey,
				magentoAuthenticatedUserCredentials.BaseMagentoUrl,
				magentoAuthenticatedUserCredentials.AccessToken,
				magentoAuthenticatedUserCredentials.AccessTokenSecret
				);
		}

		public MagentoService( MagentoNonAuthenticatedUserCredentials magentoUserCredentials )
		{
			this.MagentoServiceLowLevel = new MagentoServiceLowLevel(
				magentoUserCredentials.ConsumerKey,
				magentoUserCredentials.ConsumerSckretKey,
				magentoUserCredentials.BaseMagentoUrl,
				magentoUserCredentials.RequestTokenUrl,
				magentoUserCredentials.AuthorizeUrl,
				magentoUserCredentials.AccessTokenUrl
				);
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			this.Authorize();

			var res = await this.MagentoServiceLowLevel.GetOrdersAsync( dateFrom, dateTo ).ConfigureAwait( false );
			return res.Orders;
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync()
		{
			this.Authorize();

			var res = await this.MagentoServiceLowLevel.GetOrdersAsync().ConfigureAwait( false );
			return res.Orders;
		}

		public async Task< IEnumerable< Product > > GetProductsAsync()
		{
			this.Authorize();

			var res = await this.MagentoServiceLowLevel.GetProductsAsync().ConfigureAwait( false );
			return res.Products;
		}

		public async Task UpdateProductsAsync( IEnumerable< InventoryItem > products )
		{
			this.Authorize();

			await this.MagentoServiceLowLevel.PutInventoryAsync(products).ConfigureAwait(false);
		}

		private void Authorize()
		{
			if( string.IsNullOrWhiteSpace( this.MagentoServiceLowLevel.AccessToken ) )
			{
				var authorizeTask = this.MagentoServiceLowLevel.PopulateAccessToken();
				authorizeTask.Wait();

				if( this.AfterGettingToken != null )
					this.AfterGettingToken.Invoke( this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret );
			}
		}
	}
}