using System;
using System.Net;
using System.Runtime.CompilerServices;
using Netco.Logging;

namespace MagentoAccess
{
	public class MagentoException: Exception
	{
		protected MagentoException( string message, Exception exception, Mark mark = null )
			: base( message, exception )
		{
			this.CallMark = mark ?? Mark.Blank();
		}

		public override string ToString()
		{
			var stdToString = base.ToString();
			return stdToString + $"\tmark: {this.CallMark}";
		}

		public Mark CallMark{ get; set; }
	}

	public class MagentoAuthException: MagentoException
	{
		public MagentoAuthException( string message, Exception exception )
			: base( message, exception )
		{
		}
	}

	public class MagentoRestAuthException: MagentoException
	{
		public MagentoRestAuthException( string message, Exception exception )
			: base( message, exception )
		{
		}
	}

	public class MagentoSoapException: MagentoException
	{
		public MagentoSoapException( string message, Exception exception )
			: base( message, exception )
		{
		}

		public MagentoSoapException( string message, Exception exception, Mark mark )
			: base( message, exception, mark )
		{
		}
	}

	public class MagentoRestException: MagentoException
	{
		public MagentoRestException( string message, Exception exception )
			: base( message, exception )
		{
		}
	}

	public class MagentoWebException: MagentoException
	{
		public HttpStatusCode? StatusCode { get; private set; }

		public MagentoWebException( string message, Exception exception, HttpStatusCode? statusCode )
			: base( message, exception )
		{
			this.StatusCode = statusCode;
		}

		public MagentoWebException( string message, Exception exception )
			: this( message, exception, null )
		{
		}

		internal bool IsNotFoundException()
		{
			if ( this.StatusCode == HttpStatusCode.NotFound )
				return true;

			return false;
		}
	}

	public class MagentoCommonException: MagentoException
	{
		public MagentoCommonException( string message, Exception exception, [ CallerMemberName ] string memberName = "", Mark mark = null )
			: base( $"{memberName}:{message}", exception, mark )
		{
		}
	}
}