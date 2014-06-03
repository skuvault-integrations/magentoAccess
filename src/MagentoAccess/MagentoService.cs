using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.Credentials;
using MagentoAccess.Models.Services.PutStockItems;
using MagentoAccess.Services;
using Netco.Extensions;

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
			return res.Orders.Select( x => new Order( x ) );
		}

		public async Task< IEnumerable< Order > > GetOrdersAsync()
		{
			this.Authorize();

			var res = await this.MagentoServiceLowLevel.GetOrdersAsync().ConfigureAwait( false );
			return res.Orders.Select( x => new Order( x ) );
		}

		public async Task< IEnumerable< Product > > GetProductsSimpleAsync()
		{
			this.Authorize();

			return await this.GetProductsWithIdSkuNameDescriptionPriceAsync();
		}

		public async Task< IEnumerable< Product > > GetProductsAsync()
		{
			this.Authorize();

			var productsWithQty = await this.GetProductsWithIdQty().ConfigureAwait( false );

			var productsWithSku = await this.GetProductsWithIdSkuNameDescriptionPriceAsync().ConfigureAwait( false );

			var res = from pq in productsWithQty join ps in productsWithSku on pq.EntityId equals ps.EntityId select new Product() { Description = ps.Description, EntityId = ps.EntityId, Name = ps.Name, Sku = ps.Sku, Price = ps.Price, Qty = pq.Qty };

			return res;
		}

		public async Task UpdateInventoryAsync( IEnumerable< Inventory > products )
		{
			this.Authorize();

			var inventoryItems = products.Select( x => new StockItem()
			{
				ItemId = x.ItemId,
				MinQty = x.MinQty,
				ProductId = x.ProductId,
				Qty = x.Qty,
				StockId = x.StockId,
			} );
			await this.MagentoServiceLowLevel.PutStockItemsAsync( inventoryItems ).ConfigureAwait( false );
		}

		private async Task< IEnumerable< Product > > GetProductsWithIdSkuNameDescriptionPriceAsync()
		{
			var page = 1;
			const int itemsPerPage = 100;

			var getProductsResponse = await this.MagentoServiceLowLevel.GetProductsAsync( page, itemsPerPage ).ConfigureAwait( false );

			var productsChunk = getProductsResponse.Products;
			if( productsChunk.Count() < itemsPerPage )
				return productsChunk.Select( x => new Product { Sku = x.Sku, Description = x.Description, EntityId = x.EntityId, Name = x.Name, Price = x.Price } );

			var receivedProducts = new List< Models.Services.GetProducts.Product >();

			var lastReceiveProducts = productsChunk;

			bool isLastAndCurrentResponsesHaveTheSameProducts;

			do
			{
				receivedProducts.AddRange( productsChunk );

				var getProductsTask = this.MagentoServiceLowLevel.GetProductsAsync( ++page, itemsPerPage );
				getProductsTask.Wait();
				productsChunk = getProductsTask.Result.Products;

				//var repeatedItems = from l in lastReceiveProducts join c in productsChunk on l.EntityId equals c.EntityId select l;
				var repeatedItems = from c in productsChunk join l in lastReceiveProducts on c.EntityId equals l.EntityId select l;

				lastReceiveProducts = productsChunk;

				isLastAndCurrentResponsesHaveTheSameProducts = repeatedItems.Any();

				// try to get items that was added before last iteration
				if( isLastAndCurrentResponsesHaveTheSameProducts )
				{
					var notRrepeatedItems = productsChunk.Where( x => !repeatedItems.Exists( r => r.EntityId == x.EntityId ) );
					receivedProducts.AddRange( notRrepeatedItems );
				}
			} while( !isLastAndCurrentResponsesHaveTheSameProducts );

			return receivedProducts.Select( x => new Product { Sku = x.Sku, Description = x.Description, EntityId = x.EntityId, Name = x.Name, Price = x.Price } );
		}

		private async Task< IEnumerable< Product > > GetProductsWithIdQty()
		{
			var page = 1;
			const int itemsPerPage = 100;

			var getProductsResponse = await this.MagentoServiceLowLevel.GetStockItemsAsync( page, itemsPerPage ).ConfigureAwait( false );

			var productsChunk = getProductsResponse.Items;
			if( productsChunk.Count() < itemsPerPage )
				return productsChunk.Select( x => new Product { EntityId = x.ItemId } );

			var receivedProducts = new List< Models.Services.GetStockItems.StockItem >();

			var lastReceiveProducts = productsChunk;

			bool isLastAndCurrentResponsesHaveTheSameProducts;

			do
			{
				receivedProducts.AddRange( productsChunk );

				var getProductsTask = this.MagentoServiceLowLevel.GetStockItemsAsync( ++page, itemsPerPage );
				getProductsTask.Wait();
				productsChunk = getProductsTask.Result.Items;

				//var repeatedItems = from l in lastReceiveProducts join c in productsChunk on l.EntityId equals c.EntityId select l;
				//todo: return qty too
				var repeatedItems = from c in productsChunk join l in lastReceiveProducts on new { ItemId = c.ItemId, BackOrders = c.BackOrders, Qty = c.Qty } equals new { ItemId = l.ItemId, BackOrders = l.BackOrders, Qty = l.Qty } select l;

				lastReceiveProducts = productsChunk;

				isLastAndCurrentResponsesHaveTheSameProducts = repeatedItems.Any();

				// try to get items that was added before last iteration
				if( isLastAndCurrentResponsesHaveTheSameProducts )
				{
					var notRrepeatedItems = productsChunk.Where( x => !repeatedItems.Exists( r => new { ItemId = r.ItemId, BackOrders = r.BackOrders, Qty = r.Qty } != new { ItemId = x.ItemId, BackOrders = x.BackOrders, Qty = x.Qty } ) );
					receivedProducts.AddRange( notRrepeatedItems );
				}
			} while( !isLastAndCurrentResponsesHaveTheSameProducts );

			return receivedProducts.Select( x => new Product { EntityId = x.ItemId, Qty = x.Qty } );
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