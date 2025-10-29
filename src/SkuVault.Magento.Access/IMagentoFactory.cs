namespace SkuVault.Magento.Access
{
	/// <summary>
	/// A factory class responsible for creating instances of <see cref="IMagentoService"/>. It is used as the entry point to the library.
	/// </summary>
	public interface IMagentoFactory
	{
		IMagentoService CreateService( MagentoAuthenticatedUserCredentials userAuthCredentials, MagentoConfig magentoConfig, MagentoTimeouts operationsTimeouts );
	}
}