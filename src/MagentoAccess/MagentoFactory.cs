using CuttingEdge.Conditions;
using MagentoAccess.Misc;
using MagentoAccess.Models.Credentials;

namespace MagentoAccess
{
	public class MagentoFactory: IMagentoFactory
	{
		public IMagentoService CreateService( MagentoAuthenticatedUserCredentials userAuthCredentials, MagentoConfig magentoConfig, MagentoTimeouts operationsTimeouts )
		{
			Condition.Requires( userAuthCredentials, "userAuthCredentials" ).IsNotNull();
			return new MagentoService( userAuthCredentials, magentoConfig, operationsTimeouts );
		}
	}
}