using System.Threading.Tasks;
using MagentoAccess.Services.Rest.v2x.WebRequester;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public interface IIntegrationAdminTokenRepository
	{
		Task< AuthorizationToken > GetTokenAsync( MagentoLogin token, MagentoPass url );
	}
}