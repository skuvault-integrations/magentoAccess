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
			if( string.IsNullOrWhiteSpace( this.MagentoServiceLowLevel.AccessToken ) )
			{
				var authorizeTask = this.MagentoServiceLowLevel.PopulateAccessToken();
				authorizeTask.Wait();

				if( this.AfterGettingToken != null )
					this.AfterGettingToken.Invoke( this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret );
			}

			//todo: filter by date
			var res = this.MagentoServiceLowLevel.GetOrders();
			return res.Orders;
		}

		public IEnumerable< Product > GetProducts()
		{
			if( string.IsNullOrWhiteSpace( this.MagentoServiceLowLevel.AccessToken ) )
			{
				var authorizeTask = this.MagentoServiceLowLevel.PopulateAccessToken();
				authorizeTask.Wait();

				if( this.AfterGettingToken != null )
					this.AfterGettingToken.Invoke( this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret );
			}

			//todo: filter by date
			var res = this.MagentoServiceLowLevel.GetProducts();
			return res.Products;
		}

		public void UpdateProducts( IEnumerable< InventoryItem > products )
		{
			if (string.IsNullOrWhiteSpace(this.MagentoServiceLowLevel.AccessToken))
			{
				var authorizeTask = this.MagentoServiceLowLevel.PopulateAccessToken();
				authorizeTask.Wait();

				if (this.AfterGettingToken != null)
					this.AfterGettingToken.Invoke(this.MagentoServiceLowLevel.AccessToken, this.MagentoServiceLowLevel.AccessTokenSecret);
			}

			this.MagentoServiceLowLevel.PutInventory(products);
		}
	}
}