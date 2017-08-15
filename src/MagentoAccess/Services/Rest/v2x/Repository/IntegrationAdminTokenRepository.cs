using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Newtonsoft.Json;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public class IntegrationAdminTokenRepository : IIntegrationAdminTokenRepository
	{
		private MagentoUrl Url { get; }

		public IntegrationAdminTokenRepository( MagentoUrl url )
		{
			this.Url = url;
		}

		public async Task< AuthorizationToken > GetTokenAsync( MagentoLogin token, MagentoPass url )
		{
			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( async () =>
			{
				using( var v = await ( ( WebRequest )
					WebRequest.Create()
						.Method( MagentoWebRequestMethod.Post )
						.Url( this.Url )
						.Path( MagentoServicePath.CreateIntegrationServicePath() )
						.Body( JsonConvert.SerializeObject( new CredentialsModel() { username = token.Login, password = url.Password } ) ) )
					.RunAsync( Mark.CreateNew() ).ConfigureAwait( false ) )
				{
					return AuthorizationToken.Create( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
				}
			} ).ConfigureAwait( false );
		}

		private class CredentialsModel
		{
			public string username { get; set; }
			public string password { get; set; }
		}
	}
}