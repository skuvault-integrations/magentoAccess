using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MagentoAccess.Services.Soap
{
	internal class MagentoServiceLowLevelSoapBase
	{
		public string ApiUser { get; }
		public string ApiKey { get; }
		public string Store { get; }
		protected string BaseMagentoUrl { get; }
		protected const string DefaultSoapApiUrl = "index.php/api/v2_soap/index/"; 
		public string DefaultApiUrl => DefaultSoapApiUrl;

		/// <summary>
		/// RelativeUrl could be used instead of the DefaultApiUrl 
		/// in case a marketplace website has a redirect policy (to ignoring of index.php for example)
		/// please see details in the GUARD-2824
		/// </summary>
		public string RelativeUrl { get; }
		public string StoreVersion { get; set; }
		public bool LogRawMessages { get; }
		[JsonIgnore]
		[IgnoreDataMember]
		public Func<Task<Tuple<string, DateTime>>> PullSessionId { get; set; }
		internal IMagento1XxxHelper Magento1xxxHelper { get; set; }
		protected string _sessionId;
		protected DateTime _sessionIdCreatedAt;
		protected readonly SemaphoreSlim _getSessionIdSemaphore;
		protected readonly int _getProductsMaxThreads;
		protected readonly int _sessionIdLifeTime;
		public virtual bool GetStockItemsWithoutSkuImplementedWithPages => false;
		public bool GetOrderByIdForFullInformation => true;
		public bool GetOrdersUsesEntityInsteadOfIncrementId => false;

		protected MagentoServiceLowLevelSoapBase( string apiUser, string apiKey, string baseMagentoUrl, string relativeUrl, 
			string store, int getProductsMaxThreads, bool logRawMessages, int sessionIdLifeTime )
		{
			this.ApiUser = apiUser;
			this.ApiKey = apiKey;
			this.Store = store;
			this.BaseMagentoUrl = baseMagentoUrl;
			this.RelativeUrl = relativeUrl;
			this.LogRawMessages = logRawMessages;
			this._getProductsMaxThreads = getProductsMaxThreads;
			this._sessionIdLifeTime = sessionIdLifeTime;
			this._getSessionIdSemaphore = new SemaphoreSlim(1, 1);
		}
	}
}
