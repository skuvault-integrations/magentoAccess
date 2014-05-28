using CuttingEdge.Conditions;
using MagentoAccess.Models.Services.Credentials;

namespace MagentoAccess
{
	public class MagentoFactory : IMagentoFactory
	{
		public IMagentoService CreateService( MagentoAuthenticatedUserCredentials userAuthCredentials )
		{
			Condition.Requires( userAuthCredentials, "userAuthCredentials" ).IsNotNull();
			return new MagentoService( userAuthCredentials );
		}

		public IMagentoService CreateService( MagentoNonAuthenticatedUserCredentials userNonAuthCredentials )
		{
			Condition.Requires( userNonAuthCredentials, "userNonAuthCredentials" ).IsNotNull();
			return new MagentoService( userNonAuthCredentials );
		}
	}
}