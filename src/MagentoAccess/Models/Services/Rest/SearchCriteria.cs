using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagentoAccess.Models.Services.Rest
{
    public static class SearchCriteriaExtensions
    {
        public static String StringValue( this SearchCriteria.FilterGroup.Filter.ConditionType type )
        {
            switch( type )
            {
                case SearchCriteria.FilterGroup.Filter.ConditionType.Equals:
                    return @"eq";
                case SearchCriteria.FilterGroup.Filter.ConditionType.ValueInSet:
                    return @"finset";
                case SearchCriteria.FilterGroup.Filter.ConditionType.From:
                    return @"from";
                case SearchCriteria.FilterGroup.Filter.ConditionType.GreaterThan:
                    return @"gt";
                case SearchCriteria.FilterGroup.Filter.ConditionType.GreaterThanOrEqual:
                    return @"gteq";
                case SearchCriteria.FilterGroup.Filter.ConditionType.In:
                    return @"in";
                case SearchCriteria.FilterGroup.Filter.ConditionType.Like:
                    return @"like";
                case SearchCriteria.FilterGroup.Filter.ConditionType.LessThan:
                    return @"lt";
                case SearchCriteria.FilterGroup.Filter.ConditionType.LessThanOrEqual:
                    return @"lteq";
                case SearchCriteria.FilterGroup.Filter.ConditionType.MoreOrEqual:
                    return @"moreq";
                case SearchCriteria.FilterGroup.Filter.ConditionType.NotEqual:
                    return @"neq";
                case SearchCriteria.FilterGroup.Filter.ConditionType.NotIn:
                    return @"nin";
                case SearchCriteria.FilterGroup.Filter.ConditionType.NotNull:
                    return @"notnull";
                case SearchCriteria.FilterGroup.Filter.ConditionType.Null:
                    return @"null";
                case SearchCriteria.FilterGroup.Filter.ConditionType.To:
                    return @"to";
            }
            return null;
        }
    }

    public class SearchCriteria
    {
        public SearchCriteria()
        {
            this.FilterGroups = new List< FilterGroup >();
        }

        public SearchCriteria( IEnumerable< FilterGroup > filtergroups )
            : this()
        {
            this.AddRange( filtergroups );
        }

        public int CurrentPage { get; set; }
        public List< FilterGroup > FilterGroups { get; set; }
        public int PageSize { get; set; }

        public void Add( FilterGroup filtergroup )
        {
            this.FilterGroups.Add( filtergroup );
        }

        public void AddRange( IEnumerable< FilterGroup > filtergroups )
        {
            foreach( FilterGroup group in filtergroups )
            {
                this.Add( group );
            }
        }

        public override string ToString()
        {
            FilterGroup[] groups = this.FilterGroups.ToArray();
            var sb = new StringBuilder();
            for( var i = 0; i < groups.Length; i++ )
            {
                for( var j = 0; j < groups[ i ].Filters.Count(); j++ )
                {
                    FilterGroup.Filter[] filters = groups[ i ].Filters.ToArray();
                    if( sb.Length > 0 )
                        sb.Append( @"&" );
                    sb.Append( String.Format( @"searchCriteria[filter_groups][{0}][filters][{1}][field]={2}" +
                                              @"&searchCriteria[filter_groups][{0}][filters][{1}][value]={3}", i, j, filters[ j ].Field, Uri.EscapeDataString(filters[ j ].Value) ) );
                    if( filters[ j ].Condition.HasValue )
                        sb.Append( String.Format( @"&searchCriteria[filter_groups][{0}][filters][{1}][condition_type]={2}", i, j, filters[ j ].Condition.Value.StringValue() ) );
                }
            }
            if( sb.Length == 0 )
                sb.Append( @"searchCriteria=" );
            if( this.CurrentPage > 0 )
                sb.Append( String.Format( @"&searchCriteria[current_page]={0}", this.CurrentPage ) );
            if( this.PageSize > 0 )
                sb.Append( String.Format( @"&searchCriteria[page_size]={0}", this.PageSize ) );
            return sb.ToString();
        }

        public class FilterGroup
        {
            public FilterGroup()
            {
                this.Filters = new List< Filter >();
            }

            public FilterGroup( IEnumerable< Filter > filters )
                : this()
            {
                this.AddRange( filters );
            }

            public List< Filter > Filters { get; set; }

            public void Add( Filter filter )
            {
                this.Filters.Add( filter );
            }

            public void AddRange( IEnumerable< Filter > filters )
            {
                foreach( Filter filter in filters )
                {
                    this.Add( filter );
                }
            }

            public class Filter
            {
                public Filter( String field, String value, ConditionType? condition = null )
                {
                    this.Field = field;
                    this.Value = value;
                    this.Condition = condition;
                }

                public enum ConditionType
                {
                    Equals,
                    ValueInSet,
                    From,
                    To,
                    GreaterThan,
                    GreaterThanOrEqual,
                    In,
                    Like,
                    LessThan,
                    LessThanOrEqual,
                    MoreOrEqual,
                    NotEqual,
                    NotIn,
                    NotNull,
                    Null
                }

                public ConditionType? Condition { get; private set; }

                public String Field { get; private set; }
                public String Value { get; private set; }
            }
        }
    }
}