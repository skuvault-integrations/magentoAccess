namespace MagentoAccessTestsIntegration.TestEnvironment
{
	public class MagentoConsumerCredentials
	{
		public MagentoConsumerCredentials(string key,string secret)
		{
			Key = key;
			Secret = secret;
		}

		public string Key { get; private set; }
		public string Secret { get; private  set; }
	}
}