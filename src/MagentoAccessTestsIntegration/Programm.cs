using FluentAssertions;
using MagentoAccess;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	public class Programm
	{
		[ Test ]
		public void SimpleCass()
		{
			//------------ Arrange
			var webser = new WebRequestServices();
			var service = new MagentoServiceLowLevel(webser, "http://192.168.0.104/magento/api/rest/products");

			//------------ Act
			//var resTask = service.GetItemsAsync();
			//resTask.Wait();

			//------------ Assert
			//resTask.Result.Should().NotBeEmpty();
		}

		[Test]
		public void SimpleCass2()
		{
			//------------ Arrange
			var webser = new WebRequestServices();
			var service = new MagentoServiceLowLevel(webser, "http://192.168.0.104/magento/api/rest/products");

			//------------ Act
			var resTask = service.GetItems();
			//resTask.Wait();

			//------------ Assert
			//resTask.Result.Should().NotBeEmpty();
			resTask.Should().NotBeEmpty();
		}
	}
}