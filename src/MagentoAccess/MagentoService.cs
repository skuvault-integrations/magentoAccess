using System;
using System.Collections.Generic;
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

		public IEnumerable< Order > GetOrders( DateTime dateFrom, DateTime dateTo )
		{
			this.Authorize();

			//todo: filter by date
			//http://www.magentocommerce.com/api/rest/get_filters.html
			//http://192.168.0.104/magento/api/rest/orders?filter[1][attribute]=created_at&filter[1][from]=2013-05-09%2004:53:43&filter[1][to]=2015-05-09%2004:53:43
			var res = this.MagentoServiceLowLevel.GetOrders(dateFrom, dateTo);
			return res.Orders;
		}

		public IEnumerable< Product > GetProducts()
		{
			this.Authorize();

			//todo: filter by date
			var res = this.MagentoServiceLowLevel.GetProducts();
			return res.Products;
		}

		public void UpdateProducts( IEnumerable< InventoryItem > products )
		{
			this.Authorize();

			this.MagentoServiceLowLevel.PutInventory( products );
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