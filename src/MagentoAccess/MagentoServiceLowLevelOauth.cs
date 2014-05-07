using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using LINQtoCSV;

namespace MagentoAccess
{
	public class MagentoServiceLowLevelOauth
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

		////todo: devide constructor on 2 with access token and with URL
		//public MagentoServiceLowLevelOauth(
		//	string consumerKey,
		//	string consumerSecretKey,
		//	string accessToken = null,
		//	string accessTokenSecret = null,
		//	string requestTokenUrl = "http://192.168.0.104/magento/oauth/initiate",
		//	string authorizeUrl = "http://192.168.0.104/magento/admin/oauth_authorize",
		//	string accessTokenUrl = "http://192.168.0.104/magento/oauth/token",
		//	string resourceUrl = "http://192.168.0.104/magento/api/rest/products"
		//	)
		//{
		//	Condition.Ensures( consumerKey, "consumerKey" ).IsNotNullOrWhiteSpace();
		//	Condition.Ensures( consumerSecretKey, "consumerSecretKey" ).IsNotNullOrWhiteSpace();

		//	if( accessToken == null || accessTokenSecret == null )
		//	{
		//		Condition.Ensures( requestTokenUrl, "requestTokenUrl" ).IsNotNullOrWhiteSpace();
		//		Condition.Ensures( authorizeUrl, "authorizeUrl" ).IsNotNullOrWhiteSpace();
		//		Condition.Ensures( accessTokenUrl, "accessTokenUrl" ).IsNotNullOrWhiteSpace();
		//	}

		//	this._consumerKey = consumerKey;
		//	this._consumerSecretKey = consumerSecretKey;
		//	this._accessToken = accessToken;
		//	this._accessTokenSecret = accessTokenSecret;
		//	this._requestTokenUrl = requestTokenUrl;
		//	this._requestTokenHttpDeliveryMethod = HttpDeliveryMethods.PostRequest;
		//	this._authorizeUrl = authorizeUrl;
		//	this._accessTokenUrl = accessTokenUrl;
		//	this._accessTokenHttpDeliveryMethod = HttpDeliveryMethods.PostRequest;
		//	this._resourceUrl = resourceUrl;
		//}
		
		//string baseMagentoUrl = "http://192.168.0.104/magento/api/rest",
		//string requestTokenUrl = "http://192.168.0.104/magento/oauth/initiate",
		//string authorizeUrl = "http://192.168.0.104/magento/admin/oauth_authorize",
		//string accessTokenUrl = "http://192.168.0.104/magento/oauth/token"
		public MagentoServiceLowLevelOauth(
			string consumerKey,
			string consumerSecretKey,
			string baseMagentoUrl,
			string requestTokenUrl ,
			string authorizeUrl ,
			string accessTokenUrl
			)
		{
			Condition.Ensures( consumerKey, "consumerKey" ).IsNotNullOrWhiteSpace();
			Condition.Ensures( consumerSecretKey, "consumerSecretKey" ).IsNotNullOrWhiteSpace();
			Condition.Ensures(baseMagentoUrl, "baseMagentoUrl").IsNotNullOrWhiteSpace();
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

		public MagentoServiceLowLevelOauth(
			string consumerKey,
			string consumerSecretKey,
			string baseMagentoUrl,
			string accessToken,
			string accessTokenSecret
			)
		{
			Condition.Ensures( consumerKey, "consumerKey" ).IsNotNullOrWhiteSpace();
			Condition.Ensures(consumerSecretKey, "consumerSecretKey").IsNotNullOrWhiteSpace();
			Condition.Ensures(baseMagentoUrl, "baseMagentoUrl").IsNotNullOrWhiteSpace();
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

		public string InvokeGetCall( string partialUrl, bool needAuthorise = false, HttpDeliveryMethods requestType = HttpDeliveryMethods.GetRequest )
		{
			var serverResponse = string.Empty;
			try
			{
				var urlParrts = new List<string> { this._baseMagentoUrl, this._restApiUrl, partialUrl }.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
				var locationUri = string.Join("/", urlParrts);
				var resourceEndpoint = new MessageReceivingEndpoint(locationUri, needAuthorise ? requestType | HttpDeliveryMethods.AuthorizationHeaderRequest : requestType);
				
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
				
				var req = _consumer.PrepareAuthorizedRequest(resourceEndpoint, this._accessToken);
				req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
				
				var webRequestServices = new WebRequestServices();
				string res;
				using (var memStream = webRequestServices.GetResponseStream(req))
				{
					byte[] temp = new byte[memStream.Length];
					var v = memStream.Read(temp, 0,(int) memStream.Length);

					res = Encoding.UTF8.GetString(temp);
				}

				return res;
			}
			catch( ProtocolException ex )
			{
			}

			return serverResponse;
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
						tempVerifierCode = MagentoServiceLowLevelOauth.GetVerifierCode();
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