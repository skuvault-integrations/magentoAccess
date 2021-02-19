using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Rest.v2x.CatalogStockItemRepository;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Netco.Extensions;
using Netco.Logging;
using Newtonsoft.Json;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public class CatalogStockItemRepository : BaseRepository, ICatalogStockItemRepository
	{
		private AuthorizationToken Token { get; }
		private MagentoUrl Url { get; }
		private MagentoTimeouts OperationsTimeouts { get; }

		public CatalogStockItemRepository( AuthorizationToken token, MagentoUrl url, MagentoTimeouts operationsTimeouts )
		{
			this.Url = url;
			this.Token = token;
			this.OperationsTimeouts = operationsTimeouts;
		}

		public async Task< bool > PutStockItemAsync( string productSku, string itemId, RootObject stockItem, CancellationToken cancellationToken, Mark mark = null )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Put )
				.Path( MagentoServicePath.Create(
					MagentoServicePath.ProductsPath +
					$"/{Uri.EscapeDataString( productSku )}/" +
					MagentoServicePath.StockItemsPath +
					$"/{itemId}" ) )
				.Body( JsonConvert.SerializeObject( stockItem, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore } ) )
				.Timeout( OperationsTimeouts[ MagentoOperationEnum.UpdateStockItemQuantity ] )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{
				return TrackNetworkActivityTime( async () =>
				{
					try
					{
						using( var v = await webRequest.RunAsync( cancellationToken, mark ).ConfigureAwait( false ) )
						{
							var response = new StreamReader( v, Encoding.UTF8 ).ReadToEnd();
							return JsonConvert.DeserializeObject< int >( response ) > 0;
						}
					}
					catch( MagentoWebException exception )
					{
						if( exception.IsNotFoundException() )
						{
							return false;
						}
						throw;
					}
				} );
			} );
		}

		public async Task< IEnumerable< bool > > PutStockItemsAsync( IEnumerable< Tuple< string, string, RootObject > > items, CancellationToken cancellationToken, Mark mark = null )
		{
			var tailProducts = await items.ProcessInBatchAsync( 5, async x => await this.PutStockItemAsync( x.Item1, x.Item2, x.Item3, cancellationToken, mark.CreateChildOrNull() ).ConfigureAwait( false ) ).ConfigureAwait( false );

			return tailProducts;
		}

		public async Task< StockItem > GetStockItemAsync( string productSku, CancellationToken cancellationToken, Mark mark = null )
		{
			var webRequest = ( WebRequest )WebRequest.Create()
				.Method( MagentoWebRequestMethod.Get )
				.Path( MagentoServicePath.Create(
					MagentoServicePath.StockItemsPath +
					$"/{Uri.EscapeDataString( productSku )}" ) )
				.Timeout( OperationsTimeouts[ MagentoOperationEnum.GetStockItemQuantity ] )
				.AuthToken( this.Token )
				.Url( this.Url );

			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{
				return TrackNetworkActivityTime( async () =>
				{
					try
					{
						using( var v = await webRequest.RunAsync( cancellationToken, mark ).ConfigureAwait( false ) )
						{
							return JsonConvert.DeserializeObject< StockItem >( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
						}
					}
					catch( MagentoWebException exception )
					{
						if( exception.IsNotFoundException() )
						{
							return null;
						}
						throw;
					}
				} );
			} );
		}

		public async Task< IEnumerable< StockItem > > GetStockItemsAsync( IEnumerable< string > productSku, CancellationToken cancellationToken, Mark mark = null )
		{
			var tailProducts = await productSku.ProcessInBatchAsync( 5, async x => await this.GetStockItemAsync( x, cancellationToken, mark.CreateChildOrNull() ).ConfigureAwait( false ) ).ConfigureAwait( false );

			return tailProducts;
		}
	}
}