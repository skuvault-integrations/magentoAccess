using MagentoAccess.Models.Credentials;
using MagentoAccess.Services.Soap;
using MagentoAccess.Misc;

namespace MagentoAccess.Services.Rest.v2x
{
	internal sealed class MagentoServiceLowLevelSoap_v_r_2_0_0_0_ce_Factory : IMagentoServiceLowLevelSoapFactory
	{
		public IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials, MagentoConfig config, MagentoTimeouts operationsTimeouts )
		{
			return new MagentoServiceLowLevel(
				credentials.SoapApiUser,
				credentials.SoapApiKey,
				credentials.BaseMagentoUrl,
				credentials.RelativeUrl,
				operationsTimeouts,
				credentials.LogRawMessages );
		}
	}
}