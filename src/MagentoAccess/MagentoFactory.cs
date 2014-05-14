using CuttingEdge.Conditions;
using MagentoAccess.Models.Credentials;

namespace MagentoAccess
{
	public class MagentoFactory : IMagentoFactory
	{
		private readonly MagentoAuthenticatedUserCredentials _userCredentials;
		private readonly MagentoNonAuthenticatedUserCredentials _userNonAuthCredentials;

		public MagentoFactory( MagentoAuthenticatedUserCredentials userCredentials )
		{
			Condition.Requires( userCredentials, "userCredentials" ).IsNotNull();
			this._userCredentials = userCredentials;
		}

		public MagentoFactory( MagentoNonAuthenticatedUserCredentials userNonAuthCredentials )
		{
			Condition.Requires( userNonAuthCredentials, "userNonAuthCredentials" ).IsNotNull();
			this._userNonAuthCredentials = userNonAuthCredentials;
		}

		public IMagentoService CreateService()
		{
			return ( this._userCredentials != null ) ?
				new MagentoService( this._userCredentials ) :
				new MagentoService( this._userNonAuthCredentials );
		}
	}
}