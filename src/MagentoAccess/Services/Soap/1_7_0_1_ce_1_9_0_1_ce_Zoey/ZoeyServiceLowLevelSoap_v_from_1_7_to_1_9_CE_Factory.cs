using MagentoAccess.Models.Credentials;
using MagentoAccess.Misc;

namespace MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce_Zoey
{
	internal class ZoeyServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory : IMagentoServiceLowLevelSoapFactory
	{
		public IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials, MagentoConfig config, MagentoTimeouts operationsTimeouts )
		{
			return new ZoeyServiceLowLevelSoap_v_from_1_7_to_1_9_CE(
				credentials.SoapApiUser,
				credentials.SoapApiKey,
				credentials.BaseMagentoUrl,
				null,
				credentials.SessionLifeTimeMs,
				credentials.GetProductsThreadsLimit,
				credentials.LogRawMessages,
				config
			);
		}
	}
}