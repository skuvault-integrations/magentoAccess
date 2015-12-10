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
			_store = store;
			_baseMagentoUrl = baseMagentoUrl;
			_apiKey = apiKey;
			_apiUser = apiUser;
			_factories = factories;
		}

		public IMagentoServiceLowLevelSoap GetMagentoServiceLowLevelSoap( string pingSoapInfo, bool returnDefaultInsteadOfexception )
		{
			if( returnDefaultInsteadOfexception && !_factories.ContainsKey( pingSoapInfo ) )
				_factories.Add( pingSoapInfo, new MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE( _apiUser, _apiKey, _baseMagentoUrl, _store ) );

			return _factories[ pingSoapInfo ];
		}

		public string GetSubVersion( int deep, string magentoVer )
		{

			return magentoVer.Split('.')[deep];
		}
	}
}