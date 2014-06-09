using System;
using System.ServiceModel;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Services
{
	internal class MagentoServiceLowLevelSoap
	{
		private readonly Mage_Api_Model_Server_Wsi_HandlerPortTypeClient _magentoSoapService;

		public MagentoServiceLowLevelSoap()
		{
			this._magentoSoapService = new Mage_Api_Model_Server_Wsi_HandlerPortTypeClient(new BasicHttpBinding(), new EndpointAddress("http://192.168.0.103/magento/index.php/api/v2_soap/index/"));
		}

		public async Task<object> GetOrders()
		{
			var session = await _magentoSoapService.loginAsync("MaxKits", "123456").ConfigureAwait(false);

			var res = await _magentoSoapService.salesOrderListAsync(session.result, new filters()).ConfigureAwait(false);

			return res;
		}

		public async Task<object> GetOrders(DateTime modifiedTime)
		{
			var session = await _magentoSoapService.loginAsync("MaxKits", "123456").ConfigureAwait(false);

			var filters = new filters();
			//filters.filter = new associativeEntity[1];
			//filters.filter[0] = new associativeEntity() { key = "created_at", value = "2014-05-01 10:10:00" };


			filters.complex_filter = new complexFilter[2];
			filters.complex_filter[0] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "from", value = "2014-05-08 15:02:58" } };
			filters.complex_filter[1] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "to", value = "2014-05-28 10:48:52" } };


			var res = await _magentoSoapService.salesOrderListAsync(session.result, filters).ConfigureAwait(false);

			return res;
		}
	}
}