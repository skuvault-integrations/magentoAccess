using System;
using System.IO;
using System.ServiceModel.Channels;

namespace MagentoAccess.Services.Soap
{
	public class CustomMessageEncoder : MessageEncoder
	{
		public MessageEncoder InnerMessageEncoder { get; set; }
		public String Greeting { get; private set; }

		public CustomMessageEncoder( MessageEncoder innerMessageEncoder, String greeting )
		{
			this.InnerMessageEncoder = innerMessageEncoder;
			this.Greeting = greeting;
		}

		public override string ContentType
		{
			get { return this.InnerMessageEncoder.ContentType; }
		}

		public override string MediaType
		{
			get { return this.InnerMessageEncoder.MediaType; }
		}

		public override bool IsContentTypeSupported( string contentType )
		{
			//if( contentType == "text/xml; charset=utf-8,text/xml; charset=UTF-8" )
			return true;

			//return base.IsContentTypeSupported( contentType );
		}

		public override MessageVersion MessageVersion
		{
			get { return this.InnerMessageEncoder.MessageVersion; }
		}

		public override T GetProperty< T >()
		{
			return this.InnerMessageEncoder.GetProperty< T >();
		}

		public override Message ReadMessage( ArraySegment< byte > buffer, BufferManager bufferManager, string contentType )
		{
			// crutch special for overstockj.
			if( contentType == "text/xml; charset=utf-8,text/xml; charset=UTF-8" )
				contentType = "text/xml; charset=utf-8";

			var message = this.InnerMessageEncoder.ReadMessage( buffer, bufferManager, contentType );
			return message;
		}

		public override Message ReadMessage( Stream stream, int maxSizeOfHeaders, string contentType )
		{
			var message = this.InnerMessageEncoder.ReadMessage( stream, maxSizeOfHeaders, contentType );
			return message;
		}

		public override ArraySegment< byte > WriteMessage(
			Message message, int maxMessageSize,
			BufferManager bufferManager, int messageOffset )
		{
			message.Properties[ "greeting" ] = this.Greeting;
			return this.InnerMessageEncoder.WriteMessage(
				message, maxMessageSize, bufferManager, messageOffset );
		}

		public override void WriteMessage( Message message, Stream stream )
		{
			message.Properties[ "greeting" ] = this.Greeting;
			this.InnerMessageEncoder.WriteMessage( message, stream );
		}
	}

	public class CustomMessageEncoderFactory : MessageEncoderFactory
	{
		public MessageEncoderFactory InnerMessageEncoderFactory { get; private set; }
		public String Greeting { get; private set; }

		public CustomMessageEncoderFactory( MessageEncoderFactory innerMessageEncoderFactory, String greeting )
		{
			this.InnerMessageEncoderFactory = innerMessageEncoderFactory;
			this.Greeting = greeting;
		}

		public override MessageEncoder Encoder
		{
			get
			{
				return new CustomMessageEncoder(
					this.InnerMessageEncoderFactory.Encoder, this.Greeting );
			}
		}

		public override MessageVersion MessageVersion
		{
			get { return this.InnerMessageEncoderFactory.MessageVersion; }
		}
	}

	public class CustomMessageEncodingBindingElement : MessageEncodingBindingElement
	{
		public TextMessageEncodingBindingElement TextEncodingElement { get; private set; }
		public String Greeting { get; private set; }

		private CustomMessageEncodingBindingElement()
		{
		}

		public CustomMessageEncodingBindingElement(
			TextMessageEncodingBindingElement textEncodingElement, String greeting )
		{
			this.TextEncodingElement = textEncodingElement;
			this.Greeting = greeting;
		}

		public override IChannelFactory< TChannel > BuildChannelFactory< TChannel >( BindingContext context )
		{
			context.BindingParameters.Add( this );
			return base.BuildChannelFactory< TChannel >( context );
		}

		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			return new CustomMessageEncoderFactory(
				this.TextEncodingElement.CreateMessageEncoderFactory(),
				this.Greeting );
		}

		public override MessageVersion MessageVersion
		{
			get { return this.TextEncodingElement.MessageVersion; }
			set { this.TextEncodingElement.MessageVersion = value; }
		}

		public override BindingElement Clone()
		{
			var textEncodingElement = new TextMessageEncodingBindingElement()
			{
				MessageVersion = this.TextEncodingElement.MessageVersion,
				WriteEncoding = this.TextEncodingElement.WriteEncoding
			};
			return new CustomMessageEncodingBindingElement(
				textEncodingElement, this.Greeting );
		}
	}
}

