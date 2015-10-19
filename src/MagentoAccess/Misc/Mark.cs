using System;

namespace MagentoAccess.Misc
{
	public class Mark
	{
		public string MarkValue { get; private set; }

		public static Mark Blank()
		{
			return new Mark( string.Empty );
		}

		public static Mark CreateNew()
		{
			return new Mark( Guid.NewGuid().ToString() );
		}

		public Mark( string markValue )
		{
			MarkValue = markValue;
		}

		public override string ToString()
		{
			return this.MarkValue;
		}

		public override int GetHashCode()
		{
			return this.MarkValue.GetHashCode();
		}

		#region Equality members
		public bool Equals( Mark other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return string.Equals( this.MarkValue, other.MarkValue, StringComparison.InvariantCultureIgnoreCase );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return this.Equals( ( Mark )obj );
		}
		#endregion
	}

	public static class MarkExtensions
	{
		public static bool IsBlank( this Mark source )
		{
			return source == null || string.IsNullOrWhiteSpace( source.MarkValue );
		}

		public static string ToStringSafe( this Mark source )
		{
			return IsBlank( source ) ? PredefinedValues.EmptyJsonObject : source.ToString();
		}
	}
}