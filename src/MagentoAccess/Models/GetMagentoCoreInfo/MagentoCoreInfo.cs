namespace MagentoAccess.Models.GetMagentoCoreInfo
{
	public class MagentoCoreInfo
	{
		public string Version { get; private set; }
		public string Edition { get; private set; }

		public MagentoCoreInfo( string magentoVersion, string magentoEdition )
		{
			this.Version = magentoVersion;
			this.Edition = magentoEdition;
		}
	}
}