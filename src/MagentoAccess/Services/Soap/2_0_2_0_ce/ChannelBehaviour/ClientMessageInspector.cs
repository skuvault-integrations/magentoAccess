using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
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


			//using (XmlDictionaryReader reader = request.GetReaderAtBodyContents())
			//{
			//	//TODO: correct solution
			//	string content = reader.ReadOuterXml();
			//	//content = content.Replace("agento-2-0-2-0", "xxx123");
			//	using (var writer2 = GenerateStreamFromString(content))
			//	using (var writer = XmlDictionaryWriter.CreateBinaryWriter(writer2))
			//	{
			//		request.State
			//		request.WriteBody(writer);

			//		//my write message
			//	}
			//	//Other stuff here...                
			//}

			Modify3(ref request,channel, "Magento-2-0-2-0-ce", "magento-2-0-2-0-ce-2" );

			return null;
		}

		private void Modify2(ref Message request, IClientChannel channel)
		{

        MessageBuffer msgbuf = request.CreateBufferedCopy(int.MaxValue);
        Message tmpMessage = msgbuf.CreateMessage();
        XmlDictionaryReader xdr = tmpMessage.GetReaderAtBodyContents();
        MemoryStream ms = new MemoryStream();
       // _compiledTransform.Transform(xdr,null,ms);

        ms.Position = 0;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(ms);

        MemoryStream newStream = new MemoryStream();
        xmlDoc.Save(newStream);
        newStream.Position = 0;

        //To debug contents of the stream
        StreamReader sr = new StreamReader(newStream);
        var temp = sr.ReadToEnd();
        //At this point the XSLT tranforms has resulted in the fragment we want so all good!


        XmlReaderSettings settings = new XmlReaderSettings();
        settings.ConformanceLevel = ConformanceLevel.Fragment;
        newStream.Position = 0;
        XmlReader reader = XmlReader.Create(newStream,settings);
        reader.MoveToContent();

        //Reader seems to have lost the correct fragment!!! At least returned message does not contain correct fragment.
        Message newMessage = Message.CreateMessage(request.Version, null, reader);
        newMessage.Properties.CopyProperties(request.Properties);
        request = newMessage;

        //return request;

		}

		private void Modify3(ref Message request, IClientChannel channel, string magentoCe, string newValue )
		{

			using (XmlDictionaryReader reader = request.GetReaderAtBodyContents())
			{
				string content = reader.ReadOuterXml();
				var strBuf2 = content.Replace(magentoCe, newValue);
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

		private void Modify1 (ref Message request, IClientChannel channel)
		{

			var buffer = request.CreateBufferedCopy(Int32.MaxValue);
			var toPrint = buffer.CreateMessage();
			MemoryStream ms = new MemoryStream();
			XmlWriterSettings ws = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				OmitXmlDeclaration = true, 
				Encoding = new UTF8Encoding(false)
			};
			XmlWriter w = XmlWriter.Create(ms, ws);
			toPrint.WriteMessage(w);
			w.Flush();
			var strBuf = Encoding.UTF8.GetString(ms.ToArray());
			var strBuf2 = strBuf.Replace("Magento-2-0-2-0-ce", "magento-2-0-2-0-ce-2");

			var v = System.Xml.XmlReader.Create(GenerateStreamFromString(strBuf2));
			Message newMessage = Message.CreateMessage(request.Version, null, v);
			newMessage.Properties.CopyProperties(request.Properties);
			newMessage.Headers.CopyHeaderFrom(request,0);

			//var modifiedReply = buffer.CreateMessage(); // need to recreate the message here
			request = newMessage;

		}

		public Stream GenerateStreamFromString(string s)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(s);
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

			this.Modify3( ref reply, null,  "magento-2-0-2-0-ce-2","Magento-2-0-2-0-ce" );
		}
	}
}