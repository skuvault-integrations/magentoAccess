using MagentoAccess.Misc;

namespace MagentoAccess.Models.GetMagentoCoreInfo
{
	public class PingSoapInfo
	{
		public string Version { get; private set; }
		public string Edition { get; private set; }
		public bool SoapWorks { get; private set; }
		public string ServiceVersion { get; private set; }

		public PingSoapInfo( string magentoVersion, string magentoEdition, bool soapWorks, string serviceVersion )
		{
			this.Version = magentoVersion;
			this.Edition = magentoEdition;
			this.SoapWorks = soapWorks;
			this.ServiceVersion = serviceVersion;
		}

		public string ToJson()
		{
			return string.Format( "{{Version:{0},Edition:{1},SoapWorks:{2},ServiceVersion:{3}}}",
				string.IsNullOrWhiteSpace( this.Version ) ? PredefinedValues.NotAvailable : this.Version,
				string.IsNullOrWhiteSpace( this.Edition ) ? PredefinedValues.NotAvailable : this.Edition,
				this.SoapWorks,
				string.IsNullOrWhiteSpace( this.ServiceVersion ) ? PredefinedValues.NotAvailable : this.ServiceVersion );
		}
	}
}