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
		public void ParseMagentoStoreCommunityVersion()
		{
			var storeVersionRaw = "Magento/2.2 (Community)";

			var storeVersion = storeVersionRaw.ParseMagentoStoreInfoString();

			Assert.IsTrue( storeVersion.Version.Equals( new System.Version( 2, 2 ) ) );
			Assert.IsTrue( storeVersion.MagentoEdition.Equals( MagentoEdition.Community ));
		}

		[ Test ]
		public void ParseMagentoStoreCommunityVersionWithAbbr()
		{
			var storeVersionRaw = "Magento/2.2 (CE)";

			var storeVersion = storeVersionRaw.ParseMagentoStoreInfoString();

			Assert.IsTrue( storeVersion.Version.Equals( new System.Version( 2, 2 ) ) );
			Assert.IsTrue( storeVersion.MagentoEdition.Equals( MagentoEdition.Community ));
		}

		[ Test ]
		public void ParseMagentoStoreEnterpiseVersion()
		{
			var storeVersionRaw = "Magento/2.2 (Enterpise)";

			var storeVersion = storeVersionRaw.ParseMagentoStoreInfoString();

			Assert.IsTrue( storeVersion.Version.Equals( new System.Version( 2, 2 ) ) );
			Assert.IsTrue( storeVersion.MagentoEdition.Equals( MagentoEdition.Enterprise ));
		}

		[ Test ]
		public void ParseMagentoStoreCommunityVersionWithBuildInfo()
		{
			var storeVersionRaw = "Magento/2.9.15 (Community)";

			var storeVersion = storeVersionRaw.ParseMagentoStoreInfoString();

			Assert.IsTrue( storeVersion.Version.Equals( new System.Version( 2, 9, 15 ) ) );
			Assert.IsTrue( storeVersion.MagentoEdition.Equals( MagentoEdition.Community ));
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
			var expected = batchCase.ListLength / batchCase.BatchSize + ( batchCase.ListLength % batchCase.BatchSize == 0 ? 0 : 1 );
			batchedList.Count().Should().Be( expected );
			batchedList.Sum( x => x.Count() ).Should().Be( batchCase.ListLength );
		}

		[ Test ]
		[ TestCaseSource( typeof( BatchTestCases ), "Cases" ) ]
		public void SplitToChunks( BatchTestCase batchCase )
		{
			//------------ Arrange
			var someList = new List< int >();
			for( var i = 0; i < batchCase.ListLength; i++ )
			{
				someList.Add( i );
			}

			//------------ Act
			var batchedList = someList.SplitToChunks( batchCase.BatchSize ).ToList();

			//------------ Assert
			batchedList.Count().Should().Be( batchCase.ExpectedBatches );
			batchedList.Sum( x => x.Count() ).Should().Be( batchCase.ListLength );
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
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 1, ListLength = 1000, ExpectedBatches = 1000 } ).SetName( "BatchSize = 1, ListLength =	1000" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 1000, ListLength = 1, ExpectedBatches = 1 } ).SetName( "BatchSize = 1000, ListLength =	1" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 999, ListLength = 1000, ExpectedBatches = 2 } ).SetName( "BatchSize = 999, ListLength =	1000" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 1000, ListLength = 999, ExpectedBatches = 1 } ).SetName( "BatchSize = 1000, ListLength =	999" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 1000, ListLength = 1000, ExpectedBatches = 1 } ).SetName( "BatchSize = 1000, ListLength =	1000" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 1000, ListLength = 0, ExpectedBatches = 0 } ).SetName( "BatchSize = 1000, ListLength =	0" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 50, ListLength = 12, ExpectedBatches = 1 } ).SetName( "BatchSize = 50, ListLength =	12" );
					yield return new TestCaseData( new BatchTestCase() { BatchSize = 12, ListLength = 50, ExpectedBatches = 5 } ).SetName( "BatchSize = 12, ListLength =	50" );
				}
			}
		}
	}

	public class BatchTestCase
	{
		public int BatchSize { get; set; }
		public int ListLength { get; set; }
		public int ExpectedBatches { get; set; }
	}
}