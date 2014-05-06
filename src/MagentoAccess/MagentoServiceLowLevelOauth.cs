using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using MagentoAccess.Models.GetProducts;

namespace MagentoAccess
{
	public class MagentoServiceLowLevelOauth
	{
		private string _requestTokenUrl = "http://192.168.0.104/magento/oauth/initiate";
		private HttpDeliveryMethods _requestTokenHttpDeliveryMethod = HttpDeliveryMethods.PostRequest;
		private string _authorizeUrl = "http://192.168.0.104/magento/admin/oauth/authorize";
		private string _accessTokenUrl = "http://192.168.0.104/magento/oauth/token";
		private HttpDeliveryMethods _accessTokenHttpDeliveryMethod = HttpDeliveryMethods.PostRequest;
		private string _resourceUrl = "http://192.168.0.104/magento/api/rest/products";
		private string _consumerKey = "59704e7d6b9abd742de255b7c97421f6";
		private string _consumerSecretKey = "476130ddd7cdf5709fd9a95bee24b71d";

		private DesktopConsumer _consumer;
		private string _accessToken;
		private HttpDeliveryMethods _authorizationHeaderRequest;

		public IEnumerable< Item > GetItems()
		{
			var items = new List< Item >();

			try
			{
				var serverResponse = string.Empty;

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

				if (service.ProtocolVersion == ProtocolVersion.V10a)
				{
					var authorizePopup = new Authorize( this._consumer, ( DesktopConsumer c, out string requestToken ) => c.RequestUserAuthorization( null, null, out requestToken ) );

					//todo: add some event
					//authorizePopup.Owner = this;
					//bool? result = authorizePopup.ShowDialog();
					//if( result.HasValue && result.Value )
						this._accessToken = authorizePopup.AccessToken;
					//else
					//	return;
				}

				//var resourceHttpMethod =  HttpDeliveryMethods.PostRequest;
				var resourceHttpMethod =  HttpDeliveryMethods.GetRequest;

				bool needAuthorise = false;
				if (needAuthorise)
					resourceHttpMethod |= HttpDeliveryMethods.AuthorizationHeaderRequest;

				var resourceEndpoint = new MessageReceivingEndpoint(this._resourceUrl, resourceHttpMethod);
				using( var resourceResponse = this._consumer.PrepareAuthorizedRequestAndSend( resourceEndpoint, this._accessToken ) )
					serverResponse = resourceResponse.GetResponseReader().ReadToEnd();
			}
			catch( ProtocolException ex )
			{
				//MessageBox.Show( this, ex.Message );
			}

			return items;
		}

		public IEnumerable<Item> InvokeAuthorization()
		{
			var items = new List<Item>();

			try
			{
				var service = new ServiceProviderDescription
				{
					RequestTokenEndpoint = new MessageReceivingEndpoint(this._requestTokenUrl, this._requestTokenHttpDeliveryMethod),
					UserAuthorizationEndpoint = new MessageReceivingEndpoint(this._authorizeUrl, HttpDeliveryMethods.GetRequest),
					AccessTokenEndpoint = new MessageReceivingEndpoint(this._accessTokenUrl, this._accessTokenHttpDeliveryMethod),
					TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
					ProtocolVersion = ProtocolVersion.V10a,
				};

				var tokenManager = new InMemoryTokenManager();
				tokenManager.ConsumerKey = this._consumerKey;
				tokenManager.ConsumerSecret = this._consumerSecretKey;

				this._consumer = new DesktopConsumer(service, tokenManager);
				this._accessToken = string.Empty;

				if (service.ProtocolVersion == ProtocolVersion.V10a)
				{
					var authorizePopup = new Authorize(this._consumer, (DesktopConsumer c, out string requestToken) => c.RequestUserAuthorization(null, null, out requestToken));

					//todo: add some event
					//authorizePopup.Owner = this;
					//bool? result = authorizePopup.ShowDialog();
					//if( result.HasValue && result.Value )
					this._accessToken = authorizePopup.AccessToken;
					//else
					//	return;
				}
			}
			catch (ProtocolException ex)
			{
				//MessageBox.Show( this, ex.Message );
			}

			return items;
		}

		public IEnumerable<Item> InvokeGetCall(bool needAuthorise = false)
		{
			var items = new List<Item>();

			try
			{
				var serverResponse = string.Empty;

				//this._authorizationHeaderRequest =  HttpDeliveryMethods.PostRequest;
				this._authorizationHeaderRequest = HttpDeliveryMethods.GetRequest;

				if (needAuthorise)
					this._authorizationHeaderRequest |= HttpDeliveryMethods.AuthorizationHeaderRequest;

				var resourceEndpoint = new MessageReceivingEndpoint(this._resourceUrl, this._authorizationHeaderRequest);
				using (var resourceResponse = this._consumer.PrepareAuthorizedRequestAndSend(resourceEndpoint, this._accessToken))
					serverResponse = resourceResponse.GetResponseReader().ReadToEnd();

			}
			catch (ProtocolException ex)
			{
				//MessageBox.Show( this, ex.Message );
			}

			return items;
		}
	}

	public partial class Authorize
	{
		private DesktopConsumer consumer;
		private string requestToken;
		private string verificationKey;

		internal Authorize( DesktopConsumer consumer, FetchUri fetchUriCallback )
		{
			this.consumer = consumer;
			Task.Factory.StartNew( () =>
			{
				var browserAuthorizationLocation = fetchUriCallback( this.consumer, out this.requestToken );
				Process.Start( browserAuthorizationLocation.AbsoluteUri );
			} );

			//this.consumer = consumer;
			//ThreadPool.QueueUserWorkItem(delegate(object state) {
			//	Uri browserAuthorizationLocation = fetchUriCallback(this.consumer, out this.requestToken);
			//	System.Diagnostics.Process.Start(browserAuthorizationLocation.AbsoluteUri);
			//});
		}

		internal delegate Uri FetchUri( DesktopConsumer consumer, out string requestToken );

		internal string AccessToken { get; set; }

		private void finish( string verificationKey )
		{
			this.verificationKey = verificationKey;
			var grantedAccess = this.consumer.ProcessUserAuthorization( this.requestToken, this.verificationKey );
			this.AccessToken = grantedAccess.AccessToken;
		}

		//private void finishButton_Click(object sender, RoutedEventArgs e) {
		//var grantedAccess = this.consumer.ProcessUserAuthorization(this.requestToken, this.verifierBox.Text);
		//this.AccessToken = grantedAccess.AccessToken;
		//DialogResult = true;
		//Close();
		//}

		//private void cancelButton_Click(object sender, RoutedEventArgs e) {
		//DialogResult = false;
		//Close();
		//}
	}
}