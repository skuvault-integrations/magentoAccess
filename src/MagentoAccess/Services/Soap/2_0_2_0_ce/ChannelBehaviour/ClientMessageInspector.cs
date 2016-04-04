using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce.ChannelBehaviour
{
	internal class ClientMessageInspector: IClientMessageInspector
	{
		public string AccessToken;

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

			// crutch for magento 2.0
			this.ReplaceInMessageBody( ref request, channel, "Magento-2-0-2-0-ce", "magento-2-0-2-0-ce-2" );

			return null;
		}

		private void ReplaceInMessageBody( ref Message request, IClientChannel channel, string magentoCe, string newValue )
		{
			using( XmlDictionaryReader reader = request.GetReaderAtBodyContents() )
			{
				string content = reader.ReadOuterXml();
				var strBuf2 = content.Replace( magentoCe, newValue );
				using( var writer2 = this.GenerateStreamFromString( strBuf2 ) )
				using( var writer = XmlDictionaryWriter.CreateBinaryWriter( writer2 ) )
				{
					//request.WriteBody(writer);

					var v = XmlReader.Create( this.GenerateStreamFromString( strBuf2 ) );
					Message newMessage = Message.CreateMessage( request.Version, null, v );
					newMessage.Properties.CopyProperties( request.Properties );
					if( request.Headers != null && request.Headers.Count > 0 )
						newMessage.Headers.CopyHeaderFrom( request, 0 );

					//var modifiedReply = buffer.CreateMessage(); // need to recreate the message here
					request = newMessage;
				}
				//Other stuff here...                
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

			this.ReplaceInMessageBody( ref reply, null, "magento-2-0-2-0-ce-2", "Magento-2-0-2-0-ce" );
		}
	}
}