﻿using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using MagentoAccess.Misc;
using MagentoAccess.Services.Rest.v2x;
using NUnit.Framework;

namespace MagentoAccessTests.Services.Rest.v2x
{
	[ TestFixture ]
	public class PagingModelTest
	{
		[ Test ]
		[ TestCaseSource( typeof( GeneralTestCases ), "TestStoresCredentials" ) ]
		public void GetPages( PagetestCase pagetest )
		{
			//a
			PagingModel p = new PagingModel( pagetest.ItemsPerPage, pagetest.CurrentPage );
			//a
			var pages = p.GetPages( pagetest.PagesTotal );
			//a
			pages.Should().BeEquivalentTo( pagetest.PagesToLoad );
		}

		public static class GeneralTestCases
		{
			/// <summary>
			/// GetTestStoresCredentials shoud return the same credentials as this method
			/// </summary>
			/// <returns></returns>
			public static IEnumerable TestStoresCredentials
			{
				get
				{
					yield return new TestCaseData( new PagetestCase() { CurrentPage = 1, ItemsPerPage = 100, PagesTotal = 1000, PagesToLoad = new List< int >() { 2, 3, 4, 5, 6, 7, 8, 9, 10 } } ).SetName( "1-100-1000" );
					yield return new TestCaseData( new PagetestCase() { CurrentPage = 2, ItemsPerPage = 100, PagesTotal = 1000, PagesToLoad = new List< int >() { 3, 4, 5, 6, 7, 8, 9, 10 } } ).SetName( "0-100-1000" );

					yield return new TestCaseData( new PagetestCase() { CurrentPage = 1, ItemsPerPage = 100, PagesTotal = 1001, PagesToLoad = new List< int >() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 } } ).SetName( "1-100-1001" );
					yield return new TestCaseData( new PagetestCase() { CurrentPage = 2, ItemsPerPage = 100, PagesTotal = 1001, PagesToLoad = new List< int >() { 3, 4, 5, 6, 7, 8, 9, 10, 11 } } ).SetName( "0-100-1001" );

					yield return new TestCaseData( new PagetestCase() { CurrentPage = 9, ItemsPerPage = 100, PagesTotal = 1000, PagesToLoad = new List< int >() { 10 } } ).SetName( "9-100-1000" );
					yield return new TestCaseData( new PagetestCase() { CurrentPage = 10, ItemsPerPage = 100, PagesTotal = 1000, PagesToLoad = new List< int >() { } } ).SetName( "10-100-1000" );
				}
			}
		}

		public class PagetestCase
		{
			public int ItemsPerPage { get; set; }
			public int CurrentPage { get; set; }
			public int PagesTotal { get; set; }
			public List< int > PagesToLoad { get; set; }
		}
	}
}