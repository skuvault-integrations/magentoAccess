using System;
using System.Threading;
using MagentoAccess.MagentoSoapServiceReference_v_1_14_1_EE;

namespace MagentoAccess.Services.Soap
{
	internal class StatusChecker
	{
		private int invokeCount;
		private readonly int maxCount;

		public StatusChecker( int count )
		{
			this.invokeCount = 0;
			this.maxCount = count;
		}

		public void CheckStatus( Object stateInfo )
		{
			dynamic serviceClient = null;

			if( stateInfo is Mage_Api_Model_Server_Wsi_HandlerPortTypeClient )
				serviceClient = stateInfo as Mage_Api_Model_Server_Wsi_HandlerPortTypeClient;
			else if( stateInfo is MagentoSoapServiceReference.Mage_Api_Model_Server_Wsi_HandlerPortTypeClient )
				serviceClient = stateInfo as MagentoSoapServiceReference.Mage_Api_Model_Server_Wsi_HandlerPortTypeClient;

			Interlocked.Increment( ref this.invokeCount );
			if( this.invokeCount == this.maxCount )
			{
				Interlocked.Exchange( ref this.invokeCount, 0 );
				if( serviceClient != null )
					serviceClient.Abort();
			}
		}
	}
}