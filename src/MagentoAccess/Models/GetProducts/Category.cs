using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Models.Services.Soap.GetCategoryTree;

namespace MagentoAccess.Models.GetProducts
{
	[ Serializable ]
	public class Category : IEquatable< Category >
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

		public Category( string id )
		{
			int temp;
			Id = int.TryParse( id, out temp ) ? temp : 0;
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

		public bool Equals( Category other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return this.Id == other.Id && 
			       this.ParentId == other.ParentId && 
			       this.Level == other.Level && 
			       string.Equals( this.Name, other.Name ) && 
			       this.IsActive == other.IsActive && 
			       Equals( this.Childrens, other.Childrens );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return Equals( ( Category )obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = this.Id;
				hashCode = ( hashCode * 397 ) ^ this.ParentId;
				hashCode = ( hashCode * 397 ) ^ this.Level;
				hashCode = ( hashCode * 397 ) ^ ( this.Name != null ? this.Name.GetHashCode() : 0 );
				hashCode = ( hashCode * 397 ) ^ this.IsActive;
				hashCode = ( hashCode * 397 ) ^ ( this.Childrens != null ? this.Childrens.GetHashCode() : 0 );
				return hashCode;
			}
		}
	}
}