using System;
using System.Collections.Generic;
using System.Linq;
using MagentoAccess.Models.Credentials;
using MagentoAccess.Models.GetOrders;
using MagentoAccess.Services;

namespace MagentoAccess
{
	public class MagentoService : IMagentoService
	{
		public virtual IMagentoServiceLowLevel MagentoServiceLowLevel {get;set;}

		public MagentoService(MagentoAuthenticatedUserCredentials magentoAuthenticatedUserCredentials) {
			MagentoServiceLowLevel = new MagentoServiceLowLevel(
				magentoAuthenticatedUserCredentials.ConsumerKey,
				magentoAuthenticatedUserCredentials.ConsumerSckretKey,
				magentoAuthenticatedUserCredentials.BaseMagentoUrl,
				magentoAuthenticatedUserCredentials.AccessToken,
				magentoAuthenticatedUserCredentials.AccessTokenSecret
				);
		}

		public MagentoService(MagentoNonAuthenticatedUserCredentials magentoUserCredentials)
		{
			MagentoServiceLowLevel = new MagentoServiceLowLevel(
				magentoUserCredentials.ConsumerKey,
				magentoUserCredentials.ConsumerSckretKey,
				magentoUserCredentials.BaseMagentoUrl,
				magentoUserCredentials.RequestTokenUrl,
				magentoUserCredentials.AuthorizeUrl,
				magentoUserCredentials.AccessTokenUrl
				);
		}

		public IEnumerable< Order > GetOrders( DateTime dateFrom, DateTime dateTo )
		{
			if (string.IsNullOrWhiteSpace(MagentoServiceLowLevel.AccessToken))
			{
				var authorizeTask = MagentoServiceLowLevel.GetAccessToken();
				authorizeTask.Wait();
				//testData.CreateAccessTokenFile(service.AccessToken, service.AccessTokenSecret);
			}

			//todo: filter by date
			var res = MagentoServiceLowLevel.GetOrders();
			return res.Orders;
		}
	}
}