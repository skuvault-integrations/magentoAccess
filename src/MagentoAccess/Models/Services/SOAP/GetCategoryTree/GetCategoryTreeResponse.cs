using System.Collections.Generic;
using System.Linq;
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

	internal class CategoryNode
	{
		public int Id { get; set; }
		public int ParentId { get; set; }
		public int Level { get; set; }
		public string Name { get; set; }

		public int IsActive { get; set; }

		public List< CategoryNode > Childrens { get; set; }

		public CategoryNode( catalogCategoryEntity category )
		{
			if( category == null )
				return;

			Id = category.category_id;
			Level = category.level;
			Name = category.name;
			ParentId = category.parent_id;
			IsActive = category.is_active;
			Childrens = category.children != null ? category.children.Select( x => new CategoryNode( x ) ).Where( x => x != null ).ToList() : new List< CategoryNode >();
		}

		public CategoryNode( catalogCategoryTree category )
		{
			if( category == null )
				return;
			Id = category.category_id;
			Level = category.level;
			Name = category.name;
			ParentId = category.parent_id;
			IsActive = 1;
			Childrens = category.children != null ? category.children.Select( x => new CategoryNode( x ) ).Where( x => x != null ).ToList() : new List< CategoryNode >();
		}

		public CategoryNode( MagentoSoapServiceReference_v_1_14_1_EE.catalogCategoryTree category )
		{
			if( category == null )
				return;
			Id = category.category_id;
			Level = category.level;
			Name = category.name;
			ParentId = category.parent_id;
			IsActive = 1;
			Childrens = category.children != null ? category.children.Select( x => new CategoryNode( x ) ).Where( x => x != null ).ToList() : new List< CategoryNode >();
		}

		private CategoryNode( MagentoSoapServiceReference_v_1_14_1_EE.catalogCategoryEntity category )
		{
			if( category == null )
				return;

			Id = category.category_id;
			Level = category.level;
			Name = category.name;
			ParentId = category.parent_id;
			IsActive = category.is_active;
			Childrens = category.children != null ? category.children.Select( x => new CategoryNode( x ) ).Where( x => x != null ).ToList() : new List< CategoryNode >();
		}

		public CategoryNode FindCategoryDeep( int id )
		{
			if( this.Id == id )
				return this;

			if( this.Childrens == null || !this.Childrens.Any() )
				return null;

			foreach( var categoryNode in this.Childrens )
			{
				var res = categoryNode.FindCategoryDeep( id );
				if( res != null )
					return res;
			}

			return null;
		}
	}
}