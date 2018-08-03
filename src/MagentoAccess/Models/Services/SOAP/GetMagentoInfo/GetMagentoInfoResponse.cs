using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetMagentoInfo
{
	internal class GetMagentoInfoResponse
	{
		public GetMagentoInfoResponse( magentoInfoResponse res, string serviceVersion )
		{
			this.MagentoEdition = res.result.magento_edition;
			this.MagentoVersion = res.result.magento_version;
			this.ServiceVersion = serviceVersion;
		}

		public GetMagentoInfoResponse( MagentoSoapServiceReference_v_1_14_1_EE.magentoInfoResponse res, string serviceVersion )
		{
			this.MagentoEdition = res.result.magento_edition;
			this.MagentoVersion = res.result.magento_version;
			this.ServiceVersion = serviceVersion;
		}

		public GetMagentoInfoResponse( string magentoVersion, string magentoEdition, string serviceVersion )
		{
			this.MagentoEdition = magentoEdition;
			this.MagentoVersion = magentoVersion;
			this.ServiceVersion = serviceVersion;
		}

		public GetMagentoInfoResponse( TsZoey_v_1_9_0_1_CE.magentoInfoResponse res, string serviceVersion )
		{
			this.MagentoEdition = res.result.magento_edition;
			this.MagentoVersion = res.result.magento_version;
			this.ServiceVersion = serviceVersion;
		}

		public string MagentoVersion { get; set; }

		public string MagentoEdition { get; set; }

		public string ServiceVersion { get; set; }
	}
}