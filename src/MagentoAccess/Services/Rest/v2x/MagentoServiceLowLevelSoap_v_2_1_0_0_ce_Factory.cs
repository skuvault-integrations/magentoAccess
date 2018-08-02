using MagentoAccess.Models.Credentials;
using MagentoAccess.Services.Soap;
using MagentoAccess.Services.Soap._2_1_0_0_ce;

namespace MagentoAccess.Services.Rest.v2x
{
	internal class MagentoServiceLowLevelSoap_v_r_2_0_0_0_ce_Factory : IMagentoServiceLowLevelSoapFactory
	{
		public IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials, MagentoConfig config )
		{
			return new MagentoServiceLowLevel(
				credentials.SoapApiUser,
				credentials.SoapApiKey,
				credentials.BaseMagentoUrl,
				credentials.LogRawMessages
				);
		}
	}
}