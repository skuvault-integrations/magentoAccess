using MagentoAccess.Models.Credentials;
using MagentoAccess.Misc;

namespace MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce
{
	internal class MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory: IMagentoServiceLowLevelSoapFactory
	{
		public IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials, MagentoConfig config, MagentoTimeouts operationsTimeouts )
		{
			return new MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE(
				credentials.SoapApiUser,
				credentials.SoapApiKey,
				credentials.BaseMagentoUrl,
				credentials.RelativeUrl,
				null,
				credentials.GetProductsThreadsLimit,
				credentials.LogRawMessages,
				credentials.SessionLifeTimeMs,
				config );
		}
	}
}