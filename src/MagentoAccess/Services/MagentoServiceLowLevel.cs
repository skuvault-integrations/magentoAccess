using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using LINQtoCSV;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProduct;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.GetSrockItems;
using MagentoAccess.Models.PutStockItems;
using MagentoAccess.Services.Parsers;
using Netco.Logging;

namespace MagentoAccess.Services
{
	public class MagentoServiceLowLevel : IMagentoServiceLowLevel
	{
		private string _requestTokenUrl;
		private HttpDeliveryMethods _requestTokenHttpDeliveryMethod;
		private string _authorizeUrl;
		private string _accessTokenUrl;
		private HttpDeliveryMethods _accessTokenHttpDeliveryMethod;
		private string _baseMagentoUrl;
		private const string RestApiUrl = "api/rest";
		private string _consumerKey;
		private string _consumerSecretKey;

		private DesktopConsumer _consumer;
		private string _accessToken;
		private string _accessTokenSecret;

		protected IWebRequestServices webRequestServices { get; set; }

		public string AccessToken
		{
			get { return this._accessToken; }
		}

		public string AccessTokenSecret
		{
			get { return this._accessTokenSecret; }
		}

		public static string GetVerifierCode()
		{
			var cc = new CsvContext();
			return Enumerable.FirstOrDefault( cc.Read< FlatCsvLine >( @"..\..\Files\magento_VerifierCode.csv", new CsvFileDescription { FirstLineHasColumnNames = true } ) ).VerifierCode;
		}

		public static void SaveVerifierCode( string verifierCode )
		{
			var cc = new CsvContext();
			cc.Write< FlatCsvLine >(
				new List< FlatCsvLine > { new FlatCsvLine { VerifierCode = verifierCode } },
				@"..\..\Files\magento_VerifierCode.csv" );
		}

