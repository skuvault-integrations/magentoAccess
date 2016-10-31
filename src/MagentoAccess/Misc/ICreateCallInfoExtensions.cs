using System.Runtime.CompilerServices;

namespace MagentoAccess.Misc
{
	public static class ICreateCallInfoExtensions
	{
		public static string CreateMethodCallInfo< T >( this T obj,
			string methodParameters = "",
			string mark = "",
			string errors = "",
			string methodResult = "",
			string additionalInfo = "",
			[ CallerMemberName ] string memberName = ""
			)
		{
			var restInfo = PredefinedValues.EmptyJsonObject;
			var str = $"{{MethodName:{memberName}, " +
			          $"ConnectionInfo:{restInfo}, " +
			          $"MethodParameters:{methodParameters}, " +
			          $"Mark:{mark}" +
			          $"{( string.IsNullOrWhiteSpace( errors ) ? string.Empty : ", Errors:" + errors )}" +
			          $"{( string.IsNullOrWhiteSpace( methodResult ) ? string.Empty : ", Result:" + methodResult )}" +
			          $"{( string.IsNullOrWhiteSpace( additionalInfo ) ? string.Empty : ", " + additionalInfo )}}}";
			return str;
		}
	}
}