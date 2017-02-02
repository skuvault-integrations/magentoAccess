using System.Threading.Tasks;

namespace MagentoAccess.Services.Rest.v2x.Repository
{
	public static class Extension
	{
		public static T WaitResult< T >( this Task< T > t )
		{
			t.Wait();
			return t.Result;
		}
	}
}