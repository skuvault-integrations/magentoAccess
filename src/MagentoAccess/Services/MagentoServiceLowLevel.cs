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
using LINQtoCSV;
using MagentoAccess.Models.GetInventory;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProduct;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Services.Parsers;

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
		private string _restApiUrl = "api/rest";
		private string _consumerKey;
		private string _consumerSecretKey;

		private DesktopConsumer _consumer;
		private string _accessToken;
		private string _accessTokenSecret;
		private HttpDeliveryMethods _authorizationHeaderRequest;

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
		}

		public async Task GetAccessToken()
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
			catch( ProtocolException ex )
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

		public GetInventoryResponse GetInventory()
		{
			return this.InvokeCall< MegentoInventoryResponseParser, GetInventoryResponse >( "stockitems", true );
		}

		public PutInventoryResponse PutInventory()
		{
			return this.InvokeCall< MegentoPutInventoryResponseParser, PutInventoryResponse >( "stockitems", true, HttpDeliveryMethods.PutRequest, "<?xml version=\"1.0\"?><magento_api><data_item item_id=\"1\"><product_id>1</product_id><stock_id>1</stock_id><qty>100.0000</qty><min_qty>0.0000</min_qty></data_item></magento_api>" );
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

				var webRequestServices = new WebRequestServices();

				using( var memStream = webRequestServices.GetResponseStream( webRequest ) )
				{
					res = new TParser().Parse( memStream, false );
					return res;
				}
			}
			catch( ProtocolException ex )
			{
				//todo: log
			}

			return res;
		}

		protected HttpWebRequest CreateMagentoStandartRequest( string partialUrl, bool needAuthorise, HttpDeliveryMethods requestType, string body )
		{
			var urlParrts = new List< string > { this._baseMagentoUrl, this._restApiUrl, partialUrl }.Where( x => !string.IsNullOrWhiteSpace( x ) ).ToList();
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

			//s
			byte[] encodedBody=null;
			if( !string.IsNullOrWhiteSpace( body ) )
				encodedBody = new UTF8Encoding().GetBytes( body );
			//e

			var webRequest = this._consumer.PrepareAuthorizedRequest( resourceEndpoint, this._accessToken );

			//s
			if (encodedBody != null)
			{
				webRequest.ContentLength = encodedBody.Length;
				webRequest.ContentType = "text/xml";
			}
			//e

			webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

			//s
			if (encodedBody != null)
			{
				var getRequestStremTask = webRequest.GetRequestStreamAsync();
				getRequestStremTask.Wait();
				using (var newStream = getRequestStremTask.Result)
					newStream.Write(encodedBody, 0, encodedBody.Length);
			}
			//e

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
}