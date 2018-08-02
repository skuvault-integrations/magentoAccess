using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace MagentoAccess.Services.Soap
{
	internal abstract class BaseMagentoServiceSoapClientFactory< T, T2 > where T : ClientBase< T2 > where T2 : class
	{
		protected readonly bool ForceRecreateClient = false;

		private T _client;

		protected abstract T CreateClient();

		protected readonly string _baseMagentoUrl;
		protected readonly bool _logRawMessages;
		protected readonly MagentoConfig _config;

		internal BaseMagentoServiceSoapClientFactory( string baseMagentoUrl, bool logRawMessages, MagentoConfig config )
		{
			this._baseMagentoUrl = baseMagentoUrl;
			this._logRawMessages = logRawMessages;
			this._config = config;
		}

		public T GetClient()
		{
			if( this.ForceRecreateClient )
				return this.CreateClient();

			if( this._client == null )
			{
				this._client = this.CreateClient();
			}

			return RefreshClient( this._client );
		}

		public T RefreshClient( T client )
		{
			if( client.State != CommunicationState.Opened && client.State != CommunicationState.Created && client.State != CommunicationState.Opening )
			{
				return this.CreateClient();
			}

			return client;
		}

		protected static CustomBinding CustomBinding( string baseMagentoUrl, MessageVersion messageVersion, bool decompressionEnabled )
		{
			var textMessageEncodingBindingElement = new TextMessageEncodingBindingElement
			{
				MessageVersion = messageVersion,
				WriteEncoding = new UTF8Encoding()
			};

			BindingElement httpTransportBindingElement;
			if( baseMagentoUrl.StartsWith( "https" ) )
			{
				httpTransportBindingElement = new HttpsTransportBindingElement
				{
					DecompressionEnabled = decompressionEnabled,
					MaxReceivedMessageSize = 999999999,
					MaxBufferSize = 999999999,
					MaxBufferPoolSize = 999999999,
					KeepAliveEnabled = true,
					AllowCookies = false,
				};
			}
			else
			{
				httpTransportBindingElement = new HttpTransportBindingElement
				{
					DecompressionEnabled = decompressionEnabled,
					MaxReceivedMessageSize = 999999999,
					MaxBufferSize = 999999999,
					MaxBufferPoolSize = 999999999,
					KeepAliveEnabled = true,
					AllowCookies = false,
				};
			}

			var myTextMessageEncodingBindingElement = new CustomMessageEncodingBindingElement( textMessageEncodingBindingElement, "qwe" )
			{
				MessageVersion = messageVersion,
			};

			ICollection< BindingElement > bindingElements = new List< BindingElement >();
			var httpBindingElement = httpTransportBindingElement;
			var textBindingElement = myTextMessageEncodingBindingElement;
			bindingElements.Add( textBindingElement );
			bindingElements.Add( httpBindingElement );

			var customBinding = new CustomBinding( bindingElements ) { ReceiveTimeout = new TimeSpan( 0, 2, 30, 0 ), SendTimeout = new TimeSpan( 0, 2, 30, 0 ), OpenTimeout = new TimeSpan( 0, 2, 30, 0 ), CloseTimeout = new TimeSpan( 0, 2, 30, 0 ), Name = "CustomHttpBinding" };
			return customBinding;
		}
	}
}
