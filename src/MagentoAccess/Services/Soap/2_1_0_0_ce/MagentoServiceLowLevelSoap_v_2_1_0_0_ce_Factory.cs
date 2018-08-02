using MagentoAccess.Models.Credentials;

namespace MagentoAccess.Services.Soap._2_1_0_0_ce
{
	internal class MagentoServiceLowLevelSoap_v_2_1_0_0_ce_Factory : IMagentoServiceLowLevelSoapFactory
	{
		public IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials, MagentoConfig config )
		{
			return new MagentoServiceLowLevelSoap_v_2_1_0_0_ce(
				credentials.SoapApiUser,
				credentials.SoapApiKey,
				credentials.BaseMagentoUrl,
				credentials.LogRawMessages,
				null,
				config
				);
		}
	}
}