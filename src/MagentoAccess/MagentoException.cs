using System;

namespace MagentoAccess
{
	public class MagentoException : Exception
	{
		protected MagentoException( string message, Exception exception )
			: base( message, exception )
		{
		}
	}

	public class MagentoAuthException : MagentoException
	{
		public MagentoAuthException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}

	public class MagentoRestAuthException : MagentoException
	{
		public MagentoRestAuthException( string message, Exception exception )
			: base( message, exception )
		{
		}
	}

	public class MagentoSoapException : MagentoException
	{
		public MagentoSoapException( string message, Exception exception )
			: base( message, exception )
		{
		}
	}

	public class MagentoRestException : MagentoException
	{
		public MagentoRestException( string message, Exception exception )
			: base( message, exception )
		{
		}
	}
}