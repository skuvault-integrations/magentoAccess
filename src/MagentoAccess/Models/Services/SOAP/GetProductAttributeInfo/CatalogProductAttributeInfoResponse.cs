using System.Collections.Generic;
using System.Linq;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetProductAttributeInfo
{
	internal class CatalogProductAttributeInfoResponse
	{
		public CatalogProductAttributeInfoResponse( catalogProductAttributeInfoResponse res )
		{
			if( res != null && res.result != null && res.result.options != null && res.result.options.Any() )
				this.Attributes = res.result.options.Select( x => new ProductAttributeInfo( x.label, x.value ) ).ToList();
		}

		public CatalogProductAttributeInfoResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogProductAttributeInfoResponse res )
		{
			if( res != null && res.result != null && res.result.options != null && res.result.options.Any() )
				this.Attributes = res.result.options.Select( x => new ProductAttributeInfo( x.label, x.value ) ).ToList();
		}

		public List< ProductAttributeInfo > Attributes{ get; set; }
	}
}