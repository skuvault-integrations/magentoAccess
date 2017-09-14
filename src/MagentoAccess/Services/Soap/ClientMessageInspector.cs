using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
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
				var buffer = request.CreateBufferedCopy( int.MaxValue );
				request = buffer.CreateMessage();
				var originalMessage = buffer.CreateMessage();
				var messageSerialized = originalMessage.ToString();
				MagentoLogger.LogTraceRequestMessage( messageSerialized );
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

			reply = this.TransformMessage( reply );
		}

		private Message TransformMessage( Message oldMessage )
		{
			try
			{
				var msgbuf = oldMessage.CreateBufferedCopy( int.MaxValue );
				oldMessage = msgbuf.CreateMessage();

				var ms = new MemoryStream();
				var xw = XmlWriter.Create( ms );
				msgbuf.CreateMessage().WriteMessage( xw );
				xw.Flush();
				ms.Position = 0;
				var message = new StreamReader( ms ).ReadToEnd();

				if( message.IndexOf( "catalogCategoryTreeResponseParam", StringComparison.Ordinal ) < 0 )
					return oldMessage;

				if( message.IndexOf( "<is_active></is_active>", StringComparison.Ordinal ) >= 0 )
					message = message.Replace( "<is_active></is_active>", "<is_active>0</is_active>" );
				else if( message.IndexOf( "<is_active/>", StringComparison.Ordinal ) >= 0 )
					message = message.Replace( "<is_active/>", "<is_active>0</is_active>" );
				else
					return oldMessage;

				var messageBytes = System.Text.Encoding.UTF8.GetBytes( message );
				ms.SetLength( 0 );
				ms.Write( messageBytes, 0, messageBytes.Length );
				ms.Position = 0;

				var reader = XmlReader.Create( ms );
				var newMessage = Message.CreateMessage( reader, int.MaxValue, oldMessage.Version );

				newMessage.Headers.CopyHeadersFrom( oldMessage );
				newMessage.Properties.CopyProperties( oldMessage.Properties );

				return newMessage;
			}
			catch( Exception ex )
			{
				MagentoLogger.LogTraceException( ex );
				return oldMessage;
			}
		}
	}
}