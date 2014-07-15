using MagentoAccess.Misc;

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

		public string ToJson()
		{
			return string.Format( "{{Version:{0},Edition:{1},SoapWorks:{2}}}",
				string.IsNullOrWhiteSpace( this.Version ) ? PredefinedValues.NotAvailable : this.Version,
				string.IsNullOrWhiteSpace( this.Edition ) ? PredefinedValues.NotAvailable : this.Edition,
				this.SoapWorks );
		}
	}
}