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
		public MagentoAuthException( string message, Exception exception )
			: base( message, exception )
		{
		}
	}
}