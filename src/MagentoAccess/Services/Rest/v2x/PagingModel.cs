using System.Collections.Generic;
using System.Linq;

namespace MagentoAccess.Services.Rest.v2x
{
	public class PagingModel
	{
		public int ItemsPerPage { get; }
		public int CurrentPage { get; }

		public PagingModel(int itemsPerPage, int currentPage)
		{
			this.ItemsPerPage = itemsPerPage;
			this.CurrentPage = currentPage;
		}

		public IEnumerable< int > GetPages( int itemsTotal )
		{
			var fullPagesCount = itemsTotal / this.ItemsPerPage;
			var diff = ( itemsTotal - fullPagesCount * this.ItemsPerPage );
			return Enumerable.Range( this.CurrentPage + 1, fullPagesCount + ( diff > 0 ? 1 : 0 ) );
		}
	}
}