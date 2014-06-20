namespace MagentoAccess.Models.GetMagentoCoreInfo
{
	public class MagentoCoreInfo
	{
		public string Version { get; private set; }
		public string Edition { get; private set; }
		public bool SoapWorks { get; private set; }
		public bool RestWorks { get; private set; }

		public MagentoCoreInfo( string magentoVersion, string magentoEdition, bool soapWorks, bool restWorks )
		{
			this.Version = magentoVersion;
			this.Edition = magentoEdition;
			this.SoapWorks = soapWorks;
			this.RestWorks = restWorks;
		}
	}
}