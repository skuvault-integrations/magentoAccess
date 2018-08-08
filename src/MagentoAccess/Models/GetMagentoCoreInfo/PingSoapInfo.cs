using MagentoAccess.Misc;

namespace MagentoAccess.Models.GetMagentoCoreInfo
{
	public class PingSoapInfo
	{
		public string StoreVersion { get; private set; }
		public string StoreEdition { get; private set; }
		public bool SoapWorks { get; private set; }
		public string ServiceUsedVersion { get; private set; }

		public PingSoapInfo( string magentoStoreVersion, string magentoStoreEdition, bool soapWorks, string serviceUsedVersion )
		{
			this.StoreVersion = magentoStoreVersion;
			this.StoreEdition = magentoStoreEdition;
			this.SoapWorks = soapWorks;
			this.ServiceUsedVersion = serviceUsedVersion;
		}

		public string ToJson()
		{
			return string.Format( "{{StoreVersion:{0},StoreEdition:{1},SoapWorks:{2},ServiceUsedVersion:{3}}}",
				string.IsNullOrWhiteSpace( this.StoreVersion ) ? PredefinedValues.NotAvailable : this.StoreVersion,
				string.IsNullOrWhiteSpace( this.StoreEdition ) ? PredefinedValues.NotAvailable : this.StoreEdition,
				this.SoapWorks,
				string.IsNullOrWhiteSpace( this.ServiceUsedVersion ) ? PredefinedValues.NotAvailable : this.ServiceUsedVersion );
		}
	}
}