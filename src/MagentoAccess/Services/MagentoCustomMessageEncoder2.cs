using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace MagentoAccess.Services
{
	public class MyMessageEncoder : MessageEncoder
	{
		public MessageEncoder InnerMessageEncoder { get; set; }
		public String Greeting { get; private set; }

		public MyMessageEncoder(MessageEncoder innerMessageEncoder, String greeting)
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
			if (contentType == "text/xml; charset=utf-8,text/xml; charset=UTF-8")
			{
				return true;
			}

			return base.IsContentTypeSupported(contentType);
		}

		public override MessageVersion MessageVersion
		{
			get { return this.InnerMessageEncoder.MessageVersion; }
		}

		public override T GetProperty<T>()
		{
			return this.InnerMessageEncoder.GetProperty<T>();
		}

		public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
		{
			// crutch special for overstockjewlry. Thank you overstockjewlry for great weekends!
			if( contentType == "text/xml; charset=utf-8,text/xml; charset=UTF-8" )
			{
				contentType = "text/xml; charset=utf-8";
			}

			Message message = this.InnerMessageEncoder.ReadMessage(buffer, bufferManager, contentType);
			//Console.WriteLine("greeting = " + message.Properties["greeting"].ToString());
			return message;
		}

		public override Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders, string contentType)
		{
			Message message = this.InnerMessageEncoder.ReadMessage(stream, maxSizeOfHeaders, contentType);
			Console.WriteLine("greeting = " + message.Properties["greeting"].ToString());
			return message;
		}

		public override ArraySegment<byte> WriteMessage(
			Message message, int maxMessageSize,
			BufferManager bufferManager, int messageOffset)
		{

			message.Properties["greeting"] = this.Greeting;
			return this.InnerMessageEncoder.WriteMessage(
				message, maxMessageSize, bufferManager, messageOffset);
		}

		public override void WriteMessage(Message message, System.IO.Stream stream)
		{
			message.Properties["greeting"] = this.Greeting;
			this.InnerMessageEncoder.WriteMessage(message, stream);
		}
	}

	public class MyMessageEncoderFactory : MessageEncoderFactory
	{
		public MessageEncoderFactory InnerMessageEncoderFactory { get; private set; }
		public String Greeting { get; private set; }

		public MyMessageEncoderFactory(MessageEncoderFactory innerMessageEncoderFactory, String greeting)
		{
			this.InnerMessageEncoderFactory = innerMessageEncoderFactory;
			this.Greeting = greeting;
		}

		public override MessageEncoder Encoder
		{
			get
			{
				return new MyMessageEncoder(
					this.InnerMessageEncoderFactory.Encoder, this.Greeting);
			}
		}

		public override MessageVersion MessageVersion
		{
			get { return this.InnerMessageEncoderFactory.MessageVersion; }
		}
	}

	public class MyTextEncodingElement : BindingElementExtensionElement
	{
		[ConfigurationProperty("greeting")]
		public String Greeting
		{
			get { return (String)this["greeting"]; }
			set { this["greeting"] = value; }
		}

		[ConfigurationProperty("textEncoding")]
		public TextMessageEncodingElement TextEncoding
		{
			get { return (TextMessageEncodingElement)this["textEncoding"]; }
			set { this["textEncoding"] = value; }
		}

		public override Type BindingElementType
		{
			get { return typeof(MyTextMessageEncodingBindingElement); }
		}

		protected override BindingElement CreateBindingElement()
		{
			var textBindingElement = new TextMessageEncodingBindingElement();
			if (this.TextEncoding != null)
			{
				this.TextEncoding.ApplyConfiguration(textBindingElement);
			}
			return new MyTextMessageEncodingBindingElement(textBindingElement, this.Greeting);
		}
	}

	public class MyTextMessageEncodingBindingElement : MessageEncodingBindingElement
	{
		public TextMessageEncodingBindingElement TextEncodingElement { get; private set; }
		public String Greeting { get; private set; }

		private MyTextMessageEncodingBindingElement()
		{

		}

		public MyTextMessageEncodingBindingElement(
			TextMessageEncodingBindingElement textEncodingElement, String greeting)
		{
			this.TextEncodingElement = textEncodingElement;
			this.Greeting = greeting;
		}

		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
		{
			context.BindingParameters.Add(this);
			return base.BuildChannelFactory<TChannel>(context);
		}

		public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
		{
			context.BindingParameters.Add(this);
			return base.BuildChannelListener<TChannel>(context);
		}

		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			return new MyMessageEncoderFactory(
				this.TextEncodingElement.CreateMessageEncoderFactory(),
				this.Greeting);
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return this.TextEncodingElement.MessageVersion;
			}
			set
			{
				this.TextEncodingElement.MessageVersion = value;
			}
		}

		public override BindingElement Clone()
		{
			var textEncodingElement = new TextMessageEncodingBindingElement()
			{
				MaxReadPoolSize = this.TextEncodingElement.MaxReadPoolSize,
				MaxWritePoolSize = this.TextEncodingElement.MaxWritePoolSize,
				MessageVersion = this.TextEncodingElement.MessageVersion,
				WriteEncoding = this.TextEncodingElement.WriteEncoding
			};
			return new MyTextMessageEncodingBindingElement(
				textEncodingElement, this.Greeting);
		}
	}
}