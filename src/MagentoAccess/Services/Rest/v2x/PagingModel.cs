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

		public IEnumerable< List< T > > GetPages< T >( IEnumerable< T > source )
		{
			var sourceList = source as IList< T > ?? source.ToList();
			var fullPagesCount = sourceList.Count() / this.ItemsPerPage;
			var diff = ( sourceList.Count() - fullPagesCount * this.ItemsPerPage );
			var pages = Enumerable.Range( this.CurrentPage + 1, fullPagesCount + ( diff > 0 ? 1 : 0 ) ).ToList();
			var exit = false;
			var j = 0;
			for( var i = 0; i < pages.Count() && !exit; i++ )
			{
				var chunk = new List< T >();
				for( var k = 0; k < this.ItemsPerPage; k++ )
				{
					if( j < sourceList.Count() )
						chunk.Add( sourceList[ j++ ] );
					else
					{
						exit = true;
						break;
					}
				}
				yield return chunk;
			}
		}

		public IEnumerable< int > GetPages( int itemsTotal, int limit )
		{
			var minValues = itemsTotal < limit ? itemsTotal : limit;
			return this.GetPages( minValues );
		}
	}
}