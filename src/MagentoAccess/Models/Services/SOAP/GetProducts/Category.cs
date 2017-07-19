using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;

namespace MagentoAccess.Models.Services.Soap.GetProducts
{
	[ Serializable ]
	public class Category
	{
		internal Category( CategoryNode value )
		{
			this.Id = value.Id;
			this.ParentId = value.ParentId;
			this.Level = value.Level;
			this.Name = value.Name;
			this.IsActive = value.IsActive;

			if( value.Childrens != null )
				this.Childrens = value.Childrens.Select( x => new Category( x ) ).ToList();
		}
		
		internal Category( MagentoAccess.Models.Services.Rest.v2x.Products.CategoryNode value )
		{
			this.Id = value.id;
			this.ParentId = value.parentId;
			this.Level = value.level;
			this.Name = value.name;
			this.IsActive = value.isActive ? 1 : 0;

			if( value.childrenData != null )
				this.Childrens = value.childrenData.Select( x => new Category( x ) ).ToList();
		}

		public Category( string id )
		{
			int temp;
			this.Id = int.TryParse( id, out temp ) ? temp : 0;
		}

		public Category( Models.GetProducts.Category category )
		{
			var categoryDeepClone = category.DeepClone();
			this.Id = categoryDeepClone.Id;
			this.ParentId = categoryDeepClone.ParentId;
			this.Level = categoryDeepClone.Level;
			this.Name = categoryDeepClone.Name;
			this.IsActive = categoryDeepClone.IsActive;
			this.Childrens = categoryDeepClone.Childrens.Select( x => new Category( x ) ).ToList();
		}

		public int Id{ get; set; }
		public int ParentId{ get; set; }
		public int Level{ get; set; }
		public string Name{ get; set; }
		public int IsActive{ get; set; }
		public List< Category > Childrens{ get; set; }

		public List< Category > Flatten()
		{
			var res = new List< Category >();

			if( this.Childrens == null || !this.Childrens.Any() )
				return new List< Category >() { { this } };

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

		public Category FindCategoryDeep( int id )
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

		public Models.GetProducts.Category ToCategory()
		{
			var result = new Models.GetProducts.Category( this.Id.ToString() )
			{
				ParentId = this.ParentId,
				Level = this.Level,
				Name = this.Name,
				IsActive = this.IsActive,
			};
			if( this.Childrens != null )
				result.Childrens = this.Childrens.Select( z => z.ToCategory() ).ToList();

			return result;
		}
	}
}