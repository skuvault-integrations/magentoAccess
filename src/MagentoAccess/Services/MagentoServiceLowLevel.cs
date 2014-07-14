using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.GetOrders;
using MagentoAccess.Models.Services.GetProduct;
using MagentoAccess.Models.Services.GetProducts;
using MagentoAccess.Models.Services.GetStockItems;
using MagentoAccess.Models.Services.PutStockItems;
using MagentoAccess.Services.Parsers;
using StockItem = MagentoAccess.Models.Services.PutStockItems.StockItem;

namespace MagentoAccess.Services
{
	internal class MagentoServiceLowLevel : IMagentoServiceLowLevel
	{
		private readonly string _requestTokenUrl;
		private readonly HttpDeliveryMethods _requestTokenHttpDeliveryMethod;
		private readonly string _authorizeUrl;
		private readonly string _accessTokenUrl;
		private readonly HttpDeliveryMethods _accessTokenHttpDeliveryMethod;
		private readonly string _baseMagentoUrl;
		private const string RestApiUrl = "api/rest";
		private readonly string _consumerKey;
		private readonly string _consumerSecretKey;

		private DesktopConsumer _consumer;
		private string _accessToken;
		private string _accessTokenSecret;

		private AuthenticationManager _authenticationManager;
		private InMemoryTokenManager _tokenManager;

		public TransmitVerificationCodeDelegate TransmitVerificationCode { get; set; }

		public delegate string SaveVerifierCodeAct( string verifierCode );

		public SaveVerifierCodeAct saveVerifierCodeAct { get; set; }

		protected IWebRequestServices webRequestServices { get; set; }

		public string AccessToken
		{
			get { return this._accessToken; }
		}

		public string AccessTokenSecret
		{
			get { return this._accessTokenSecret; }
		}

		public string RequestToken
		{
			get { return this._authenticationManager.RequestToken; }
		}

		public string GetVerifierCode()
		{
			if( this.TransmitVerificationCode != null )
				return this.TransmitVerificationCode.Invoke();

			return string.Empty;
		}

