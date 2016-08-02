using System;
using System.Collections.Generic;
using MagentoAccess;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Services.Rest;
using MagentoAccess.Services.Soap;
using MagentoAccessTests.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTests
{
	[ TestFixture ]
	internal class MagentoServiceTest
	{
		private readonly MagentoAuthenticatedUserCredentials _magentoAuthenticatedUserCredentials = new MagentoAuthenticatedUserCredentials(
			AccessToken,
			AccessTokenSecret,
			BaseMagentoUrl,
			ConsumerSckretKey,
			ConsumerKey,
			SoapApiUser,
			SoapApiKey
			);

		private IMagentoServiceLowLevelSoap _magentoServiceLowLevelSoapThrowExceptionsStub;
		private IMagentoServiceLowLevelRest _magentoServiceLowLevelRestThrowExceptionStub;

		private const string AccessToken = "accessToken";
		private const string AccessTokenSecret = "accessTokenSecret";
		private const string BaseMagentoUrl = "http://baseMagentoUrl.com";
		private const string ConsumerSckretKey = "consumerSckretKey";
		private const string ConsumerKey = "consumerKey";
		private const string SoapApiUser = "soapApiUser";
		private const string SoapApiKey = "soapApiKey";

		[ TestFixtureSetUp ]
		public void Setup()
		{
			this._magentoServiceLowLevelSoapThrowExceptionsStub = new MagentoServiceLowLevelSoapVFrom17To19CeThrowExcetpionStub( SoapApiUser, SoapApiKey, BaseMagentoUrl, "0" );
			this._magentoServiceLowLevelRestThrowExceptionStub = new MagentoServiceLowLevelRestRestStubThrowExceptionStub( AccessToken, ConsumerSckretKey, BaseMagentoUrl, AccessToken, AccessTokenSecret );
		}

		[ Test ]
		public void InitiateDesktopAuthentication_ExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub, MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.Throws< MagentoCommonException >( () => magentoService.InitiateDesktopAuthentication() );
		}

		[ Test ]
		public void PopulateAccessTokenAndAccessTokenSecret_ExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub, MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.Throws< MagentoCommonException >( () => magentoService.PopulateAccessTokenAndAccessTokenSecret( "qwe", "qwe", "qwe" ) );
		}

		[ Test ]
		public void GetOrdersAsync_ExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null )
			{
				MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub,
				MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub
			};

			Assert.Throws< MagentoCommonException >( async () => await magentoService.GetOrdersAsync().ConfigureAwait( false ) );
		}

		[ Test ]
		public void GetOrdersAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub, MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.Throws< MagentoCommonException >( async () => await magentoService.GetOrdersAsync( default(DateTime), default(DateTime) ).ConfigureAwait( false ) );
		}

		[ Test ]
		public void GetProductsAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub, MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.Throws< MagentoCommonException >( async () => await magentoService.GetProductsAsync(  ).ConfigureAwait( false ) );
		}

		[ Test ]
		public void GetProductsSimpleAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub, MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.Throws< MagentoCommonException >( async () => await magentoService.GetProductsSimpleAsync().ConfigureAwait( false ) );
		}

		[ Test ]
		public void PingRestAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub, MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.Throws< MagentoCommonException >( async () => await magentoService.PingRestAsync().ConfigureAwait( false ) );
		}

		[ Test ]
		public void PingSoapAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub, MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.Throws< MagentoCommonException >( async () => await magentoService.PingSoapAsync().ConfigureAwait( false ) );
		}

		[ Test ]
		public void RequestVerificationUri_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub, MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.Throws< MagentoCommonException >( () => magentoService.RequestVerificationUri() );
		}

		[ Test ]
		public void UpdateInventoryAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelRest = this._magentoServiceLowLevelRestThrowExceptionStub, MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.Throws< MagentoCommonException >( async () => await magentoService.UpdateInventoryAsync( new List< Inventory > { new Inventory() } ).ConfigureAwait( false ) );
		}
	}
}