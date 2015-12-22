using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetCategoryTree
{
	internal class GetCategoryTreeResponse
	{
		public GetCategoryTreeResponse( catalogCategoryTreeResponse catalogCategoryTreeResponse )
		{
			if( catalogCategoryTreeResponse == null || catalogCategoryTreeResponse.result != null )
				return;
			var rootCategory = new CategoryNode( catalogCategoryTreeResponse.result );
			RootCategory = rootCategory;
		}

		public GetCategoryTreeResponse( MagentoSoapServiceReference_v_1_14_1_EE.catalogCategoryTreeResponse catalogCategoryTreeResponse )
		{
			if( catalogCategoryTreeResponse == null || catalogCategoryTreeResponse.result != null )
				return;
			var rootCategory = new CategoryNode( catalogCategoryTreeResponse.result );
			RootCategory = rootCategory;
		}

		public CategoryNode RootCategory { get; set; }
	}
}