﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Services.Rest.v2x.WebRequester;
using Netco.Logging;
using Newtonsoft.Json;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public class IntegrationAdminTokenRepository : BaseRepository, IIntegrationAdminTokenRepository
	{
		private MagentoUrl Url { get; }
		private MagentoTimeouts OperationsTimeouts { get; }

		public IntegrationAdminTokenRepository( MagentoUrl url, MagentoTimeouts operationsTimeouts, string relativeUrl = "" )
		{
			this.Url = url;
			this.OperationsTimeouts = operationsTimeouts;
			this.RelativeUrl = relativeUrl;
		}

		public async Task< AuthorizationToken > GetTokenAsync( MagentoLogin token, MagentoPass password, CancellationToken cancellationToken )
		{
			return await ActionPolicies.RepeatOnChannelProblemAsync.Get( () =>
			{				
				return TrackNetworkActivityTime( async () =>
				{
					using( var v = await ( ( WebRequest )
						WebRequest.Create()
							.Method( MagentoWebRequestMethod.Post )
							.Timeout( OperationsTimeouts[ MagentoOperationEnum.GetToken ] )
							.Url( this.Url )
							.Path( MagentoServicePath.CreateIntegrationServicePath() )
							.Body( JsonConvert.SerializeObject( new CredentialsModel() { username = token.Login, password = password.Password } ) ) )
						.RunAsync( cancellationToken, Mark.CreateNew(), RelativeUrl ).ConfigureAwait( false ) )
					{
						return AuthorizationToken.Create( new StreamReader( v, Encoding.UTF8 ).ReadToEnd() );
					}
				} );
			} ).ConfigureAwait( false );
		}

		private class CredentialsModel
		{
			public string username { get; set; }
			public string password { get; set; }
		}
	}
}