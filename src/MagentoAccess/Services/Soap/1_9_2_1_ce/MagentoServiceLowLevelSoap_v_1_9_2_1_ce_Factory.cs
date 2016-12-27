using MagentoAccess.Models.Credentials;
using MagentoAccess.Services.Soap._1_9_2_1_ce;

namespace MagentoAccess.Services.Soap._1_9_2_1_ce
{
	internal class MagentoServiceLowLevelSoap_v_1_9_2_1_ce_Factory : IMagentoServiceLowLevelSoapFactory
	{
		public IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials )
		{
			return new MagentoServiceLowLevelSoap_v_1_9_2_1_ce(
				credentials.SoapApiUser,
				credentials.SoapApiKey,
				credentials.BaseMagentoUrl,
				null,
				credentials.GetProductsThreadsLimit,
				credentials.LogRawMessages,
				credentials.SessionLifeTimeMs 
				);
		}
	}
}