		public void SaveVerifierCode( string verifierCode )
		{
			if( this.saveVerifierCodeAct != null )
				this.saveVerifierCodeAct.Invoke( verifierCode );
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

		public virtual async Task InitiateDescktopAuthenticationProcess()
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
					var authorizer = new AuthenticationManager( this._consumer );
					authorizer.TransmitVerificationCode = this.TransmitVerificationCode;
					var verifiedCode = await authorizer.StartBrowserToGetVerificationCodeAsync().ConfigureAwait( false );
					this._accessToken = authorizer.GetAccessToken( verifiedCode );
					this._accessTokenSecret = tokenManager.GetTokenSecret( this._accessToken );
				}
			}
			catch( Exception ex )
			{
				throw new MagentoRestException( "An exception occured in PopulateAccessToken", ex );
			}
		}

		public virtual VerificationData RequestVerificationUri()
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

				this._tokenManager = new InMemoryTokenManager();
				this._tokenManager.ConsumerKey = this._consumerKey;
				this._tokenManager.ConsumerSecret = this._consumerSecretKey;

				this._consumer = new DesktopConsumer( service, this._tokenManager );

				this._accessToken = string.Empty;

				this._authenticationManager = new AuthenticationManager( this._consumer );

				var verificationUri = this._authenticationManager.GetVerificationUri();

				var tokenSecret = this._tokenManager.GetTokenSecret( this._authenticationManager.RequestToken );

				return new VerificationData { RequestToken = this._authenticationManager.RequestToken, RequestTokenSecret = tokenSecret, Uri = verificationUri };
			}
				//catch (ProtocolException)
			catch( Exception ex )
			{
				throw new MagentoRestAuthException( "An exception occured while attempting to get to get 'Verification URI'", ex );
			}
		}

		public virtual void PopulateAccessTokenAndAccessTokenSecret( string verificationCode, string requestToken, string requestTokenSecret )
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
				tokenManager.tokensAndSecrets[ requestToken ] = requestTokenSecret;

				this._consumer = new DesktopConsumer( service, tokenManager );

				this._accessToken = string.Empty;

				var authorizer = new AuthenticationManager( this._consumer, requestToken );

				this._accessToken = authorizer.GetAccessToken( verificationCode );
				this._accessTokenSecret = tokenManager.GetTokenSecret( this._accessToken );
			}
			catch( Exception ex )
			{
				throw new MagentoRestAuthException( "An exception occured while attempting to  populate access token and access token secret", ex );
			}
		}

		public virtual async Task< GetProductResponse > GetProductAsync( string id )
		{
			try
			{
				return await this.InvokeCallAsync< MagentoProductResponseParser, GetProductResponse >( string.Format( "products/{0}", id ), true ).ConfigureAwait( false );
			}
			catch( Exception exc )
			{
				throw new MagentoRestException( string.Format( "An error occured during GetProductAsync{0}", id ), exc );
			}
		}

		public virtual async Task< GetProductsResponse > GetProductsAsync( int page, int limit, bool thrownExc = false )
		{
			try
			{
				var limitFilter = string.Format( "page={0}&limit={1}", page, limit );
				var resultUrl = string.Format( "{0}?{1}", "products", limitFilter );
				return await this.InvokeCallAsync< MagentoProductsResponseParser, GetProductsResponse >( resultUrl, true, throwException : thrownExc ).ConfigureAwait( false );
			}
			catch( Exception exc )
			{
				if( thrownExc )
					throw new MagentoRestException( string.Format( "An error occured during GetProductsAsync(page:{0},limit:{1})", page, limit ), exc );
				else
				{
					MagentoLogger.Log().Trace( exc, string.Format( "[magento] GetProductsAsyncc(page:{0},limit:{1}) throw an exception .", page, limit ) );
					return new GetProductsResponse();
				}
			}
		}

		public virtual async Task< GetStockItemsResponse > GetStockItemsAsync( int page, int limit )
		{
			try
			{
				var limitFilter = string.Format( "page={0}&limit={1}", page, limit );
				var resultUrl = string.Format( "{0}?{1}", "stockitems", limitFilter );
				return await this.InvokeCallAsync< MegentoInventoryResponseParser, GetStockItemsResponse >( resultUrl, true ).ConfigureAwait( false );
			}
			catch( Exception exc )
			{
				throw new MagentoRestException( string.Format( "An error occured during GetStockItemsAsync(page:{0},limit{1})", page, limit ), exc );
			}
		}

		public virtual async Task< PutStockItemsResponse > PutStockItemsAsync( IEnumerable< StockItem > inventoryItems )
		{
			try
			{
				var inventoryItemsFormated = inventoryItems.Select( x =>
				{
					Condition.Requires( x.ItemId ).IsNotNullOrWhiteSpace();

					var productIdSection = string.IsNullOrWhiteSpace( x.ProductId ) ? string.Empty : string.Format( "<product_id>{0}</product_id>", x.ProductId );

					var stockIdSection = string.IsNullOrWhiteSpace( x.StockId ) ? string.Empty : string.Format( "<stock_id>{0}</stock_id>", x.StockId );

					var res = string.Format( "<data_item item_id=\"{0}\">{1}{2}<qty>{3}</qty><min_qty>{4}</min_qty></data_item>", x.ItemId, productIdSection, stockIdSection, x.Qty, x.MinQty );

					return res;
				} ).ToList();

				var inventoryItemsAggregated = string.Concat( inventoryItemsFormated );

				return await this.InvokeCallAsync< MegentoPutInventoryResponseParser, PutStockItemsResponse >( "stockitems", true, HttpDeliveryMethods.PutRequest, string.Format( "<?xml version=\"1.0\"?><magento_api>{0}</magento_api>", inventoryItemsAggregated ) ).ConfigureAwait( false );
			}
			catch( Exception exc )
			{
				var productsBriefInfo = string.Join( "|", inventoryItems.Select( x => string.Format( "ItemId:{0}, ProductId:{1}, Qty:{2}, StockId:{3}", x.ItemId, x.ProductId, x.Qty, x.StockId ) ) );
				throw new MagentoRestException( string.Format( "An error occured during PutStockItemsAsync({0})", productsBriefInfo ), exc );
			}
		}

		public virtual async Task< GetOrdersResponse > GetOrdersAsync()
		{
			try
			{
				return await this.InvokeCallAsync< MegentoOrdersResponseParser, GetOrdersResponse >( "orders", true ).ConfigureAwait( false );
			}
			catch( Exception exc )
			{
				throw new MagentoRestException( string.Format( "An error occured during GetOrdersAsync()" ), exc );
			}
		}

		public virtual async Task< GetOrdersResponse > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			try
			{
				var filterUrl = string.Format( "filter[1][attribute]=created_at&filter[1][from]={0}&filter[1][to]={1}", dateFrom.ToUrlParameterString(), dateTo.ToUrlParameterString() );
				var url = string.Format( "{0}?{1}", "orders", filterUrl );
				return await this.InvokeCallAsync< MegentoOrdersResponseParser, GetOrdersResponse >( url, true ).ConfigureAwait( false );
			}
			catch( Exception exc )
			{
				throw new MagentoRestException( string.Format( "An error occured during GetOrdersAsync(datefrom:{0},dateTo:{1})", dateFrom, dateTo ), exc );
			}
		}

		protected TParsed InvokeCall< TParser, TParsed >( string partialUrl, bool needAuthorise = false, HttpDeliveryMethods requestType = HttpDeliveryMethods.GetRequest, string body = null ) where TParser : IMagentoBaseResponseParser< TParsed >, new()
		{
			var res = default( TParsed );
			try
			{
				ActionPolicies.Get.Do( () =>
				{
					var webRequest = this.CreateMagentoStandartRequest( partialUrl, needAuthorise, requestType, body );
					using( var memStream = this.webRequestServices.GetResponseStream( webRequest ) )
						res = new TParser().Parse( memStream, false );
				} );

				return res;
			}
			catch( Exception exc )
			{
				throw new MagentoRestException( string.Format( "InvokeCall(partialUrl:{0},needAuthorise{1}) throw an exception.", partialUrl, needAuthorise ), exc );
			}
		}

		protected async Task< TParsed > InvokeCallAsync< TParser, TParsed >( string partialUrl, bool needAuthorise = false, HttpDeliveryMethods requestType = HttpDeliveryMethods.GetRequest, string body = null, bool throwException = false ) where TParser : IMagentoBaseResponseParser< TParsed >, new()
		{
			var res = default( TParsed );
			try
			{
				await ActionPolicies.GetAsync.Do( async () =>
				{
					var webRequest = this.CreateMagentoStandartRequest( partialUrl, needAuthorise, requestType, body );
					using( var memStream = await this.webRequestServices.GetResponseStreamAsync( webRequest ).ConfigureAwait( false ) )
						res = new TParser().Parse( memStream, false );
				} ).ConfigureAwait( false );

				return res;
			}
			catch( Exception exception )
			{
				if( throwException )
					throw new MagentoRestException( string.Format( "InvokeCall(partialUrl:{0},needAuthorise{1}) throw an exception.", partialUrl, needAuthorise ), exception );
				else
					MagentoLogger.Log().Trace( exception, "[magento] Invoke call async partial url:{0} throw an exception .", partialUrl );
			}

			return res;
		}

		protected HttpWebRequest CreateMagentoStandartRequest( string partialUrl, bool needAuthorise, HttpDeliveryMethods requestType, string body )
		{
			var urlParrts = new List< string > { this._baseMagentoUrl, RestApiUrl, partialUrl }.Where( x => !string.IsNullOrWhiteSpace( x ) ).ToList();

			var locationUri = urlParrts.BuildUrl();

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

			this.webRequestServices.PopulateRequestByBody( body, webRequest );

			return webRequest;
		}
	}

	public class VerificationData
	{
		public Uri Uri { get; set; }
		public string RequestToken { get; set; }
		public string RequestTokenSecret { get; set; }
	}

	internal class AuthenticationManager
	{
		private readonly DesktopConsumer consumer;
		private string requestToken;
		private string verificationKey;

		internal string AccessToken { get; set; }

		public string RequestToken
		{
			get { return this.requestToken; }
		}

		public TransmitVerificationCodeDelegate TransmitVerificationCode { get; set; }

		internal AuthenticationManager( DesktopConsumer consumer )
		{
			this.consumer = consumer;
		}

		public AuthenticationManager( DesktopConsumer consumer, string requestToken )
		{
			this.consumer = consumer;
			this.requestToken = requestToken;
		}

		public Uri GetVerificationUri()
		{
			var verificationUri = this.consumer.RequestUserAuthorization( null, null, out this.requestToken );
			return verificationUri;
		}

		public async Task< string > StartBrowserToGetVerificationCodeAsync()
		{
			var browserAuthorizationLocation = this.GetVerificationUri();
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
						tempVerifierCode = this.TransmitVerificationCode.Invoke();
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

	public delegate string TransmitVerificationCodeDelegate();

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