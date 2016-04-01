using MagentoAccess.Models.Credentials;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;

namespace MagentoAccess.Services.Soap
{
	internal interface IMagentoServiceLowLevelSoapFactory
	{
		IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials );
	}
}