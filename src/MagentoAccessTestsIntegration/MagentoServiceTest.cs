using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Models.Services.Credentials;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	internal class MagentoServiceTest : BaseTest
	{
		[ Test ]
		public void GetOrders_UserAlreadyHasAccessTokens_ReceiveOrders()
		{
			//------------ Arrange

			//------------ Act
			var firstCreatedItem = this._orders.OrderBy( x => x.updated_at.ToDateTimeOrDefault() ).First();
			var lastCreatedItem = this._orders.OrderBy( x => x.updated_at.ToDateTimeOrDefault() ).Last();

			var modifiedFrom = new DateTime( DateTime.Parse( firstCreatedItem.updated_at ).Ticks, DateTimeKind.Utc ).AddSeconds( 1 );
			var modifiedTo = new DateTime( DateTime.Parse( lastCreatedItem.updated_at ).Ticks, DateTimeKind.Utc ).AddSeconds( -1 );

			var getOrdersTask = this._magentoService.GetOrdersAsync( modifiedFrom, modifiedTo );
			getOrdersTask.Wait();

			//------------ Assert
			var thatMustBeReturned = this._orders.Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.increment_id ).ToList();
			var thatWasReturned = getOrdersTask.Result.Select( x => x.OrderIncrementalId ).ToList();

			thatWasReturned.Should().BeEquivalentTo( thatMustBeReturned );
		}

		[ Test ]
		public void GetProductsAsync_UserAlreadyHasAccessTokens_ReceiveProducts()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._magentoService.GetProductsAsync();
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNull().And.NotBeEmpty();
		}

		[ Test ]
		public void UpdateInventoryAsync_UserAlreadyHasAccessTokens_ReceiveProducts()
		{
			//------------ Arrange

			//------------ Act
			var getProductsTask = this._magentoService.GetProductsAsync();
			getProductsTask.Wait();

			var allProductsinMagent = getProductsTask.Result.ToList();
			var onlyProductsCreatedForThisTests = allProductsinMagent.Where( x => this._productsIds.ContainsKey( int.Parse( x.ProductId ) ) );

			var itemsToUpdate = onlyProductsCreatedForThisTests.Select( x => new Inventory() { ProductId = x.ProductId, ItemId = x.EntityId, Qty = 123 } );

			var updateInventoryTask = this._magentoService.UpdateInventoryAsync( itemsToUpdate );
			updateInventoryTask.Wait();

			/////

			var getProductsTask2 = this._magentoService.GetProductsAsync();
			getProductsTask2.Wait();

			var allProductsinMagent2 = getProductsTask2.Result.ToList();
			var onlyProductsCreatedForThisTests2 = allProductsinMagent2.Where( x => this._productsIds.ContainsKey( int.Parse( x.ProductId ) ) );

			var itemsToUpdate2 = onlyProductsCreatedForThisTests2.Select( x => new Inventory() { ProductId = x.ProductId, ItemId = x.EntityId, Qty = 100500 } );

			var updateInventoryTask2 = this._magentoService.UpdateInventoryAsync( itemsToUpdate2 );
			updateInventoryTask2.Wait();

			//------------ Assert
			var getProductsTask3 = this._magentoService.GetProductsAsync();
			getProductsTask3.Wait();

			var allProductsinMagent3 = getProductsTask3.Result.ToList();
			var onlyProductsCreatedForThisTests3 = allProductsinMagent3.Where( x => this._productsIds.ContainsKey( int.Parse( x.ProductId ) ) );

			onlyProductsCreatedForThisTests2.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == 123 );
			onlyProductsCreatedForThisTests3.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == 100500 );
		}

		[ Test ]
		public void PingSoapAsync_IncorrectApiKey_ThrowException()
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
					this._testData.GetMagentoConsumerCredentials().Key,
					this._testData.GetMagentoSoapUser().ApiUser,
					"incorrect ApiKey" ) );

				var magentoInfoAsyncTask = service.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void PingSoapAsync_IncorrectApiUser_ThrowException()
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
					this._testData.GetMagentoConsumerCredentials().Key,
					"incorrect ApiUser",
					this._testData.GetMagentoSoapUser().ApiKey ) );

				var magentoInfoAsyncTask = service.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void PingRestAsync_IncorrectAccessToken_ThrowException()
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

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void PingRestAsync_IncorrectAccessTokenSecret_ThrowException()
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

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void PingRestAsync_IncorrectBaseUrl_ThrowException()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = new MagentoService( new MagentoAuthenticatedUserCredentials(
					this._testData.GetMagentoAccessToken().AccessToken,
					this._testData.GetMagentoAccessToken().AccessTokenSecret,
					this._testData.GetMagentoUrls().MagentoBaseUrl + "IncorrectUrlPart",
					this._testData.GetMagentoConsumerCredentials().Secret,
					this._testData.GetMagentoConsumerCredentials().Key,
					this._testData.GetMagentoSoapUser().ApiUser,
					this._testData.GetMagentoSoapUser().ApiKey ) );

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void PingRestAsync_IncorrectConsumerSecret_ThrowException()
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

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void PingRestAsync_IncorrectConsumerKey_ThrowException()
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

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		public void PingRestAsync_CorrectConsumerKey_NotExceptionThrow()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var magentoInfoAsyncTask = this._magentoService.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldNotThrow< Exception >();
		}

		[ Test ]
		public void PingSoapAsync_CorrectCredentials_NoExceptionThrow()
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var magentoInfoAsyncTask = this._magentoService.PingSoapAsync();
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
			var verificationData = this._magentoServiceNotAuth.RequestVerificationUri();
			var requestToken = verificationData.RequestToken;
			var requestTokenSecret = verificationData.RequestTokenSecret;

			Process.Start( verificationData.Uri.AbsoluteUri );

			var verificationCode = string.Empty;

			this._magentoServiceNotAuth.PopulateAccessTokenAndAccessTokenSecret( verificationCode, requestToken, requestTokenSecret );

			//------------ Assert
			this._magentoServiceNotAuth.MagentoServiceLowLevel.AccessToken.Should().NotBeNullOrWhiteSpace();
			this._magentoServiceNotAuth.MagentoServiceLowLevel.AccessTokenSecret.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		[ Ignore ]
		//this test is not completed to use it - manually fill verificatio code in file
		public void InitiateDesktopAuthentication_UserHasNotGotAccessTokens_AuthCalled()
		{
			//------------ Arrange

			//------------ Act
			this._magentoServiceNotAuth.TransmitVerificationCode = this.transmitVerificationCode;
			this._magentoServiceNotAuth.InitiateDesktopAuthentication();
			//------------ Assert
			this._magentoServiceNotAuth.MagentoServiceLowLevel.AccessToken.Should().NotBeNullOrWhiteSpace();
			this._magentoServiceNotAuth.MagentoServiceLowLevel.AccessTokenSecret.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		[ Ignore ]
		public void RequestVerificationUri_UserHasNotGotAccessTokensURLCOntainsPort_AuthCalled()
		{
			//------------ Arrange

			//------------ Act
			//this._serviceNotAuth.TransmitVerificationCode = () => this._transmitVerification;
			var data = this._magentoServiceNotAuth.RequestVerificationUri();
			//------------ Assert
			data.Should().NotBeNull();
			data.RequestToken.Should().NotBeNullOrWhiteSpace();
			data.RequestTokenSecret.Should().NotBeNullOrWhiteSpace();
			data.Uri.Should().NotBeNull();
		}
	}
}