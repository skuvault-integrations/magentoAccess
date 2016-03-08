using MagentoAccess.Models.Credentials;

namespace MagentoAccess
{
	public interface IMagentoFactory
	{
		IMagentoService CreateService( MagentoAuthenticatedUserCredentials userAuthCredentials, MagentoConfig magentoConfig );

		IMagentoService CreateService( MagentoNonAuthenticatedUserCredentials userNonAuthCredentials );
	}
}