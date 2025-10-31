using System;
using MagentoAccess.Misc;
using MagentoAccess.Models.Credentials;

namespace MagentoAccess
{
	public class MagentoFactory: IMagentoFactory
	{
		public IMagentoService CreateService( MagentoAuthenticatedUserCredentials userAuthCredentials, MagentoConfig magentoConfig, MagentoTimeouts operationsTimeouts )
		{
			return userAuthCredentials == null ? throw new ArgumentNullException( nameof(userAuthCredentials), "userAuthCredentials must not be null" ) : new MagentoService( userAuthCredentials, magentoConfig, operationsTimeouts );
		}
	}
}