using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PutInventory;

namespace MagentoAccess.Models
{
	public static class ConvertToExtensions
	{
		public static Inventory ToInventory( this Product product, int setQty = 0 )
		{
			return new Inventory() { ProductId = product.ProductId, ItemId = product.EntityId, Qty = setQty };
		}

		public static IEnumerable< Inventory > ToInventory( this IEnumerable< Product > product, Func< Product, int > setQty = null )
		{
			return product.Select( x => x.ToInventory( setQty != null ? setQty( x ) : 0 ) );
		}
	}
}