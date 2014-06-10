using System;
using System.ServiceModel;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;

namespace MagentoAccess.Services
{
	internal class MagentoServiceLowLevelSoap
	{
		private readonly Mage_Api_Model_Server_Wsi_HandlerPortTypeClient _magentoSoapService;

		protected string UserName { get; set; }

		protected string Password { get; set; }

		public MagentoServiceLowLevelSoap( string userName, string password, string endpointAddress )
		{
			this.UserName = userName;
			this.Password = password;
			this._magentoSoapService = new Mage_Api_Model_Server_Wsi_HandlerPortTypeClient( new BasicHttpBinding(), new EndpointAddress( endpointAddress ) );
		}

		public async Task< object > GetOrders( DateTime modifiedFrom, DateTime modifiedTo )
		{
			var session = await this._magentoSoapService.loginAsync( this.UserName, this.Password ).ConfigureAwait( false );

			var filters = new filters { complex_filter = new complexFilter[ 2 ] };
			filters.complex_filter[ 0 ] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "from", value = modifiedFrom.ToString() } };
			filters.complex_filter[ 1 ] = new complexFilter() { key = "updated_at", value = new associativeEntity() { key = "to", value = modifiedTo.ToString() } };

			var res = await this._magentoSoapService.salesOrderListAsync( session.result, filters ).ConfigureAwait( false );

			return res;
		}
	}
}