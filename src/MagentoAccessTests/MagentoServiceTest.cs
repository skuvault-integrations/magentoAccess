using System;
using System.Collections.Generic;
using MagentoAccess;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.PutInventory;
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
			SoapApiKey,
			GetProductsThreadsLimit,
			SessionLifeTime,
			LogRawMessages
			);

		private IMagentoServiceLowLevelSoap _magentoServiceLowLevelSoapThrowExceptionsStub;

		private const string AccessToken = "accessToken";
		private const string AccessTokenSecret = "accessTokenSecret";
		private const string BaseMagentoUrl = "http://baseMagentoUrl.com";
		private const string ConsumerSckretKey = "consumerSckretKey";
		private const string ConsumerKey = "consumerKey";
		private const string SoapApiUser = "soapApiUser";
		private const string SoapApiKey = "soapApiKey";
		private const int GetProductsThreadsLimit = 4;
		private const int SessionLifeTime = 3590;
		private const bool LogRawMessages = true;

		[ TestFixtureSetUp ]
		public void Setup()
		{
			this._magentoServiceLowLevelSoapThrowExceptionsStub = new MagentoServiceLowLevelSoapVFrom17To19CeThrowExcetpionStub( SoapApiUser, SoapApiKey, BaseMagentoUrl, "0" );
		}

		[ Test ]
		public void GetOrdersAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert

			Assert.ThrowsAsync< MagentoCommonException >( async () => await magentoService.GetOrdersAsync( default(DateTime), default(DateTime) ).ConfigureAwait( false ) );
		}

		[ Test ]
		public void GetProductsAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.ThrowsAsync< MagentoCommonException >( async () => await magentoService.GetProductsAsync( new[] { 0, 1 } ).ConfigureAwait( false ) );
		}

		[ Test ]
		public void PingSoapAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.ThrowsAsync< MagentoCommonException >( async () => await magentoService.PingSoapAsync().ConfigureAwait( false ) );
		}


		[ Test ]
		public void UpdateInventoryAsync_ByDateExceptionOccured_ExceptionThrown()
		{
			//------------ Arrange
			var magentoService = new MagentoService( this._magentoAuthenticatedUserCredentials, null ) { MagentoServiceLowLevelSoap = this._magentoServiceLowLevelSoapThrowExceptionsStub };

			//------------ Act
			//------------ Assert
			Assert.ThrowsAsync< MagentoCommonException >( async () => await magentoService.UpdateInventoryAsync( new List< Inventory > { new Inventory() } ).ConfigureAwait( false ) );
		}
	}
}