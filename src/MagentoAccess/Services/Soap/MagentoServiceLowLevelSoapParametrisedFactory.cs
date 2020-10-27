using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Misc;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;
using MagentoAccess.Services.Soap._1_9_2_1_ce;

namespace MagentoAccess.Services.Soap
{
	internal class MagentoServiceLowLevelSoapFactory
	{
		private readonly Dictionary< string, IMagentoServiceLowLevelSoap > _factories;
		private readonly MagentoAuthenticatedUserCredentials _magentoAuthenticatedUserCredentials;
		private readonly MagentoConfig _config;
		private readonly MagentoTimeouts _operationsTimeouts;

		public MagentoServiceLowLevelSoapFactory( MagentoAuthenticatedUserCredentials magentoAuthenticatedUserCredentials, Dictionary< string, IMagentoServiceLowLevelSoap > factories, MagentoConfig config, MagentoTimeouts operationsTimeouts )
		{
			this._magentoAuthenticatedUserCredentials = magentoAuthenticatedUserCredentials;
			this._factories = factories;
			this._config = config.DefaultIfNull();
			this._operationsTimeouts = operationsTimeouts;
		}

		public IEnumerable< KeyValuePair< string, IMagentoServiceLowLevelSoap > > GetAll()
		{
			return this._factories.ToList();
		}

		public IMagentoServiceLowLevelSoap GetMagentoServiceLowLevelSoap( string magentoVersion, bool tryToSelectSuitable, bool returnNullIfNoSuitable )
		{
			Func< Dictionary< string, IMagentoServiceLowLevelSoap >, string, IMagentoServiceLowLevelSoap > getMagentoLowLevelServiceAndConfigureIt = ( facts, ver ) =>
			{
				var magentoServiceLowLevelSoap = facts[ ver ];
				magentoServiceLowLevelSoap.StoreVersion = ver;
				return magentoServiceLowLevelSoap;
			};

			var factories = this._factories.OrderBy( x => x.Key ).ToDictionary( x => x.Key, y => y.Value );
			factories.Add( "1.9.3.x", new MagentoServiceLowLevelSoap_v_1_9_2_1_ce_Factory().CreateMagentoLowLevelService( this._magentoAuthenticatedUserCredentials, this._config, this._operationsTimeouts ) );
			
			Version storeVersion;
			
			if ( Version.TryParse( magentoVersion, out storeVersion ) )
			{
				// use Rest API for version higher than 2.1.0 or default not specified version - 1.0
				if ( ( storeVersion.Major == 2 && storeVersion.Minor > 1 ) || ( storeVersion.Major == 1 && storeVersion.Minor == 0 ) )
				{
					var restService = factories.FirstOrDefault( s => s.Key.Equals( MagentoVersions.MR_2_0_0_0 ) );
					factories.Add( storeVersion.ToString(), restService.Value );
				}
			}

			if( tryToSelectSuitable && !factories.ContainsKey( magentoVersion ) )
			{
				// try to use similar version
				for( var j = 0; j < magentoVersion.Length; j++ )
				{
					var versions = factories.Keys.Where( x => x.Substring( 0, x.Length - j ) == magentoVersion.Substring( 0, magentoVersion.Length - j ) );
					var versionList = versions as IList< string > ?? versions.ToList();
					if( versionList.Any() )
					{
						factories.Add( magentoVersion, factories[ versionList.First() ] );
						return getMagentoLowLevelServiceAndConfigureIt( factories, magentoVersion );
					}
				}

				// there is no suitable
				if( returnNullIfNoSuitable )
					return null;

				// try to use 1.7- 1.9 low level service if can't detect version
				this._factories.Add( magentoVersion, new MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory().CreateMagentoLowLevelService( this._magentoAuthenticatedUserCredentials, this._config, this._operationsTimeouts ) );
				factories.Add( magentoVersion, new MagentoServiceLowLevelSoap_v_1_7_to_1_9_0_1_CE_Factory().CreateMagentoLowLevelService( this._magentoAuthenticatedUserCredentials, this._config, this._operationsTimeouts ) );
			}
			return getMagentoLowLevelServiceAndConfigureIt( factories, magentoVersion );
		}

		public string GetSubVersion( int deep, string magentoVer )
		{
			return magentoVer.Split( '.' )[ deep ];
		}
	}
}