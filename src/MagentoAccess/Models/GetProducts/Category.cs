using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;

namespace MagentoAccess.Models.GetProducts
{
	public class Category
	{
		internal Category( CategoryNode value )
		{
			Id = value.Id;
			ParentId = value.ParentId;
			Level = value.Level;
			Name = value.Name;
			IsActive = value.IsActive;

			if( value.Childrens != null )
				Childrens = value.Childrens.Select( x => new Category( x ) ).ToList();
		}

		public int Id { get; set; }
		public int ParentId { get; set; }
		public int Level { get; set; }
		public string Name { get; set; }

		public int IsActive { get; set; }

		public List< Category > Childrens { get; set; }

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
	}
}