namespace MagentoAccess.Models.GetMagentoCoreInfo
{
	public class PingSoapInfo
	{
		public string Version { get; private set; }
		public string Edition { get; private set; }
		public bool SoapWorks { get; private set; }

		public PingSoapInfo( string magentoVersion, string magentoEdition, bool soapWorks )
		{
			this.Version = magentoVersion;
			this.Edition = magentoEdition;
			this.SoapWorks = soapWorks;
		}
	}
}