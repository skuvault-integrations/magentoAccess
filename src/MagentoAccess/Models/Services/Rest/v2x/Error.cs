using System.Collections.Generic;

namespace MagentoAccess.Models.Services.Rest.v2x
{
	public class Parameter
	{
		public string resources { get; set; }
		public string fieldName { get; set; }
		public string fieldValue { get; set; }
	}

	public class Error
	{
		public string message { get; set; }
		public List< Parameter > parameters { get; set; }
	}

	public class RootObject
	{
		public string message { get; set; }
		public List< Error > errors { get; set; }
		public int code { get; set; }
		public List< Parameter > parameters { get; set; }
		public string trace { get; set; }
	}
}