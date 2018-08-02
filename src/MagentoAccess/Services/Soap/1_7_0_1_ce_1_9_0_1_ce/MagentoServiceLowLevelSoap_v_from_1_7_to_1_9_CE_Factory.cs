using MagentoAccess.Models.Credentials;

namespace MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce
{
	internal class MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory: IMagentoServiceLowLevelSoapFactory
	{
		public IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials, MagentoConfig config )
		{
			return new MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE(
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