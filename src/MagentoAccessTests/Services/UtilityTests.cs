using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagentoAccess.Models.Services.Rest;
using NUnit.Framework;

namespace AbilityMagentoAccessTests
{
    [TestFixture]
    class UtilityTests
    {
        [Test]
        public void SearchCriteriaTest()
        {
            const String expected1 = @"searchCriteria=";
            Assert.AreEqual(expected1, new SearchCriteria().ToString());
            
            const String expected2 = @"searchCriteria[filter_groups][0][filters][0][field]=size&" +
            "searchCriteria[filter_groups][0][filters][0][value]=Large&" +
            @"searchCriteria[filter_groups][0][filters][0][condition_type]=eq&" +
            @"searchCriteria[filter_groups][0][filters][1][field]=color&" +
            @"searchCriteria[filter_groups][0][filters][1][value]=Red&" +
            @"searchCriteria[filter_groups][0][filters][1][condition_type]=eq";

            var searchCriteria2 = new SearchCriteria(new List<SearchCriteria.FilterGroup>()
            {
                new SearchCriteria.FilterGroup(new List<SearchCriteria.FilterGroup.Filter>()
                {
                    new SearchCriteria.FilterGroup.Filter(@"size", @"Large", SearchCriteria.FilterGroup.Filter.ConditionType.Equals),
                    new SearchCriteria.FilterGroup.Filter(@"color", @"Red", SearchCriteria.FilterGroup.Filter.ConditionType.Equals)
                })
            });

            Assert.AreEqual(expected2, searchCriteria2.ToString());

            const String expected3 = @"searchCriteria[filter_groups][0][filters][0][field]=size&" +
            "searchCriteria[filter_groups][0][filters][0][value]=Large&" +
            @"searchCriteria[filter_groups][0][filters][1][field]=color&" +
            @"searchCriteria[filter_groups][0][filters][1][value]=Red";

            var searchCriteria3 = new SearchCriteria(new List<SearchCriteria.FilterGroup>()
            {
                new SearchCriteria.FilterGroup(new List<SearchCriteria.FilterGroup.Filter>()
                {
                    new SearchCriteria.FilterGroup.Filter(@"size", @"Large"),
                    new SearchCriteria.FilterGroup.Filter(@"color", @"Red")
                })
            });

            Assert.AreEqual(expected3, searchCriteria3.ToString());

            const String expected4 = @"searchCriteria[filter_groups][0][filters][0][field]=price&searchCriteria[filter_groups][0][filters][0]" +
                            @"[value]=20&searchCriteria[filter_groups][0][filters][0][condition_type]=lt&searchCriteria[filter_groups][0][filters][1][field]" +
                            @"=price&searchCriteria[filter_groups][0][filters][1][value]=50&searchCriteria[filter_groups][0][filters][1][condition_type]" +
                            @"=gt&searchCriteria[filter_groups][1][filters][0][field]=price&searchCriteria[filter_groups][1][filters][0][value]=1&searchCriteria" +
                            @"[filter_groups][1][filters][0][condition_type]=from&searchCriteria[filter_groups][1][filters][1][field]=price&searchCriteria" +
                            @"[filter_groups][1][filters][1][value]=100&searchCriteria[filter_groups][1][filters][1][condition_type]=to&searchCriteria[filter_groups][2]" +
                            @"[filters][0][field]=price&searchCriteria[filter_groups][2][filters][0][value]=0&searchCriteria[filter_groups][2][filters][0]" +
                            @"[condition_type]=neq&searchCriteria[current_page]=1&searchCriteria[page_size]=100";

            var searchCriteria4 = new SearchCriteria(
                    new List<SearchCriteria.FilterGroup>{
                        new SearchCriteria.FilterGroup( 
                            new List<SearchCriteria.FilterGroup.Filter>
                            {
                                new SearchCriteria.FilterGroup.Filter(
                                    @"price",
                                    @"20",
                                    SearchCriteria.FilterGroup.Filter.ConditionType.LessThan
                                    ),
                                new SearchCriteria.FilterGroup.Filter(
                                    @"price",
                                    @"50",
                                    SearchCriteria.FilterGroup.Filter.ConditionType.GreaterThan
                                    )
                            }),
                        new SearchCriteria.FilterGroup(
                            new List<SearchCriteria.FilterGroup.Filter>
                            {
                                new SearchCriteria.FilterGroup.Filter(
                                    @"price",
                                    @"1",
                                    SearchCriteria.FilterGroup.Filter.ConditionType.From
                                    ),
                                new SearchCriteria.FilterGroup.Filter(
                                    @"price",
                                    @"100",
                                    SearchCriteria.FilterGroup.Filter.ConditionType.To
                                    )
                            }),
                        new SearchCriteria.FilterGroup(
                            new List<SearchCriteria.FilterGroup.Filter>
                            {
                                new SearchCriteria.FilterGroup.Filter(
                                    @"price",
                                    @"0",
                                    SearchCriteria.FilterGroup.Filter.ConditionType.NotEqual
                                    )
                            })
                    }) { CurrentPage = 1, PageSize = 100 };

            Assert.AreEqual(expected4, searchCriteria4.ToString());

        }
    }
}
