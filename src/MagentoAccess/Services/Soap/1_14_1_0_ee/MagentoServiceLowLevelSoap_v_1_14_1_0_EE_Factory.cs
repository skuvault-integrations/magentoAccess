using MagentoAccess.Models.Credentials;

namespace MagentoAccess.Services.Soap._1_14_1_0_ee
{
	internal class MagentoServiceLowLevelSoap_v_1_14_1_0_EE_Factory: IMagentoServiceLowLevelSoapFactory
	{
		public IMagentoServiceLowLevelSoap CreateMagentoLowLevelService( MagentoAuthenticatedUserCredentials credentials )
		{
			return new MagentoServiceLowLevelSoap_v_1_14_1_0_EE(
				credentials.SoapApiUser,
				credentials.SoapApiKey,
				credentials.BaseMagentoUrl,
				null,
				credentials.GetProductsThreadsLimit,
				credentials.GetProductsThreadsLimit);
		}
	}
}