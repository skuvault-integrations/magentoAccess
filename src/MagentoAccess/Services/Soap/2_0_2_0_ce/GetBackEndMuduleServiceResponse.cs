using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Magento2backendModuleServiceV1_v_2_0_2_0_CE;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce
{
	internal class GetBackEndMuduleServiceResponse
	{
		public GetBackEndMuduleServiceResponse( backendModuleServiceV1GetModulesResponse1 backendModuleServiceV1GetModulesResponse1 )
		{
			this.Modules = backendModuleServiceV1GetModulesResponse1.backendModuleServiceV1GetModulesResponse.result.ToList();
		}

		public List< string > Modules{ get; private set; }
	}
}