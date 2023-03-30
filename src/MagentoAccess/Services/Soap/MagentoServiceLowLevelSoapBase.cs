using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MagentoAccess.Services.Soap
{
	/// <summary>
	/// Base class for Magento low-level SOAP services (for versions from 1.14.1.0 to 1.9.2.1)
	/// </summary>
	internal class MagentoServiceLowLevelSoapBase
	{
		public string ApiUser { get; }
		public string ApiKey { get; }
		public string Store { get; }
		protected string BaseMagentoUrl { get; }
		
		public static readonly string DefaultSoapApiUrl = "index.php/api/v2_soap/index/";
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

		protected MagentoServiceLowLevelSoapBase( string apiUser, string apiKey, string baseMagentoUrl, 
			string store, int getProductsMaxThreads, bool logRawMessages, int sessionIdLifeTime )
		{
			this.ApiUser = apiUser;
			this.ApiKey = apiKey;
			this.Store = store;
			this.BaseMagentoUrl = baseMagentoUrl;
			this.LogRawMessages = logRawMessages;
			this._getProductsMaxThreads = getProductsMaxThreads;
			this._sessionIdLifeTime = sessionIdLifeTime;
			this._getSessionIdSemaphore = new SemaphoreSlim(1, 1);
		}

		protected static string GetRelativeUrl( bool useRedirect ) 
		{
			return !useRedirect ? DefaultSoapApiUrl : DefaultSoapApiUrl.Replace( "index.php/", "" );
		}

		public Task< bool > InitAsync( bool suppressExceptions = false )
		{
			return Task.FromResult( true );
		}
	}
}
