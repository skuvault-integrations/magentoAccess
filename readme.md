MagentoAccess is .NET wrapper around [Magento API](http://www.magentocommerce.com/api/rest/introduction.html) was created to simplify and speed up working with it.

# Supported versions
MagentoAccess tested with these Magento versions. 


**Community Edition**
* 1.7.0.2
* 1.8.1.0
* 1.9.0.1
* 1.9.1.0
* 1.9.2.0
* 1.9.2.1
* 1.9.2.2
* 1.9.2.3
* 1.9.2.4
* 1.9.3.1
* 2.0.2
* 2.0.4
* 2.0.5
* 2.0.6
* 2.0.7
* 2.0.8
* 2.0.9
* 2.0.10
* 2.0.11
* 2.0.13
* 2.1.0
* 2.1.1
* 2.1.2
* 2.1.3
* 2.1.5

**Enterprise Edition**
* 1.14.1.0


# Getting Started

Install the [NuGet package](https://www.nuget.org/packages/MagentoAccess).

**Create service and get products from Magento**

For most of magento versions ```SoapApiUser```, ```SoapApiKey```, ```StoreUrl``` will be enough (set other parameters as empty strings). MagentoAccess interacts with store through SOAP ( REST supported only for Magento 2), WS-I compliance should be turned on.
```C#
			var servicesFactory = new MagentoFactory();

			var magentoService = servicesFactory.CreateService(new MagentoAuthenticatedUserCredentials( "n/a", "n/a", "https://www.youstore.com", "n/a", "n/a", "User", "Password", 4, 1800000, false ), new MagentoConfig() { EditionByDefault = "ce", VersionByDefault = "2.1.0.0", Protocol = MagentoDefaultProtocol.SoapOnly } ); // 2.1.0.0. used instead of 2.1.0 for back compatibility with magento 1.x.x.x

			// Call only if you are not sure about your magento store version specified in CreateService.
			// Here magentoService will try to determine your store version and configure itself to work with your store.
			// This may take few minutes.
			var magentoVersion = await magentoService.DetermineMagentoVersionAndSetupServiceAsync();

			var magentoInventory = await magentoService.GetProductsAsync();
```


**Other methods to work with Magento**

To work with magento store you should use ```public class MagentoService : IMagentoService```. Here you can see all methods to work with store.
```C#
  public interface IMagentoService
  {
    MagentoService.SaveAccessToken AfterGettingToken { get; set; }

    TransmitVerificationCodeDelegate TransmitVerificationCode { get; set; }

    Func<string> AdditionalLogInfo { get; set; }

    Task<IEnumerable<Order>> GetOrdersAsync(DateTime dateFrom, DateTime dateTo);

    Task<IEnumerable<Order>> GetOrdersAsync();

    Task UpdateInventoryAsync(IEnumerable<Inventory> products);

    Task<IEnumerable<Product>> GetProductsSimpleAsync();

    Task<IEnumerable<Product>> GetProductsAsync(bool includeDetails = false);

    VerificationData RequestVerificationUri();

    void PopulateAccessTokenAndAccessTokenSecret(string verificationCode, string requestToken, string requestTokenSecret);

    Task<PingSoapInfo> PingSoapAsync(Mark mark = null);

    Task<PingRestInfo> PingRestAsync();

    Task UpdateInventoryBySkuAsync(IEnumerable<InventoryBySku> inventory);

    Task<IEnumerable<CreateProductModelResult>> CreateProductAsync(IEnumerable<CreateProductModel> models);

    Task<IEnumerable<DeleteProductModelResult>> DeleteProductAsync(IEnumerable<DeleteProductModel> models);

    Task<IEnumerable<CreateOrderModelResult>> CreateOrderAsync(IEnumerable<CreateOrderModel> models);

    Task<IEnumerable<Order>> GetOrdersAsync(IEnumerable<string> orderIds);

    Task<IEnumerable<Product>> FillProductsDetailsAsync(IEnumerable<Product> products);

    Task<IEnumerable<PingSoapInfo>> DetermineMagentoVersionAsync(Mark mark = null);

    Task<PingSoapInfo> DetermineMagentoVersionAndSetupServiceAsync(Mark mark = null);
  }
```
