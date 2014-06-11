using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MagentoAccess.Misc;
using NUnit.Framework;

namespace MagentoAccessTests.Misc
{
	[ TestFixture ]
	public class ExtensionsTest
	{
		[ Test ]
		public void Test()
		{
			//------------ Arrange
			var list1 = new List< int >();
			for( var i = 0; i < 121; i++ )
			{
				list1.Add( i + 10 );
			}

			//------------ Act
			var list1c = list1.SplitToChunks( 4 ).ToList();

			//------------ Assert
			list1c.Count().Should().Be( 31 );
		}
	}
}