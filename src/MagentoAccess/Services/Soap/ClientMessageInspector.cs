using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using MagentoAccess.Misc;

namespace MagentoAccess.Services.Soap
{
	internal class ClientMessageInspector : IClientMessageInspector
	{
		public bool LogRawMessages { get; set; } = false;

		public object BeforeSendRequest( ref Message request, IClientChannel channel )
		{
			//trace
			if( this.LogRawMessages )
			{
				var buffer = request.CreateBufferedCopy(int.MaxValue);
				request = buffer.CreateMessage();
				var originalMessage = buffer.CreateMessage();
				var messageSerialized = originalMessage.ToString();
				MagentoLogger.LogTraceRequestMessage(messageSerialized);
			}

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

			if ( !httpRequestMessage.Headers.AllKeys.Contains( "User-Agent" ) )
			{
				httpRequestMessage.Headers.Add( "User-Agent", MagentoService.UserAgentHeader );
			}

			return null;
		}

		public void AfterReceiveReply( ref Message reply, object correlationState )
		{
			//trace
			if( this.LogRawMessages )
			{
				var buffer = reply.CreateBufferedCopy( int.MaxValue );
				reply = buffer.CreateMessage();
				var originalMessage = buffer.CreateMessage();
				var messageSerialized = originalMessage.ToString();
				var property = originalMessage.Properties[ HttpResponseMessageProperty.Name.ToString() ] as HttpResponseMessageProperty;
				if( property != null )
					messageSerialized = "HttpStatusCode: " + property.StatusCode.ToString() + ", message:" + messageSerialized;
				MagentoLogger.LogTraceResponseMessage( messageSerialized );
			}
			
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