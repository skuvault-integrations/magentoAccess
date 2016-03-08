using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce;

namespace MagentoAccess.Services.Soap
{
	internal class MagentoServiceLowLevelSoapFactory
	{
		private readonly Dictionary< string, IMagentoServiceLowLevelSoap > _factories;
		private readonly string _apiUser;
		private readonly string _apiKey;
		private readonly string _baseMagentoUrl;
		private readonly string _store;

		public MagentoServiceLowLevelSoapFactory( string store, string baseMagentoUrl, string apiKey, string apiUser, Dictionary< string, IMagentoServiceLowLevelSoap > factories )
		{
			this._store = store;
			this._baseMagentoUrl = baseMagentoUrl;
			this._apiKey = apiKey;
			this._apiUser = apiUser;
			this._factories = factories;
		}

		public IMagentoServiceLowLevelSoap GetMagentoServiceLowLevelSoap( string magentoVersion, bool tryToSelectSuitable, bool returnNullIfNoSuitable )
		{
			var factories = this._factories.OrderBy( x => x.Key ).ToDictionary( x => x.Key, y => y.Value );
			if( tryToSelectSuitable && !factories.ContainsKey( magentoVersion ) )
			{
				for( var j = 0; j < magentoVersion.Length; j++ )
				{
					var versions = factories.Keys.Where( x => x.Substring( 0, x.Length - j ) == magentoVersion.Substring( 0, magentoVersion.Length - j ) );
					var versionList = versions as IList< string > ?? versions.ToList();
					if( versionList.Any() )
					{
						factories.Add( magentoVersion, factories[ versionList.First() ] );
						return factories[ magentoVersion ];
					}
				}
				if( returnNullIfNoSuitable )
					return null;

				this._factories.Add( magentoVersion, new MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE( _apiUser, _apiKey, _baseMagentoUrl, _store ) );
			}

			return factories[ magentoVersion ];
		}

		public string GetSubVersion( int deep, string magentoVer )
		{
			return magentoVer.Split( '.' )[ deep ];
		}
	}
}