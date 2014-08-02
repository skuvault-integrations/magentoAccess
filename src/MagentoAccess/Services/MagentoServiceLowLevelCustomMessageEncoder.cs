using System;
using System.IO;
using System.Net.Mime;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace MagentoAccess.Services
{
	public class CustomMessageFormatter : IClientMessageFormatter
	{
		public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
		{
			return null;
		}

		public object DeserializeReply(Message message, object[] parameters)
		{
			return null;
		}
	}

	public class CustomTextMessageEncoderFactory : MessageEncoderFactory
	{
		private MessageEncoder encoder;
		private MessageVersion version;
		private string mediaType;
		private string charSet;

		internal CustomTextMessageEncoderFactory(string mediaType, string charSet,
			MessageVersion version)
		{
			this.version = version;
			this.mediaType = mediaType;
			this.charSet = charSet;
			this.encoder = new CustomTextMessageEncoder(this);
		}

		public override MessageEncoder Encoder
		{
			get
			{
				return this.encoder;
			}
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return this.version;
			}
		}

		internal string MediaType
		{
			get
			{
				return this.mediaType;
			}
		}

		internal string CharSet
		{
			get
			{
				return this.charSet;
			}
		}
	}

	public class CustomTextMessageEncoder : MessageEncoder
	{
		private CustomTextMessageEncoderFactory factory;
		private XmlWriterSettings writerSettings;
		private string contentType;

		public override bool IsContentTypeSupported( string contentType )
		{
			return true;
		}

		public CustomTextMessageEncoder(CustomTextMessageEncoderFactory factory)
		{
			this.factory = factory;

			this.writerSettings = new XmlWriterSettings();
			this.writerSettings.Encoding = Encoding.GetEncoding(factory.CharSet);
			this.contentType = string.Format("{0}; charset={1}",
				this.factory.MediaType, this.writerSettings.Encoding.HeaderName);
		}

		public override string ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public override string MediaType
		{
			get
			{
				return factory.MediaType;
			}
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return this.factory.MessageVersion;
			}
		}

		public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
		{
			byte[] msgContents = new byte[buffer.Count];
			Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
			bufferManager.ReturnBuffer(buffer.Array);

			MemoryStream stream = new MemoryStream(msgContents);
			return ReadMessage(stream, int.MaxValue);
		}

		public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
		{
			XmlReader reader = XmlReader.Create(stream);
			return Message.CreateMessage(reader, maxSizeOfHeaders, this.MessageVersion);
		}

		public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
		{
			MemoryStream stream = new MemoryStream();
			XmlWriter writer = XmlWriter.Create(stream, this.writerSettings);
			message.WriteMessage(writer);
			writer.Close();

			byte[] messageBytes = stream.GetBuffer();
			int messageLength = (int)stream.Position;
			stream.Close();

			int totalLength = messageLength + messageOffset;
			byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
			Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

			ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
			return byteArray;
		}

		public override void WriteMessage(Message message, Stream stream)
		{
			XmlWriter writer = XmlWriter.Create(stream, this.writerSettings);
			message.WriteMessage(writer);
			writer.Close();
		}
	}

	public class CustomTextMessageBindingElement : MessageEncodingBindingElement
	{
		public override BindingElement Clone()
		{
			return new CustomTextMessageBindingElement { MessageVersion = this.MessageVersion };
		}

		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			return new CustomTextMessageEncoderFactory(MediaTypeNames.Text.Xml, "utf-8", this.MessageVersion);
		}

		public override MessageVersion MessageVersion { get; set; }
	}
}