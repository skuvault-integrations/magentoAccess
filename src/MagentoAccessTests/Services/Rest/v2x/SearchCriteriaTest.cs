using System.Collections;
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
			Assert.AreEqual( testTestCase.Expected, actual );
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
					yield return new TestCaseData( new SearchCriteriaTestCase() { Expected = @"searchCriteria=", SearchCriteria = new SearchCriteria() } ).SetName( "SearchCriteria:{}" );
					yield return new TestCaseData( new SearchCriteriaTestCase()
					{
						Expected = @"searchCriteria[filter_groups][0][filters][0][field]=size&" +
						           @"searchCriteria[filter_groups][0][filters][0][value]=Large&" +
						           @"searchCriteria[filter_groups][0][filters][0][condition_type]=eq&" +
						           @"searchCriteria[filter_groups][0][filters][1][field]=color&" +
						           @"searchCriteria[filter_groups][0][filters][1][value]=Red&" +
						           @"searchCriteria[filter_groups][0][filters][1][condition_type]=eq",
						SearchCriteria = new SearchCriteria()
						{
							filter_groups = new List< FilterGroup >()
							{
								new FilterGroup()
								{
									filters = new List< Filter >
									{
										new Filter( @"size", @"Large", Filter.ConditionType.GreaterThan ),
										new Filter( @"color", @"Red", Filter.ConditionType.GreaterThan ),
									}
								}
							}
						}
					} ).SetName( "SearchCriteria{FilterGroup{Filter{Field,Value,Condition},Filter{Field,Value,Condition}}}" );

					yield return new TestCaseData( new SearchCriteriaTestCase()
					{
						Expected = @"searchCriteria[filter_groups][0][filters][0][field]=size&" +
						           "searchCriteria[filter_groups][0][filters][0][value]=Large&" +
						           @"searchCriteria[filter_groups][0][filters][1][field]=color&" +
						           @"searchCriteria[filter_groups][0][filters][1][value]=Red",
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
						Expected = "searchCriteria[filter_groups][0][filters][0][field]=price" +
						           "&searchCriteria[filter_groups][0][filters][0][value]=20" +
						           "&searchCriteria[filter_groups][0][filters][0][condition_type]=lt" +
						           "&searchCriteria[filter_groups][0][filters][1][field]=price" +
						           "&searchCriteria[filter_groups][0][filters][1][value]=50" +
						           "&searchCriteria[filter_groups][0][filters][1][condition_type]=gt" +
						           "&searchCriteria[filter_groups][1][filters][0][field]=price" +
						           "&searchCriteria[filter_groups][1][filters][0][value]=1" +
						           "&searchCriteria[filter_groups][1][filters][0][condition_type]=from" +
						           "&searchCriteria[filter_groups][1][filters][1][field]=price" +
						           "&searchCriteria[filter_groups][1][filters][1][value]=100" +
						           "&searchCriteria[filter_groups][1][filters][1][condition_type]=to" +
						           "&searchCriteria[filter_groups][2][filters][0][field]=price" +
						           "&searchCriteria[filter_groups][2][filters][0][value]=0" +
						           "&searchCriteria[filter_groups][2][filters][0][condition_type]=neq" +
						           "&searchCriteria[current_page]=1" +
						           "&searchCriteria[page_size]=100",
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