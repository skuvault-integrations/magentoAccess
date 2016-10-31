using System.Collections;
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

		[ Test ]
		[ TestCaseSource( typeof( BatchTestCases ), "Cases" ) ]
		public void Batch( BatchTestCase batchCase )
		{
			//------------ Arrange
			var someList = new List< int >();
			for( var i = 0; i < batchCase.ListLength; i++ )
			{
				someList.Add( i );
			}

			//------------ Act
			var batchedList = someList.Batch( batchCase.BatchSize ).ToList();

			//------------ Assert
			var expected = batchCase.ListLength / batchCase.BatchSize + (batchCase.ListLength % batchCase.BatchSize == 0 ? 0 : 1);
			batchedList.Count().Should().Be( expected );
			batchedList.Sum( x => x.Count() ).Should().Be( batchCase.ListLength );
		}

		[Test]
		[TestCaseSource(typeof(BatchTestCases), "Cases")]
		public void SplitToChunks(BatchTestCase batchCase)
		{
			//------------ Arrange
			var someList = new List<int>();
			for (var i = 0; i < batchCase.ListLength; i++)
			{
				someList.Add(i);
			}

			//------------ Act
			var batchedList = someList.SplitToChunks(batchCase.BatchSize).ToList();

			//------------ Assert
			var expected = batchCase.ListLength / batchCase.BatchSize + batchCase.ListLength % batchCase.BatchSize == 0 ? 0 : 1;
			batchedList.Count().Should().Be(expected);
			batchedList.Sum(x => x.Count()).Should().Be(batchCase.ListLength);
		}

		public static class BatchTestCases
		{
			/// <summary>
			/// GetTestStoresCredentials shoud return the same credentials as this method
			/// </summary>
			/// <returns></returns>
			public static IEnumerable Cases
			{
				get
				{
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 1, ListLength = 1000 } ).SetName( "BatchSize = 1, ListLength =	1000" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 999, ListLength = 1000 } ).SetName( "BatchSize = 999, ListLength =	1000" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 1000, ListLength = 1000 } ).SetName( "BatchSize = 1000, ListLength =	1000" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 1001, ListLength = 1000 } ).SetName( "BatchSize = 1001, ListLength =	1000" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 1000, ListLength = 0 } ).SetName( "BatchSize = 1000, ListLength =	0" );
				}
			}
		}
	}

	public class BatchTestCase
	{
		public int BatchSize { get; set; }
		public int ListLength { get; set; }
	}
}