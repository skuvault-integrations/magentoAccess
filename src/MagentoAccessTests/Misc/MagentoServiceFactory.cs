using MagentoAccess.Services.Soap;
using NUnit.Framework;

namespace MagentoAccessTests.Misc
{
	[ TestFixture ]
	public class MagentoServiceFactory
	{
		[ TestCase( 0, "1.2.3.4", Result = "1" ) ]
		[ TestCase( 3, "1.2.3.4", Result = "4" ) ]
		[ TestCase( 0, "11.2.3.4", Result = "11" ) ]
		[ TestCase( 3, "11.2.3.44", Result = "44" ) ]
		[ Test ]
		public string Test( int deep, string magentoVer )
		{
			//------------ Arrange
			var magentoServiceLowLevelSoapFactory = new MagentoServiceLowLevelSoapFactory( null, null, null, null, null );

			//------------ Act
			var version = magentoServiceLowLevelSoapFactory.GetSubVersion( deep, magentoVer );
			//------------ Assert
			return version;
		}
	}
}