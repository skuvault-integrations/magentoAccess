using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce.ChannelBehaviour
{
	internal class ClientMessageInspector : IClientMessageInspector
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


			using (XmlDictionaryReader reader = request.GetReaderAtBodyContents())
			{
				//TODO: correct solution
				string content = reader.ReadOuterXml();
				var s = content + "asD";
				//Other stuff here...                
			}


			//string data = request.GetBody<string>();
			//var reader = request.GetReaderAtBodyContents();
			//var newMessage = System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Default, "newAction", reader);
			//var xxx = reader.ReadContentAsString();

			return null;
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
		}
	}
}