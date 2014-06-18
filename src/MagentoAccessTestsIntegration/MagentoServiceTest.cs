using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using MagentoAccess.Models.PutInventory;
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
		public void GetMagentoInfoAsync_StoreDoesNotContainsUser_ExceptionThrowns()
		{
			//------------ Arrange

			//------------ Act

			Action act = () =>
			{
				this._service.MagentoServiceLowLevelSoap.ApiKey = "incorrect key";
				this._service.MagentoServiceLowLevelSoap.ApiUser = "incorrect user";
				var magentoInfoAsyncTask = this._service.GetMagentoInfoAsync();
				magentoInfoAsyncTask.Wait();
			};

			//------------ Assert

			act.ShouldThrow< Exception >();
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

		[ Test ]
		[ Ignore ]
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