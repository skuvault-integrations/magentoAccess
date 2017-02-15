using System;

namespace MagentoAccess.Misc
{
	public class Mark
	{
		public string MarkValue { get; private set; }
		public Mark Parrent { get; private set; }

		public static Mark Blank()
		{
			return new Mark( string.Empty );
		}

		public static Mark CreateNew()
		{
			return CreateNew( null );
		}

		public static Mark CreateNew( Mark parent )
		{
			return new Mark( Guid.NewGuid().ToString(), parent );
		}

		public Mark( string markValue ) : this( markValue, null )
		{
		}

		public Mark( string markValue, Mark parent )
		{
			this.MarkValue = markValue;
			this.Parrent = parent;
		}

		public override string ToString()
		{
			return this.MarkValue;
		}

		public override int GetHashCode()
		{
			return this.MarkValue.GetHashCode() ^ this.Parrent.GetHashCode();
		}

		#region Equality members
		public bool Equals( Mark other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return string.Equals( this.MarkValue, other.MarkValue, StringComparison.InvariantCultureIgnoreCase ) && this.Parrent.Equals( other.Parrent );
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

		public static Mark CreateChildOrNull( this Mark source )
		{
			return source != null ? Mark.CreateNew( source ) : null;
		}

		public static string ToStringSafe( this Mark source )
		{
			return IsBlank( source ) ? PredefinedValues.EmptyJsonObject : source.ToString();
		}
	}
}