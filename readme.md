MagentoAccess is .NET wrapper around [Magento API](http://www.magentocommerce.com/api/rest/introduction.html) was created to simplify and speed up working with it.

# Supported versions
MagentoAccess tested with these Magento versions. 


**Community Edition**
* 1.9.0.1
* 1.9.1.0
* 1.7.0.2
* 1.8.1.0
* 1.9.2.0
* 1.9.2.2
* 2.0.2
* 2.0.7
* 2.1.0 (may not work, development in process)

**Enterprise Edition**
* 1.14.1.0


# Getting Started

Install the [NuGet package](https://www.nuget.org/packages/MagentoAccess).

**Create service and get products from Magento**

For most of magento versions ```SoapApiUser```, ```SoapApiKey```, ```StoreUrl``` will be enough (set other parameters as empty strings). MagentoAccess interacts with store through SOAP (adding support for REST is in process), WS-I compliance should be turned on.
```C#
			var servicesFactory = new MagentoFactory();
			var magentoService = servicesFactory.CreateService(new MagentoAuthenticatedUserCredentials("AccessToken", "AccessTokenSecret", "StoreUrl", "ConsumerSecret", "ConsumerKey", "SoapApiUser", "SoapApiKey"), new MagentoConfig() { EditionByDefault = "ce", VersionByDefault = "2.0.2.0" });
			
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
