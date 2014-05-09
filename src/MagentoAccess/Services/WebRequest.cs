namespace MagentoAccess.Services
{
	//public partial class WebRequestServices 
	//{
	//	private readonly EbayUserCredentials _userCredentials;
	//	private readonly EbayDevCredentials _ebayDevCredentials;

	//	public WebRequestServices(EbayUserCredentials userCredentials, EbayDevCredentials ebayDevCredentials)
	//	{
	//		Condition.Requires(userCredentials, "userCredentials").IsNotNull();
	//		Condition.Requires(ebayDevCredentials, "ebayDevCredentials").IsNotNull();

	//		this._userCredentials = userCredentials;
	//		this._ebayDevCredentials = ebayDevCredentials;
	//	}

	//	#region BaseRequests
	//	public WebRequest CreateServiceGetRequest(string serviceUrl, IEnumerable<Tuple<string, string>> rawUrlParameters)
	//	{
	//		var parametrizedServiceUrl = serviceUrl;

	//		if (rawUrlParameters.Any())
	//		{
	//			parametrizedServiceUrl += "?" + rawUrlParameters.Aggregate(string.Empty,
	//				(accum, item) => accum + "&" + string.Format("{0}={1}", item.Item1, item.Item2));
	//		}

	//		var serviceRequest = WebRequest.Create(parametrizedServiceUrl);
	//		serviceRequest.Method = WebRequestMethods.Http.Get;
	//		return serviceRequest;
	//	}

	//	public async Task<WebRequest> CreateServicePostRequestAsync(string serviceUrl, string body, Dictionary<string, string> rawHeaders)
	//	{
	//		var encoding = new UTF8Encoding();
	//		var encodedBody = encoding.GetBytes(body);

	//		var serviceRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
	//		serviceRequest.Method = WebRequestMethods.Http.Post;
	//		serviceRequest.ContentType = "text/xml";
	//		serviceRequest.ContentLength = encodedBody.Length;
	//		serviceRequest.KeepAlive = true;

	//		foreach (var rawHeadersKey in rawHeaders.Keys)
	//		{
	//			serviceRequest.Headers.Add(rawHeadersKey, rawHeaders[rawHeadersKey]);
	//		}

	//		using (var newStream = await serviceRequest.GetRequestStreamAsync().ConfigureAwait(false))
	//			newStream.Write(encodedBody, 0, encodedBody.Length);

	//		return serviceRequest;
	//	}
	//	#endregion

	//	#region logging
	//	private void LogTraceStartGetResponse()
	//	{
	//		this.Log().Trace("[ebay] Get response started.");
	//	}

	//	private void LogTraceEndGetResponse()
	//	{
	//		this.Log().Trace("[ebay] Get response ended.");
	//	}

	//	private void LogTraceGetResponseAsyncStarted()
	//	{
	//		this.Log().Trace("[ebay] Get response async started.");
	//	}

	//	private void LogTraceGetResponseAsyncEnded()
	//	{
	//		this.Log().Trace("[ebay] Get response async ended.");
	//	}
	//	#endregion

	//	#region ResponseHanding
	//	public Stream GetResponseStream(WebRequest webRequest)
	//	{
	//		this.LogTraceStartGetResponse();
	//		using (var response = (HttpWebResponse)webRequest.GetResponse())
	//		using (var dataStream = response.GetResponseStream())
	//		{
	//			var memoryStream = new MemoryStream();
	//			if (dataStream != null)
	//				dataStream.CopyTo(memoryStream, 0x100);
	//			memoryStream.Position = 0;
	//			return memoryStream;
	//		}
	//		this.LogTraceEndGetResponse();
	//	}

	//	public async Task<Stream> GetResponseStreamAsync(WebRequest webRequest)
	//	{
	//		this.LogTraceGetResponseAsyncStarted();
	//		using (var response = (HttpWebResponse)await webRequest.GetResponseAsync().ConfigureAwait(false))
	//		using (var dataStream = await new TaskFactory<Stream>().StartNew(() => response != null ? response.GetResponseStream() : null).ConfigureAwait(false))
	//		{
	//			var memoryStream = new MemoryStream();
	//			await dataStream.CopyToAsync(memoryStream, 0x100).ConfigureAwait(false);
	//			memoryStream.Position = 0;
	//			this.LogTraceGetResponseAsyncEnded();
	//			return memoryStream;
	//		}
	//	}
	//	#endregion
	//}
}