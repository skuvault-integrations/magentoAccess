using System;
using System.Collections.Generic;
using System.Text;

namespace MagentoAccess.Models.Services.Rest.v2x
{
	public static class SearchCriteriaExtensions
	{
		public static string StringValue( this Filter.ConditionType type )
		{
			switch( type )
			{
				case Filter.ConditionType.Equals:
					return @"eq";
				case Filter.ConditionType.ValueInSet:
					return @"finset";
				case Filter.ConditionType.From:
					return @"from";
				case Filter.ConditionType.GreaterThan:
					return @"gt";
				case Filter.ConditionType.GreaterThanOrEqual:
					return @"gteq";
				case Filter.ConditionType.In:
					return @"in";
				case Filter.ConditionType.Like:
					return @"like";
				case Filter.ConditionType.LessThan:
					return @"lt";
				case Filter.ConditionType.LessThanOrEqual:
					return @"lteq";
				case Filter.ConditionType.MoreOrEqual:
					return @"moreq";
				case Filter.ConditionType.NotEqual:
					return @"neq";
				case Filter.ConditionType.NotIn:
					return @"nin";
				case Filter.ConditionType.NotNull:
					return @"notnull";
				case Filter.ConditionType.Null:
					return @"null";
				case Filter.ConditionType.To:
					return @"to";
			}
			return null;
		}
	}

	public class SearchCriteria
	{
		public List< FilterGroup > filter_groups { get; set; }
		public List< SortOrder > sort_orders { get; set; }
		public int page_size { get; set; }
		public int current_page { get; set; }

		public override string ToString()
		{
			var groups = this.filter_groups.ToArray();
			var sb = new StringBuilder();
			for( var i = 0; i < groups.Length; i++ )
			{
				for( var j = 0; j < groups[ i ].filters.Count; j++ )
				{
					var filters = groups[ i ].filters.ToArray();
					if( sb.Length > 0 )
						sb.Append( @"&" );
					sb.Append( $@"searchCriteria[filter_groups][{i}][filters][{j}][field]={filters[ j ].field}" + $@"&searchCriteria[filter_groups][{i}][filters][{j}][value]={Uri.EscapeDataString( filters[ j ].value )}" );
					if( !string.IsNullOrWhiteSpace( filters[ j ].condition_type ) )
						sb.Append( $@"&searchCriteria[filter_groups][{i}][filters][{j}][condition_type]={filters[ j ].condition_type}" );
				}
			}
			if( sb.Length == 0 )
				sb.Append( @"searchCriteria=" );
			if( this.current_page > 0 )
				sb.Append( $@"&searchCriteria[current_page]={this.current_page}" );
			if( this.page_size > 0 )
				sb.Append( $@"&searchCriteria[page_size]={this.page_size}" );
			return sb.ToString();
		}
	}

	public class FilterGroup
	{
		public List< Filter > filters { get; set; }
	}

	public class Filter
	{
		public string field { get; set; }
		public string value { get; set; }
		public string condition_type { get; set; }

		public Filter( string field, string value, ConditionType? condition = null )
		{
			this.field = field;
			this.value = value;
			this.condition_type = condition != null ? condition.ToString() : null;
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
	}

	public class SortOrder
	{
		public string field { get; set; }
		public string direction { get; set; }
	}
}