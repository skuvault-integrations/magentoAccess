namespace MagentoAccess.Models.Services.Soap.GetSessionId
{
	internal class GetSessionIdResponse
	{
		public string SessionId{ get; protected set; }
		public bool ReceivedFromCache{ get; protected set; }

		public GetSessionIdResponse( string sessionId, bool receivedFromCache )
		{
			this.SessionId = sessionId;
			this.ReceivedFromCache = receivedFromCache;
		}
	}
}