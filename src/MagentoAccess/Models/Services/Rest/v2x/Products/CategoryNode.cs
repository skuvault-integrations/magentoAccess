using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MagentoAccess.Models.Services.Rest.v2x.Products
{
	[ JsonConverter( typeof( LaxPropertyNameMatchingConverter ) ) ]
	public class CategoryNode
	{
		public int id { get; set; }
		public int parentId { get; set; }
		public string name { get; set; }
		public bool isActive { get; set; }
		public int position { get; set; }
		public int level { get; set; }

		public List< CategoryNode > childrenData { get; set; }
		
		public List< CategoryNode > Flatten()
		{
			if( this.childrenData == null || !this.childrenData.Any() )
				return new List< CategoryNode >() { this };

			var res = this.childrenData.SelectMany( children => children.Flatten() ).ToList();
			res.Add( this );
			return res;
		}
	}
}
