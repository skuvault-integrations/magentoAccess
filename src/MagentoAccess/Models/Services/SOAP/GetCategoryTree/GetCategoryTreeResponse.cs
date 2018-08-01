using MagentoAccess.Magento2catalogCategoryManagementV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetCategoryTree
{
	internal class GetCategoryTreeResponse
	{
		public GetCategoryTreeResponse( catalogCategoryTreeResponse catalogCategoryTreeResponse )
		{
			if( catalogCategoryTreeResponse == null || catalogCategoryTreeResponse.result == null )
				return;
			var rootCategory = new CategoryNode( catalogCategoryTreeResponse.result );
			RootCategory = rootCategory;
		}

		public GetCategoryTreeResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogCategoryTreeResponse catalogCategoryTreeResponse )
		{
			if( catalogCategoryTreeResponse == null || catalogCategoryTreeResponse.result == null )
				return;
			var rootCategory = new CategoryNode( catalogCategoryTreeResponse.result );
			RootCategory = rootCategory;
		}

		public GetCategoryTreeResponse( catalogCategoryManagementV1GetTreeResponse1 catalogCategoryTreeResponse )
		{
			if( catalogCategoryTreeResponse == null || catalogCategoryTreeResponse.catalogCategoryManagementV1GetTreeResponse == null
			    || catalogCategoryTreeResponse.catalogCategoryManagementV1GetTreeResponse.result == null )
				return;
			var rootCategory = new CategoryNode( catalogCategoryTreeResponse.catalogCategoryManagementV1GetTreeResponse.result );
			this.RootCategory = rootCategory;
		}

		public GetCategoryTreeResponse( Magento2catalogCategoryManagementV1_v_2_1_0_0_CE.catalogCategoryManagementV1GetTreeResponse1 catalogCategoryTreeResponse )
		{
			if( catalogCategoryTreeResponse == null || catalogCategoryTreeResponse.catalogCategoryManagementV1GetTreeResponse == null
			    || catalogCategoryTreeResponse.catalogCategoryManagementV1GetTreeResponse.result == null )
				return;
			var rootCategory = new CategoryNode( catalogCategoryTreeResponse.catalogCategoryManagementV1GetTreeResponse.result );
			this.RootCategory = rootCategory;
		}

		public GetCategoryTreeResponse( TsZoey_v_1_9_0_1_CE.catalogCategoryTreeResponse catalogCategoryTreeResponse )
		{
			if (catalogCategoryTreeResponse == null || catalogCategoryTreeResponse.result == null)
				return;
			var rootCategory = new CategoryNode(catalogCategoryTreeResponse.result);
			RootCategory = rootCategory;
		}

		public CategoryNode RootCategory { get; set; }
	}
}