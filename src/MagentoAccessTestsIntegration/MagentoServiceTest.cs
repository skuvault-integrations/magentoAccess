using System;
using System.Diagnostics;
using FluentAssertions;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	public class MagentoServiceTest : BaseTest
	{
		[ Test ]
		public void GetOrders_UserAlreadyHasAccessTokens_ReceiveOrders()
		{
			//------------ Arrange

			//------------ Act
			var getOrdersTask = this._service.GetOrdersAsync( DateTime.Now.AddMonths( -3 ), DateTime.Now );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.Should().NotBeNull().And.NotBeEmpty();
		}

		[ Test ]
		public void GetProducts_UserAlreadyHasAccessTokens_ReceiveProducts()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._service.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull().And.NotBeEmpty();
		}

		[ Test ]
		[ Ignore ]
		public void GetProducts_UserHasNotGotAccessTokens_AuthCalled()
		{
			//------------ Arrange

			//------------ Act
			var verificationData = this._serviceNotAuth.RequestVerificationUri();
			var requestToken = verificationData.RequestToken;
			var requestTokenSecret = verificationData.RequestTokenSecret;

			Process.Start( verificationData.Uri.AbsoluteUri );

			var verificationCode = string.Empty;

			this._serviceNotAuth.PopulateAccessTokenAndAccessTokenSecret( verificationCode, requestToken, requestTokenSecret );

			//------------ Assert
			this._serviceNotAuth.MagentoServiceLowLevel.AccessToken.Should().NotBeNullOrWhiteSpace();
			this._serviceNotAuth.MagentoServiceLowLevel.AccessTokenSecret.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		[ Ignore ]
		public void Authorize_UserHasNotGotAccessTokens_AuthCalled()
		{
			//------------ Arrange

			//------------ Act
			this._serviceNotAuth.TransmitVerificationCode = () => this._transmitVerification;
			this._serviceNotAuth.InitiateDesktopAuthentication();
			//------------ Assert
			this._serviceNotAuth.MagentoServiceLowLevel.AccessToken.Should().NotBeNullOrWhiteSpace();
			this._serviceNotAuth.MagentoServiceLowLevel.AccessTokenSecret.Should().NotBeNullOrWhiteSpace();
		}


		[Test]
		[Ignore]
		public void GetVerificationURI_UserHasNotGotAccessTokensURLCOntainsPort_AuthCalled()
		{
			//------------ Arrange

			//------------ Act
			//this._serviceNotAuth.TransmitVerificationCode = () => this._transmitVerification;
			var data = this._serviceNotAuth.RequestVerificationUri();
			//------------ Assert
			data.Should().NotBeNull();
			data.RequestToken.Should().NotBeNullOrWhiteSpace();
			data.RequestTokenSecret.Should().NotBeNullOrWhiteSpace();
			data.Uri.Should().NotBeNull();
		}
	}
}