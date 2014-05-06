using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using LINQtoCSV;

namespace MagentoAccess
{
	public class MagentoServiceLowLevelOauth
	{
		private string _requestTokenUrl = "http://192.168.0.104/magento/oauth/initiate";
		private HttpDeliveryMethods _requestTokenHttpDeliveryMethod = HttpDeliveryMethods.PostRequest;
		private string _authorizeUrl = "http://192.168.0.104/magento/admin/oauth_authorize";
		private string _accessTokenUrl = "http://192.168.0.104/magento/oauth/token";
		private HttpDeliveryMethods _accessTokenHttpDeliveryMethod = HttpDeliveryMethods.PostRequest;
		private string _resourceUrl = "http://192.168.0.104/magento/api/rest/products";
		private string _consumerKey = "59704e7d6b9abd742de255b7c97421f6";
		private string _consumerSecretKey = "476130ddd7cdf5709fd9a95bee24b71d";

		private DesktopConsumer _consumer;
		private string _accessToken;
		private HttpDeliveryMethods _authorizationHeaderRequest;

		public event AuthorizationEventHandler AuthorizeCompleted;

		public static string GetAccessToken()
		{
			var cc = new CsvContext();
			return Enumerable.FirstOrDefault( cc.Read< FlatCsvLine >( @"..\..\Files\magento_VerifierCode.csv", new CsvFileDescription { FirstLineHasColumnNames = true } ) ).VerifierCode;
		}

		public static void SaveAccessToken( string accessToken )
		{
			var cc = new CsvContext();
			cc.Write< FlatCsvLine >(
				new List< FlatCsvLine > { new FlatCsvLine { VerifierCode = accessToken } },
				@"..\..\Files\magento_VerifierCode.csv" );
		}

		public void Authorize()
		{
			//todo: before get new authorizationm try read from disk
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
					authorizer.AuthorizationFinished += ( x, y ) =>
					{
						this._accessToken = authorizer.AccessToken;
					};
					authorizer.AuthorizationFinished += ( x, y ) => this.AuthorizeCompleted.Invoke( this, new AuthorizationEventArgs() );
				}
			}
			catch( ProtocolException ex )
			{
			}
		}

		public string InvokeGetCall( bool needAuthorise = false )
		{
			var serverResponse = string.Empty;
			try
			{
				//this._authorizationHeaderRequest =  HttpDeliveryMethods.PostRequest;
				this._authorizationHeaderRequest = HttpDeliveryMethods.GetRequest;

				if( needAuthorise )
					this._authorizationHeaderRequest |= HttpDeliveryMethods.AuthorizationHeaderRequest;

				var resourceEndpoint = new MessageReceivingEndpoint( this._resourceUrl, this._authorizationHeaderRequest );
				using( var resourceResponse = this._consumer.PrepareAuthorizedRequestAndSend( resourceEndpoint, this._accessToken ) )
					serverResponse = resourceResponse.GetResponseReader().ReadToEnd();
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

	public delegate void AuthorizationEventHandler( object sender, AuthorizationEventArgs args );

	public class AuthorizationEventArgs
	{
	}

	public partial class Authorize
	{
		private DesktopConsumer consumer;
		private string requestToken;
		private string verificationKey;
		internal string AccessToken { get; set; }
		public event AuthorizationEventHandler AuthorizationFinished;

		internal Authorize( DesktopConsumer consumer )
		{
			this.consumer = consumer;
			var browserAuthorizationLocation = consumer.RequestUserAuthorization( null, null, out this.requestToken );
			Process.Start( browserAuthorizationLocation.AbsoluteUri );

			Task.Factory.StartNew( () =>
			{
				var counter = 0;
				var accessToken = string.Empty;
				do
				{
					Task.Delay( 2000 );
					counter++;
					accessToken = MagentoServiceLowLevelOauth.GetAccessToken();
				} while( string.IsNullOrWhiteSpace( accessToken ) || counter > 300 );

				if( !string.IsNullOrWhiteSpace( accessToken ) )
				{
					this.finish( accessToken );
					this.AuthorizationFinished.Invoke( this, new AuthorizationEventArgs() );
				}
			} );
		}

		private void finish( string verifKey )
		{
			this.verificationKey = verifKey;
			var grantedAccess = this.consumer.ProcessUserAuthorization( this.requestToken, this.verificationKey );
			this.AccessToken = grantedAccess.AccessToken;
		}
	}
}