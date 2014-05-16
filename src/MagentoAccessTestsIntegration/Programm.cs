using System;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Models.Credentials;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	public class Programm
	{
		[ Test ]
		public void GetOrders()
		{
			//------------ Arrange

			//var fact = new MagentoFactory(new MagentoAuthenticatedUserCredentials(accessToken.AccessToken, accessToken.AccessTokenSecret, magentoUrls.MagentoBaseUrl, consumer.Key, consumer.Secret));
			//var MagentoService = fact.CreateService();

			////------------ Act
			//var orders = MagentoService.GetOrders(DateTime.Now, DateTime.Now);

			////------------ Assert
			//orders.Should().NotBeEmpty();
		}
	}
}