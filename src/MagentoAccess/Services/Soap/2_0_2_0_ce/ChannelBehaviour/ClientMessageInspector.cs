using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce.ChannelBehaviour
{
	internal class ClientMessageInspector : IClientMessageInspector
	{
		private const string StandartNamespaceStub = "hereshouldbeyourmagentostoreurl.com";
		public string AccessToken;
		private readonly object lockObject = new object();
		private string replacedUrl;

		public object BeforeSendRequest( ref Message request, IClientChannel channel )
		{
			//legacy behaviour
			HttpRequestMessageProperty httpRequestMessage;
			object httpRequestMessageObject;
			if( request.Properties.TryGetValue( HttpRequestMessageProperty.Name, out httpRequestMessageObject ) )
			{
				httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
				if( string.IsNullOrEmpty( httpRequestMessage.Headers[ "Accept-Encoding" ] ) )
					httpRequestMessage.Headers.Remove( "Accept-Encoding" );
			}
			else
			{
				httpRequestMessage = new HttpRequestMessageProperty();
				httpRequestMessage.Headers.Add( "Accept-Encoding", "" );
				request.Properties.Add( HttpRequestMessageProperty.Name, httpRequestMessage );
			}

			//Auth Mangeot 2.0
			if( httpRequestMessage.Headers.AllKeys.Contains( "Authorization" ) )
				httpRequestMessage.Headers[ "Authorization" ] = "Bearer " + this.AccessToken;
			else
				httpRequestMessage.Headers.Add( "Authorization", "Bearer " + this.AccessToken );

			//Crutch for magento 2.0
			var newValue = channel.RemoteAddress.Uri.ToString();
			var urlWithoutProtocolIndex1 = newValue.IndexOf( "//", StringComparison.Ordinal ) + 2;
			var urlWithoutProtocolIndex2 = newValue.IndexOf( "/soap/default?services=", StringComparison.Ordinal );
			var storeUrlWithoutProtocol = newValue.Substring( urlWithoutProtocolIndex1, urlWithoutProtocolIndex2 - urlWithoutProtocolIndex1 ).Trim();

			lock( this.lockObject )
			{
				if( string.IsNullOrWhiteSpace( this.replacedUrl ) )
					this.replacedUrl = storeUrlWithoutProtocol;
			}
			this.ReplaceInMessageBody( ref request, StandartNamespaceStub, storeUrlWithoutProtocol );

			return null;
		}

		private void ReplaceInMessageBody( ref Message request, string magentoCe, string newValue )
		{
			using( var reader = request.GetReaderAtBodyContents() )
			{
				var content = reader.ReadOuterXml();
				var contentWithChanges = content.Replace( magentoCe, newValue );
				var xmlReader = XmlReader.Create( this.GenerateStreamFromString( contentWithChanges ) );
				var newMessage = Message.CreateMessage( request.Version, null, xmlReader );
				newMessage.Properties.CopyProperties( request.Properties );
				if( request.Headers != null && request.Headers.Count > 0 )
					newMessage.Headers.CopyHeaderFrom( request, 0 );
				request = newMessage;
			}
		}

		public Stream GenerateStreamFromString( string s )
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter( stream );
			writer.Write( s );
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		public void AfterReceiveReply( ref Message reply, object correlationState )
		{
			var prop =
				reply.Properties[ HttpResponseMessageProperty.Name.ToString() ] as HttpResponseMessageProperty;

			if( prop != null )
			{
				// get the content type headers
				var contentType = prop.Headers[ "Content-Type" ];
			}

			this.ReplaceInMessageBody( ref reply, this.replacedUrl, StandartNamespaceStub );
		}
	}
}