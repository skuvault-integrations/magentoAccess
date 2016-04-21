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
* 2.0.2.0 (may not work, development in process)

**Enterprise Edition**
* 1.14.1.0


# Getting Started

Install the [NuGet package](https://www.nuget.org/packages/MagentoAccess).

**Create service and get products from Magento**

For most of magento versions ```SoapApiUser```, ```SoapApiKey```, ```StoreUrl``` will be enough (set other parameters as empty strings)
```C#
			var servicesFactory = new MagentoFactory();
			var magentoService = servicesFactory.CreateService(new MagentoAuthenticatedUserCredentials("AccessToken", "AccessTokenSecret", "StoreUrl", "ConsumerSecret", "ConsumerKey", "SoapApiUser", "SoapApiKey"), new MagentoConfig() { EditionByDefault = "ce", VersionByDefault = "2.0.2.0" });
			// call only if you are not sure about your magento store version
			var magentoVersion = await magentoService.DetermineMagentoVersionAndSetupServiceAsync();

			var magentoInventory = await magentoService.GetProductsAsync();
```

