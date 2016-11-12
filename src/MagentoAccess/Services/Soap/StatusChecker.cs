using System;
using System.ServiceModel;
using System.Threading;
using MagentoAccess.MagentoSoapServiceReference_v_1_14_1_EE;

namespace MagentoAccess.Services.Soap
{
	internal class StatusChecker
	{
		private int invokeCount;
		public int ChecksCount => this.invokeCount;
		public bool IsAborted => this.maxCount <= this.invokeCount;
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

		public void CheckStatus2< TClient >( ClientBase< TClient > clientBase ) where TClient : class
		{
			lock( this )
			{
				if( ++this.invokeCount == this.maxCount )
				{
					clientBase?.Abort();
				}
			}
		}

		public void CheckStatus3< TClient >( object stateInfo ) where TClient : class
		{
			lock( this )
			{
				var temp = stateInfo as ClientBase< TClient >;
				if( ++this.invokeCount == this.maxCount )
				{
					this.invokeCount = 0;
					if( temp != null )
					{
						temp.Abort();
					}
				}
			}
		}
	}
}