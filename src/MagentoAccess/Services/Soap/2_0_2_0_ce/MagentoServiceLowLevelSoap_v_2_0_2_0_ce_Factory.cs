using MagentoAccess.Models.Credentials;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce
{
	internal class MagentoServiceLowLevelSoap_v_2_0_2_0_ce_Factory : IMagentoServiceLowLevelSoapFactory
	{
		public IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials )
		{
			return new MagentoServiceLowLevelSoap_v_2_0_2_0_ce(
				credentials.SoapApiUser,
				credentials.SoapApiKey,
				credentials.BaseMagentoUrl,
				credentials.LogRawMessages,
				null
				);
		}
	}
}