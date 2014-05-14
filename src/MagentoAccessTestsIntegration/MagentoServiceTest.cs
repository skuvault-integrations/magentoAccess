using System;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Models.Credentials;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[TestFixture]
	public class MagentoServiceTest
	{
		[Test]
		public void GetOrders_UserAlreadyHasAccessTokens_GetsOrders()
		{
			//------------ Arrange
			var testData = new TestData(@"..\..\Files\magento_ConsumerKey.csv", @"..\..\Files\magento_AuthorizeEndPoints.csv", @"..\..\Files\magento_AccessToken.csv");
			var consumer = testData.GetMagentoConsumerCredentials();
			var authorityUrls = testData.GetMagentoUrls();
			var accessToken = testData.GetMagentoAccessToken();
			MagentoService service;

			service = new MagentoService(new MagentoAuthenticatedUserCredentials()
			{
				AccessToken = accessToken.AccessToken,
				AccessTokenSecret = accessToken.AccessTokenSecret,
				BaseMagentoUrl = authorityUrls.MagentoBaseUrl,
				ConsumerKey = consumer.Key,
				ConsumerSckretKey = consumer.Secret
			});

			//------------ Act
			var res = service.GetOrders(DateTime.Now.AddMonths(-3), DateTime.Now);

			//------------ Assert
			res.Should().NotBeNull().And.NotBeEmpty();
		} 
	}
}