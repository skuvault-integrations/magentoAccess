using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.PutInventory;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration
{
	[ TestFixture ]
	internal class MagentoServiceTest : BaseTest
	{
		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void DetermineMagentoVersionAsync_CorrectCredentials_ReceiveStoreVersion( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion );
			//------------ Act

			var getOrdersTask = magentoService.DetermineMagentoVersionAsync();
			getOrdersTask.Wait();

			//------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Should().NotBeNull();
			pingSoapInfo.Any( x => x.SoapWorks && string.Compare( x.Version, credentials.MagentoVersion, StringComparison.CurrentCultureIgnoreCase ) == 0 ).Should().BeTrue();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void DetermineMagentoVersionAsync_InCorrectApiKey_ReceiveNull( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey+"1", "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion );
			//------------ Act

			var getOrdersTask = magentoService.DetermineMagentoVersionAsync();
			getOrdersTask.Wait();

			//------------ Assert
			var pingSoapInfo = getOrdersTask.Result;

			pingSoapInfo.Any(x => x.SoapWorks && string.Compare(x.Version, credentials.MagentoVersion, StringComparison.CurrentCultureIgnoreCase) == 0).Should().BeFalse();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetOrders_UserAlreadyHasAccessTokens_ReceiveOrders( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion );
			//------------ Act
			//var firstCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).First();
			//var lastCreatedItem = this._orders[ credentials.StoreUrl ].OrderBy( x => x.UpdatedAt ).Last();

			//var modifiedFrom = new DateTime( ( firstCreatedItem.UpdatedAt ).Ticks, DateTimeKind.Utc ).AddSeconds( 1 );
			//var modifiedTo = new DateTime( ( lastCreatedItem.UpdatedAt ).Ticks, DateTimeKind.Utc ).AddSeconds( -1 );
			var modifiedFrom = new DateTime( 2016, 2, 28, 23, 23, 59 ).AddSeconds( 1 );
			var modifiedTo = new DateTime( 2016, 4, 2, 23, 30, 39 ).AddSeconds( -1 );

			var getOrdersTask = magentoService.GetOrdersAsync( modifiedFrom, modifiedTo );
			getOrdersTask.Wait();

			//------------ Assert
			//var thatMustBeReturned = this._orders[ credentials.StoreUrl ].Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.OrderIncrementalId ).ToList();
			//var thatWasReturned = getOrdersTask.Result.Where( x => x != firstCreatedItem && x != lastCreatedItem ).Select( x => x.OrderIncrementalId ).ToList();
			var thatWasReturned = getOrdersTask.Result.Select( x => x.OrderIncrementalId ).ToList();

			//thatWasReturned.Should().BeEquivalentTo( thatMustBeReturned );
			thatWasReturned.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void GetProductsAsync_UserAlreadyHasAccessTokens_ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService( credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion );

			//------------ Act
			var getProductsTask = magentoService.GetProductsAsync(true);
			getProductsTask.Wait();

			//------------ Assert
			getProductsTask.Result.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void UpdateInventoryAsync_UserAlreadyHasAccessTokens_ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService(credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion);

			//------------ Act
			var onlyProductsCreatedForThisTests = this.GetOnlyProductsCreatedForThisTests(  magentoService );
			var updateFirstTimeQty = 123;
			var updateInventoryTask = magentoService.UpdateInventoryAsync( onlyProductsCreatedForThisTests.ToInventory( x => updateFirstTimeQty ).ToList() );
			updateInventoryTask.Wait();

			/////
			var onlyProductsCreatedForThisTests2 = this.GetOnlyProductsCreatedForThisTests( magentoService);

			var updateSecondTimeQty = 100500;
			var updateInventoryTask2 = magentoService.UpdateInventoryAsync( onlyProductsCreatedForThisTests2.ToInventory( x => updateSecondTimeQty ).ToList() );
			updateInventoryTask2.Wait();

			//------------ Assert
			var onlyProductsCreatedForThisTests3 = this.GetOnlyProductsCreatedForThisTests( magentoService);

			onlyProductsCreatedForThisTests2.Should().NotBeNullOrEmpty();
			onlyProductsCreatedForThisTests2.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == updateFirstTimeQty );
			onlyProductsCreatedForThisTests3.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == updateSecondTimeQty );
		}

		[ Ignore ]
		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void UpdateInventoryBYSkuAsync_UserAlreadyHasAccessTokens_ReceiveProducts( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService(credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion);

			//------------ Act
			var getProductsTask = magentoService.GetProductsAsync();
			getProductsTask.Wait();

			var allProductsinMagent = getProductsTask.Result.ToList();
			var onlyProductsCreatedForThisTests = allProductsinMagent.Where( x => this._productsIds[ credentials.StoreUrl ].ContainsKey( int.Parse( x.ProductId ) ) );

			var itemsToUpdate = onlyProductsCreatedForThisTests.Select( x => new InventoryBySku() { Sku = x.Sku, Qty = 123 } );

			var updateInventoryTask = magentoService.UpdateInventoryBySkuAsync( itemsToUpdate );
			updateInventoryTask.Wait();

			/////

			var getProductsTask2 = magentoService.GetProductsAsync();
			getProductsTask2.Wait();

			var allProductsinMagent2 = getProductsTask2.Result.ToList();
			var onlyProductsCreatedForThisTests2 = allProductsinMagent2.Where( x => this._productsIds[ credentials.StoreUrl ].ContainsKey( int.Parse( x.ProductId ) ) );

			var itemsToUpdate2 = onlyProductsCreatedForThisTests2.Select( x => new InventoryBySku() { Sku = x.Sku, Qty = 100500 } );

			var updateInventoryTask2 = magentoService.UpdateInventoryBySkuAsync( itemsToUpdate2 );
			updateInventoryTask2.Wait();

			//------------ Assert
			var getProductsTask3 = magentoService.GetProductsAsync();
			getProductsTask3.Wait();

			var allProductsinMagent3 = getProductsTask3.Result.ToList();
			var onlyProductsCreatedForThisTests3 = allProductsinMagent3.Where( x => this._productsIds[ credentials.StoreUrl ].ContainsKey( int.Parse( x.ProductId ) ) );

			onlyProductsCreatedForThisTests2.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == 123 );
			onlyProductsCreatedForThisTests3.Should().OnlyContain( x => x.Qty.ToDecimalOrDefault() == 100500 );
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void PingSoapAsync_IncorrectApiKey_ThrowException( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = this.CreateMagentoService(credentials.SoapApiUser, "incorrectKey", "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion);

				var magentoInfoAsyncTask = service.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void PingSoapAsync_IncorrectApiUser_ThrowException( MagentoServiceSoapCredentials credentials )
		{
			// can be red for magento 2.0 since user doesn't used in magento2.0 version
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = this.CreateMagentoService("incorrectuser", credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion);

				var magentoInfoAsyncTask = service.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void PingSoapAsync_IncorrectUrl_ThrowException( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange

			//------------ Act
			Action act = () =>
			{
				var service = this.CreateMagentoService( "incorrectuser", credentials.SoapApiKey, "null", "null", "null", "null", "http://w.com", "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion );

				var magentoInfoAsyncTask = service.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		public void PingSoapAsync_CorrectCredentials_NoExceptionThrow( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService(credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion);
			//------------ Act
			Action act = () =>
			{
				var magentoInfoAsyncTask = magentoService.PingSoapAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldNotThrow< Exception >();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		[ Ignore ]
		public void PopulateAccessTokenAndAccessTokenSecret_UserHasNotGotAccessTokens_AuthCalled( MagentoServiceSoapCredentials credentials )
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
			this._magentoServiceNotAuth.MagentoServiceLowLevelRest.AccessToken.Should().NotBeNullOrWhiteSpace();
			this._magentoServiceNotAuth.MagentoServiceLowLevelRest.AccessTokenSecret.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		[ Ignore ]
		//this test is not completed to use it - manually fill verificatio code in file
		public void InitiateDesktopAuthentication_UserHasNotGotAccessTokens_AuthCalled()
		{
			//------------ Arrange

			//------------ Act
			this._magentoServiceNotAuth.TransmitVerificationCode = this.transmitVerificationCode;
			this._magentoServiceNotAuth.InitiateDesktopAuthentication();
			//------------ Assert
			this._magentoServiceNotAuth.MagentoServiceLowLevelRest.AccessToken.Should().NotBeNullOrWhiteSpace();
			this._magentoServiceNotAuth.MagentoServiceLowLevelRest.AccessTokenSecret.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
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

		#region Rest
		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		[ Ignore( "Since rest is a vestigie" ) ]
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
					this._testData.GetMagentoSoapUser().ApiKey ), null );

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		[ Ignore( "Since rest is a vestigie" ) ]
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
					this._testData.GetMagentoSoapUser().ApiKey ), null );

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		[ Ignore( "Since rest is a vestigie" ) ]
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
					this._testData.GetMagentoSoapUser().ApiKey ), null );

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		[ Ignore( "Since rest is a vestigie" ) ]
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
					this._testData.GetMagentoSoapUser().ApiKey ), null );

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		[ Ignore( "Since rest is a vestigie" ) ]
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
					this._testData.GetMagentoSoapUser().ApiKey ), null );

				var magentoInfoAsyncTask = service.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldThrow< Exception >();
		}

		[ Test ]
		[ TestCaseSource( "GetTestStoresCredentials" ) ]
		[ Ignore( "Since rest is a vestigie" ) ]
		public void PingRestAsync_CorrectConsumerKey_NotExceptionThrow( MagentoServiceSoapCredentials credentials )
		{
			//------------ Arrange
			var magentoService = this.CreateMagentoService(credentials.SoapApiUser, credentials.SoapApiKey, "null", "null", "null", "null", credentials.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentials.MagentoVersion);

			//------------ Act
			Action act = () =>
			{
				var magentoInfoAsyncTask = magentoService.PingRestAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert
			act.ShouldNotThrow< Exception >();
		}
		#endregion Rest
	}
}