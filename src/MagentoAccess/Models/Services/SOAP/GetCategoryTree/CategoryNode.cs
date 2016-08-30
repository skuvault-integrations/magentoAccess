using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Magento2catalogCategoryManagementV1_v_2_0_2_0_CE;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Models.Services.Soap.GetCategoryTree
{
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

		public CategoryNode( CatalogDataCategoryTreeInterface category )
		{
			if( category == null )
				return;

			this.Id = category.id;
			this.Level = category.level;
			this.Name = category.name;
			this.ParentId = category.parentId;
			this.IsActive = string.Compare( category.isActive, "true", StringComparison.InvariantCultureIgnoreCase ) == 0 ? 1 : 0;
			this.Childrens = category.childrenData != null ? category.childrenData.Select( x => new CategoryNode( x ) ).Where( x => x != null ).ToList() : new List< CategoryNode >();
		}

		public CategoryNode( Magento2catalogCategoryManagementV1_v_2_1_0_0_CE.CatalogDataCategoryTreeInterface category )
		{
			if( category == null )
				return;

			this.Id = category.id;
			this.Level = category.level;
			this.Name = category.name;
			this.ParentId = category.parentId;
			this.IsActive = category.isActive ? 1 : 0;
			this.Childrens = category.childrenData != null ? category.childrenData.Select( x => new CategoryNode( x ) ).Where( x => x != null ).ToList() : new List< CategoryNode >();
		}

		public List< CategoryNode > Flatten()
		{
			var res = new List< CategoryNode >();

			if( this.Childrens == null || !this.Childrens.Any() )
				return new List< CategoryNode >() { this };

			foreach( var children in this.Childrens )
			{
				var dictionary = children.Flatten();
				foreach( var subChildren in dictionary )
				{
					res.Add( subChildren );
				}
			}
			res.Add( this );
			return res;
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