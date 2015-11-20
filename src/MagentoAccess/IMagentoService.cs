﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetMagentoCoreInfo;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Models.GetProducts;
using MagentoAccess.Models.PingRest;
using MagentoAccess.Models.PutInventory;
using MagentoAccess.Services.Rest;

namespace MagentoAccess
{
	public interface IMagentoService
	{
		Task< IEnumerable< Order > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo );

		Task< IEnumerable< Order > > GetOrdersAsync();

		Task UpdateInventoryAsync( IEnumerable< Inventory > products );

		Task< IEnumerable< Product > > GetProductsSimpleAsync();

		Task< IEnumerable< Product > > GetProductsAsync();

		VerificationData RequestVerificationUri();

		void PopulateAccessTokenAndAccessTokenSecret( string verificationCode, string requestToken, string requestTokenSecret );

		MagentoService.SaveAccessToken AfterGettingToken { get; set; }

		TransmitVerificationCodeDelegate TransmitVerificationCode { get; set; }
		Func< string > AdditionalLogInfo { get; set; }

		Task< PingSoapInfo > PingSoapAsync( Mark mark = null );

		Task< PingRestInfo > PingRestAsync();
	}
}