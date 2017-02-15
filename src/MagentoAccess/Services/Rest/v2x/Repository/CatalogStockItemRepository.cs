using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Netco.Extensions;
using Newtonsoft.Json;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public class CatalogStockItemRepository : ICatalogStockItemRepository
	{
		private AuthorizationToken Token { get; }
		private MagentoUrl Url { get; }

		public CatalogStockItemRepository( AuthorizationToken token, MagentoUrl url )
		{
			this.Url = url;
			this.Token = token;
		}

		public async Task< bool > PutStockItemAsync( string productSku, string itemId, RootObject stockItem )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Put )
				.Path( MagentoServicePath.Create(
					MagentoServicePath.Products.RepositoryPath +
					$"/{productSku}/" +
					MagentoServicePath.CatalogStockItems.RepositoryPath +
					$"/{itemId}" ) )
				.Body( JsonConvert.SerializeObject( stockItem, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore } ) )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync( Mark.CreateNew() ).ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< int >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() ) == 1;
				}
			} );
		}

		public async Task< IEnumerable< bool > > PutStockItemsAsync( IEnumerable< Tuple< string, string, RootObject > > items )
		{
			var tailProducts = await items.ProcessInBatchAsync( 10, async x => await this.PutStockItemAsync( x.Item1, x.Item2, x.Item3 ).ConfigureAwait( false ) ).ConfigureAwait( false );

			return tailProducts;
		}

		public async Task< StockItem > GetStockItemAsync( string productSku )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Create(
					MagentoServicePath.CatalogStockItems.RepositoryPath +
					$"/{productSku}" ) )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await webRequest.RunAsync( Mark.CreateNew() ).ConfigureAwait( false ) )
				{
					return JsonConvert.DeserializeObject< StockItem >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} );
		}

		public async Task< IEnumerable< StockItem > > GetStockItemsAsync( IEnumerable< string > productSku )
		{
			var tailProducts = await productSku.ProcessInBatchAsync( 10, async x => await this.GetStockItemAsync( x ).ConfigureAwait( false ) ).ConfigureAwait( false );

			return tailProducts;
		}
	}
}