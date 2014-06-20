using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.Credentials;
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

			var modifiedFrom = DateTime.Parse( "2014-05-08 15:02:58" );
			var modifiedTo = DateTime.Parse( "2014-06-28 10:48:52" );
			var getOrdersTask = this._service.GetOrdersAsync( modifiedFrom, modifiedTo );
			getOrdersTask.Wait();

			//------------ Assert
			getOrdersTask.Result.Should().NotBeNull().And.NotBeEmpty();
		}

		[ Test ]
		public void GetProductsAsync_UserAlreadyHasAccessTokens_ReceiveProducts()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._service.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull().And.NotBeEmpty();
		}

		[ Test ]
		public void UpdateInventoryAsync_UserAlreadyHasAccessTokens_ReceiveProducts()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._service.GetProductsAsync();
			getProductsTask.Wait();

			var itemsToUpdate = getProductsTask.Result.Select( x => new Inventory() { ProductId = x.ProductId, ItemId = x.EntityId, Qty = long.Parse( "3" + x.EntityId.Last().ToString() ) } );
			var updateInventoryTask = this._service.UpdateInventoryAsync( itemsToUpdate );
			updateInventoryTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull().And.NotBeEmpty();
		}

		[ Test ]
		public void GetMagentoInfoAsync_IncorrectApiKey_ExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = new MagentoService(new MagentoAuthenticatedUserCredentials(
					this._testData.GetMagentoAccessToken().AccessToken,
					this._testData.GetMagentoAccessToken().AccessTokenSecret,
					this._testData.GetMagentoUrls().MagentoBaseUrl,
					this._testData.GetMagentoConsumerCredentials().Secret,
					this._testData.GetMagentoConsumerCredentials().Key,
					this._testData.GetMagentoSoapUser().ApiUser,
					"incorrect ApiKey"));

				var magentoInfoAsyncTask = service.GetMagentoInfoAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void GetMagentoInfoAsync_IncorrectApiUser_ExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = new MagentoService(new MagentoAuthenticatedUserCredentials(
					this._testData.GetMagentoAccessToken().AccessToken,
					this._testData.GetMagentoAccessToken().AccessTokenSecret,
					this._testData.GetMagentoUrls().MagentoBaseUrl,
					this._testData.GetMagentoConsumerCredentials().Secret,
					this._testData.GetMagentoConsumerCredentials().Key,
					"incorrect ApiUser",
					this._testData.GetMagentoSoapUser().ApiKey));

				var magentoInfoAsyncTask = service.GetMagentoInfoAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void GetMagentoInfoAsync_IncorrectAccessToken_ExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = new MagentoService( new MagentoAuthenticatedUserCredentials(
					"incorrect access token ",
					this._testData.GetMagentoAccessToken().AccessTokenSecret,
					this._testData.GetMagentoUrls().MagentoBaseUrl,
					this._testData.GetMagentoConsumerCredentials().Secret,
					this._testData.GetMagentoConsumerCredentials().Key,
					this._testData.GetMagentoSoapUser().ApiUser,
					this._testData.GetMagentoSoapUser().ApiKey ) );

				var magentoInfoAsyncTask = service.GetMagentoInfoAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void GetMagentoInfoAsync_IncorrectAccessTokenSecret_ExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = new MagentoService( new MagentoAuthenticatedUserCredentials(
					this._testData.GetMagentoAccessToken().AccessToken,
					"incorrect access token secret",
					this._testData.GetMagentoUrls().MagentoBaseUrl,
					this._testData.GetMagentoConsumerCredentials().Secret,
					this._testData.GetMagentoConsumerCredentials().Key,
					this._testData.GetMagentoSoapUser().ApiUser,
					this._testData.GetMagentoSoapUser().ApiKey ) );

				var magentoInfoAsyncTask = service.GetMagentoInfoAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void GetMagentoInfoAsync_IncorrectBaseUrl_ExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = new MagentoService( new MagentoAuthenticatedUserCredentials(
					this._testData.GetMagentoAccessToken().AccessToken,
					this._testData.GetMagentoAccessToken().AccessTokenSecret,
					"http://199.48.164.39/incorrectUrl",
					this._testData.GetMagentoConsumerCredentials().Secret,
					this._testData.GetMagentoConsumerCredentials().Key,
					this._testData.GetMagentoSoapUser().ApiUser,
					this._testData.GetMagentoSoapUser().ApiKey ) );

				var magentoInfoAsyncTask = service.GetMagentoInfoAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void GetMagentoInfoAsync_IncorrectConsumerSecret_ExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = new MagentoService( new MagentoAuthenticatedUserCredentials(
					this._testData.GetMagentoAccessToken().AccessToken,
					this._testData.GetMagentoAccessToken().AccessTokenSecret,
					this._testData.GetMagentoUrls().MagentoBaseUrl,
					"incorrect consumer secret",
					this._testData.GetMagentoConsumerCredentials().Key,
					this._testData.GetMagentoSoapUser().ApiUser,
					this._testData.GetMagentoSoapUser().ApiKey ) );

				var magentoInfoAsyncTask = service.GetMagentoInfoAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void GetMagentoInfoAsync_IncorrectConsumerKey_ExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = new MagentoService( new MagentoAuthenticatedUserCredentials(
					this._testData.GetMagentoAccessToken().AccessToken,
					this._testData.GetMagentoAccessToken().AccessTokenSecret,
					this._testData.GetMagentoUrls().MagentoBaseUrl,
					this._testData.GetMagentoConsumerCredentials().Secret,
					"incorrect consumer key",
					this._testData.GetMagentoSoapUser().ApiUser,
					this._testData.GetMagentoSoapUser().ApiKey ) );

				var magentoInfoAsyncTask = service.GetMagentoInfoAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void GetMagentoInfoAsync_CorrectCredentials_NoExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var magentoInfoAsyncTask = this._service.GetMagentoInfoAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldNotThrow< Exception >();
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
		//this test is not completed to use it - manually fill verificatio code in file
		public void InitiateDesktopAuthentication_UserHasNotGotAccessTokens_AuthCalled()
		{
			//------------ Arrange

			//------------ Act
			this._serviceNotAuth.TransmitVerificationCode = this.transmitVerificationCode;
			this._serviceNotAuth.InitiateDesktopAuthentication();
			//------------ Assert
			this._serviceNotAuth.MagentoServiceLowLevel.AccessToken.Should().NotBeNullOrWhiteSpace();
			this._serviceNotAuth.MagentoServiceLowLevel.AccessTokenSecret.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		[ Ignore ]
		public void RequestVerificationUri_UserHasNotGotAccessTokensURLCOntainsPort_AuthCalled()
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