		public MagentoServiceLowLevel(
			string consumerKey,
			string consumerSecretKey,
			string baseMagentoUrl,
			string requestTokenUrl,
			string authorizeUrl,
			string accessTokenUrl
			)
		{
			Condition.Ensures( consumerKey, "consumerKey" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( consumerSecretKey, "consumerSecretKey" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( baseMagentoUrl, "baseMagentoUrl" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( requestTokenUrl, "requestTokenUrl" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( authorizeUrl, "authorizeUrl" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( accessTokenUrl, "accessTokenUrl" ).IsNotNullOrWhiteSpace();

			this._consumerKey = consumerKey;
			this._consumerSecretKey = consumerSecretKey;
			this._requestTokenUrl = requestTokenUrl;
			this._requestTokenHttpDeliveryMethod = HttpDeliveryMethods.PostRequest;
			this._authorizeUrl = authorizeUrl;
			this._accessTokenUrl = accessTokenUrl;
			this._accessTokenHttpDeliveryMethod = HttpDeliveryMethods.PostRequest;
			this._baseMagentoUrl = baseMagentoUrl;
			this.webRequestServices = new WebRequestServices();
		}

		public MagentoServiceLowLevel(
			string consumerKey,
			string consumerSecretKey,
			string baseMagentoUrl,
			string accessToken,
			string accessTokenSecret
			)
		{
			Condition.Ensures( consumerKey, "consumerKey" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( consumerSecretKey, "consumerSecretKey" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( baseMagentoUrl, "baseMagentoUrl" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( accessToken, "accessToken" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( accessTokenSecret, "accessTokenSecret" ).IsNotNullOrWhiteSpace();

			this._consumerKey = consumerKey;
			this._consumerSecretKey = consumerSecretKey;
			this._accessToken = accessToken;
			this._accessTokenSecret = accessTokenSecret;
			this._baseMagentoUrl = baseMagentoUrl;
			this.webRequestServices = new WebRequestServices();
		}

		public async Task PopulateAccessToken()
		{
			try
			{
				var service = new ServiceProviderDescription
				{
					RequestTokenEndpoint = new MessageReceivingEndpoint( this._requestTokenUrl, this._requestTokenHttpDeliveryMethod ),
					UserAuthorizationEndpoint = new MessageReceivingEndpoint( this._authorizeUrl, HttpDeliveryMethods.GetRequest ),
					AccessTokenEndpoint = new MessageReceivingEndpoint( this._accessTokenUrl, this._accessTokenHttpDeliveryMethod ),
					TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
					ProtocolVersion = ProtocolVersion.V10a,
				};

				var tokenManager = new InMemoryTokenManager();
				tokenManager.ConsumerKey = this._consumerKey;
				tokenManager.ConsumerSecret = this._consumerSecretKey;

				this._consumer = new DesktopConsumer( service, tokenManager );

				this._accessToken = string.Empty;

				if( service.ProtocolVersion == ProtocolVersion.V10a )
				{
					var authorizer = new Authorize( this._consumer );
					var verifiedCode = await authorizer.GetVerifiedCodeAsync().ConfigureAwait( false );
					this._accessToken = authorizer.GetAccessToken( verifiedCode );
					this._accessTokenSecret = tokenManager.GetTokenSecret( this._accessToken );
				}
			}
			catch( ProtocolException )
			{
			}
		}

		public GetProductResponse GetProduct( string id )
		{
			return this.InvokeCall< MagentoProductResponseParser, GetProductResponse >( string.Format( "products/{0}", id ), true );
		}

		public GetProductsResponse GetProducts()
		{
			return this.InvokeCall< MagentoProductsResponseParser, GetProductsResponse >( "products", true );
		}

		public GetStockItemsResponse GetInventory()
		{
			return this.InvokeCall< MegentoInventoryResponseParser, GetStockItemsResponse >( "stockitems", true );
		}

		public PutStockItemsResponse PutInventory( IEnumerable< InventoryItem > inventoryItems )
		{
			var inventoryItemsFormated = inventoryItems.Select( x =>
			{
				Condition.Requires( x.ItemId ).IsNotNullOrWhiteSpace();

				var productIdSection = string.IsNullOrWhiteSpace( x.ProductId ) ? string.Empty : string.Format( "<product_id>{0}</product_id>", x.ProductId );

				var stockIdSection = string.IsNullOrWhiteSpace( x.StockId ) ? string.Empty : string.Format( "<stock_id>{0}</stock_id>", x.StockId );

				var res = string.Format( "<data_item item_id=\"{0}\">{1}{2}<qty>{3}</qty><min_qty>{4}</min_qty></data_item>", x.ItemId, productIdSection, stockIdSection, x.Qty, x.MinQty );

				return res;
			} );

			var inventoryItemsAggregated = string.Concat( inventoryItemsFormated );

			return this.InvokeCall< MegentoPutInventoryResponseParser, PutStockItemsResponse >( "stockitems", true, HttpDeliveryMethods.PutRequest, string.Format( "<?xml version=\"1.0\"?><magento_api>{0}</magento_api>", inventoryItemsAggregated ) );
		}

		public GetOrdersResponse GetOrders()
		{
			return this.InvokeCall< MegentoOrdersResponseParser, GetOrdersResponse >( "orders", true );
		}

		protected TParsed InvokeCall< TParser, TParsed >( string partialUrl, bool needAuthorise = false, HttpDeliveryMethods requestType = HttpDeliveryMethods.GetRequest, string body = null ) where TParser : IMagentoBaseResponseParser< TParsed >, new()
		{
			var res = default( TParsed );
			try
			{
				var webRequest = this.CreateMagentoStandartRequest( partialUrl, needAuthorise, requestType, body );

				ActionPolicies.Get.Do( () =>
				{
					using( var memStream = this.webRequestServices.GetResponseStream( webRequest ) )
						res = new TParser().Parse( memStream, false );
				} );

				return res;
			}
			catch( ProtocolException )
			{
				this.Log().Trace("[magento] Invoke call partial url:[0} throw an exception .", partialUrl);
			}

			return res;
		}

		protected HttpWebRequest CreateMagentoStandartRequest( string partialUrl, bool needAuthorise, HttpDeliveryMethods requestType, string body )
		{
			var urlParrts = new List< string > { this._baseMagentoUrl, RestApiUrl, partialUrl }.Where( x => !string.IsNullOrWhiteSpace( x ) ).ToList();
			var locationUri = string.Join( "/", urlParrts );
			var resourceEndpoint = new MessageReceivingEndpoint( locationUri, needAuthorise ? requestType | HttpDeliveryMethods.AuthorizationHeaderRequest : requestType );

			var service = new ServiceProviderDescription
			{
				TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
				ProtocolVersion = ProtocolVersion.V10a,
			};

			var tokenManager = new InMemoryTokenManager();
			tokenManager.ConsumerKey = this._consumerKey;
			tokenManager.ConsumerSecret = this._consumerSecretKey;
			tokenManager.tokensAndSecrets[ this._accessToken ] = this._accessTokenSecret;

			this._consumer = new DesktopConsumer( service, tokenManager );

			var webRequest = this._consumer.PrepareAuthorizedRequest( resourceEndpoint, this._accessToken );

			webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

			webRequestServices.PopulateRequestByBody(body, webRequest);

			return webRequest;
		}
	}

	internal class FlatCsvLine
	{
		public FlatCsvLine()
		{
		}

		[ CsvColumn( Name = "VerifierCode", FieldIndex = 1 ) ]
		public string VerifierCode { get; set; }
	}

	public partial class Authorize
	{
		private readonly DesktopConsumer consumer;
		private string requestToken;
		private string verificationKey;
		internal string AccessToken { get; set; }

		internal Authorize( DesktopConsumer consumer )
		{
			this.consumer = consumer;
		}

		public async Task< string > GetVerifiedCodeAsync()
		{
			var browserAuthorizationLocation = this.consumer.RequestUserAuthorization( null, null, out this.requestToken );
			Process.Start( browserAuthorizationLocation.AbsoluteUri );

			var verifierCode = await Task.Factory.StartNew( () =>
			{
				var counter = 0;
				var tempVerifierCode = string.Empty;
				do
				{
					Task.Delay( 2000 );
					counter++;
					try
					{
						tempVerifierCode = MagentoServiceLowLevel.GetVerifierCode();
					}
					catch( Exception )
					{
					}
				} while( string.IsNullOrWhiteSpace( tempVerifierCode ) || counter > 300 );

				return tempVerifierCode;
			} ).ConfigureAwait( false );

			return verifierCode;
		}

		public string GetAccessToken( string verifKey )
		{
			this.verificationKey = verifKey;
			var grantedAccess = this.consumer.ProcessUserAuthorization( this.requestToken, this.verificationKey );
			return this.AccessToken = grantedAccess.AccessToken;
		}
	}

	internal class InMemoryTokenManager : IConsumerTokenManager
	{
		public Dictionary< string, string > tokensAndSecrets = new Dictionary< string, string >();

		internal InMemoryTokenManager()
		{
		}

		public string ConsumerKey { get; internal set; }

		public string ConsumerSecret { get; internal set; }

		#region ITokenManager Members
		public string GetConsumerSecret( string consumerKey )
		{
			if( consumerKey == this.ConsumerKey )
				return this.ConsumerSecret;
			else
				throw new ArgumentException( "Unrecognized consumer key.", "consumerKey" );
		}

		public string GetTokenSecret( string token )
		{
			return this.tokensAndSecrets[ token ];
		}

		public void StoreNewRequestToken( UnauthorizedTokenRequest request, ITokenSecretContainingMessage response )
		{
			this.tokensAndSecrets[ response.Token ] = response.TokenSecret;
		}

		public void ExpireRequestTokenAndStoreNewAccessToken( string consumerKey, string requestToken, string accessToken, string accessTokenSecret )
		{
			this.tokensAndSecrets.Remove( requestToken );
			this.tokensAndSecrets[ accessToken ] = accessTokenSecret;
		}

		/// <summary>
		/// Classifies a token as a request token or an access token.
		/// </summary>
		/// <param name="token">The token to classify.</param>
		/// <returns>Request or Access token, or invalid if the token is not recognized.</returns>
		public TokenType GetTokenType( string token )
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}