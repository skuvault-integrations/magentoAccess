﻿using System.Collections;
using System.Collections.Generic;
using MagentoAccess.Models.Services.Rest.v2x;
using NUnit.Framework;

namespace MagentoAccessTests.Services.Rest.v2x
{
	[ TestFixture ]
	class SearchCriteriaTest
	{
		[ Test ]
		[ TestCaseSource( typeof( SearchCriteriaTestCaseSource ), "Cases" ) ]
		public static void SearchCriteriaToString( SearchCriteriaTestCase testTestCase )
		{
			//a
			//a
			var actual = testTestCase.SearchCriteria.ToString();
			//a
			Assert.That( actual, Is.EqualTo( testTestCase.Expected ) );
		}

		public class SearchCriteriaTestCase
		{
			public string Expected { get; set; }
			public SearchCriteria SearchCriteria { get; set; }
		}

		public static class SearchCriteriaTestCaseSource
		{
			public static IEnumerable Cases
			{
				get
				{
					yield return new TestCaseData( new SearchCriteriaTestCase() { Expected = @"searchCriteria=", SearchCriteria = new SearchCriteria { filter_groups = new List<FilterGroup>() } } ).SetName( "SearchCriteria:{}" );
					yield return new TestCaseData( new SearchCriteriaTestCase()
					{
						Expected = @"searchCriteria[filterGroups][0][filters][0][field]=size&" +
						           @"searchCriteria[filterGroups][0][filters][0][value]=Large&" +
						           @"searchCriteria[filterGroups][0][filters][0][conditionType]=eq&" +
						           @"searchCriteria[filterGroups][0][filters][1][field]=color&" +
						           @"searchCriteria[filterGroups][0][filters][1][value]=Red&" +
						           @"searchCriteria[filterGroups][0][filters][1][conditionType]=gt",
						SearchCriteria = new SearchCriteria()
						{
							filter_groups = new List< FilterGroup >()
							{
								new FilterGroup()
								{
									filters = new List< Filter >
									{
										new Filter( @"size", @"Large", Filter.ConditionType.Equals ),
										new Filter( @"color", @"Red", Filter.ConditionType.GreaterThan ),
									}
								}
							}
						}
					} ).SetName( "SearchCriteria{FilterGroup{Filter{Field,Value,Condition},Filter{Field,Value,Condition}}}" );

					yield return new TestCaseData( new SearchCriteriaTestCase()
					{
						Expected = @"searchCriteria[filterGroups][0][filters][0][field]=size&" +
						           @"searchCriteria[filterGroups][0][filters][0][value]=Large&" +
						           @"searchCriteria[filterGroups][0][filters][1][field]=color&" +
						           @"searchCriteria[filterGroups][0][filters][1][value]=Red",
						SearchCriteria = new SearchCriteria()
						{
							filter_groups = new List< FilterGroup >()
							{
								new FilterGroup()
								{
									filters = new List< Filter >
									{
										new Filter( @"size", @"Large" ),
										new Filter( @"color", @"Red" ),
									}
								}
							}
						}
					} ).SetName( "SearchCriteria{FilterGroup{Filter{Field,Value},Filter{Field,Value}}}" );

					yield return new TestCaseData( new SearchCriteriaTestCase()
					{
						Expected = "searchCriteria[filterGroups][0][filters][0][field]=price" +
						           "&searchCriteria[filterGroups][0][filters][0][value]=20" +
						           "&searchCriteria[filterGroups][0][filters][0][conditionType]=lt" +
						           "&searchCriteria[filterGroups][0][filters][1][field]=price" +
						           "&searchCriteria[filterGroups][0][filters][1][value]=50" +
						           "&searchCriteria[filterGroups][0][filters][1][conditionType]=gt" +
						           "&searchCriteria[filterGroups][1][filters][0][field]=price" +
						           "&searchCriteria[filterGroups][1][filters][0][value]=1" +
						           "&searchCriteria[filterGroups][1][filters][0][conditionType]=from" +
						           "&searchCriteria[filterGroups][1][filters][1][field]=price" +
						           "&searchCriteria[filterGroups][1][filters][1][value]=100" +
						           "&searchCriteria[filterGroups][1][filters][1][conditionType]=to" +
						           "&searchCriteria[filterGroups][2][filters][0][field]=price" +
						           "&searchCriteria[filterGroups][2][filters][0][value]=0" +
						           "&searchCriteria[filterGroups][2][filters][0][conditionType]=neq" +
						           "&searchCriteria[currentPage]=1" +
						           "&searchCriteria[pageSize]=100",
						SearchCriteria = new SearchCriteria()
						{
							filter_groups = new List< FilterGroup >()
							{
								new FilterGroup()
								{
									filters = new List< Filter >
									{
										new Filter( @"price", @"20", Filter.ConditionType.LessThan ),
										new Filter( @"price", @"50", Filter.ConditionType.GreaterThan ),
									}
								},
								new FilterGroup()
								{
									filters = new List< Filter >
									{
										new Filter( @"price", @"1", Filter.ConditionType.From ),
										new Filter( @"price", @"100", Filter.ConditionType.To ),
									}
								},
								new FilterGroup()
								{
									filters = new List< Filter >
									{
										new Filter( @"price", @"0", Filter.ConditionType.NotEqual )
									}
								},
							},
							current_page = 1, page_size = 100
						}
					} ).SetName( "SearchCriteria{FilterGroup{Filter{Field,Value,Condition},Filter{Field,Value,Condition}}Pages" );
				}
			}
		}
	}
}