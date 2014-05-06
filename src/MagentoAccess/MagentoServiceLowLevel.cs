using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using MagentoAccess.Interfaces;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using Netco.Extensions;

namespace MagentoAccess
{
	public class MagentoServiceLowLevel : IMagentoServiceLowLevel
	{
		private readonly IWebRequestServices _webRequestServices = new WebRequestServices();
		private readonly string _endPoint;


		public MagentoServiceLowLevel(
			//EbayUserCredentials credentials, 
			//EbayDevCredentials ebayDevCredentials, 
			IWebRequestServices webRequestServices,
			string endPouint = "http://192.168.0.104/magento/api/rest/products")
		{
			//Condition.Requires( credentials, "credentials" ).IsNotNull();
			Condition.Ensures( endPouint, "endPoint" ).IsNotNullOrEmpty();
			Condition.Requires( webRequestServices, "webRequestServices" ).IsNotNull();
			//Condition.Requires( ebayDevCredentials, "ebayDevCredentials" ).IsNotNull();

			//this._userCredentials = credentials;
			this._webRequestServices = webRequestServices;
			this._endPoint = endPouint;
			//this._itemsPerPage = itemsPerPage;
			//this._ebayDevCredentials = ebayDevCredentials;
		}

		//public async Task<WebRequest> CreateEbayStandartGetRequestAsync(string url, Dictionary<string, string> headers, string body)
		//{
		//	try
		//	{
		//		const string xMagentoApiOauthVersion = "oauth_version";
		//		if (!headers.Exists(keyValuePair => keyValuePair.Key == xMagentoApiOauthVersion))
		//			headers.Add(xMagentoApiOauthVersion, "1.0");

		//		const string xMagenToApiOAuthSignatureMethod = "oauth_signature_method";
		//		if (!headers.Exists(keyValuePair => keyValuePair.Key == xMagenToApiOAuthSignatureMethod))
		//			headers.Add(xMagenToApiOAuthSignatureMethod, "HMAC-SHA1");

		//		const string xMagentiApiOauthNonce = "oauth_nonce";
		//		if (!headers.Exists(keyValuePair => keyValuePair.Key == xMagentiApiOauthNonce))
		//			headers.Add(xMagentiApiOauthNonce, "ZzeuFMWqirj21kc");

		//		const string xMagentoApiOauthTimestamp = "oauth_timestamp";
		//		if (!headers.Exists(keyValuePair => keyValuePair.Key == xMagentoApiOauthTimestamp))
		//			headers.Add(xMagentoApiOauthTimestamp, ((long)(DateTime.Now.ToUniversalTime()- new DateTime(1970,1,1,0,0,0,0)).TotalSeconds).ToString());

		//		const string xMagentoApiConsumerKey = "oauth_consumer_key";
		//		if (!headers.Exists(keyValuePair => keyValuePair.Key == xMagentoApiConsumerKey))
		//			headers.Add(xMagentoApiConsumerKey, "59704e7d6b9abd742de255b7c97421f6");

		//		const string xMagentoApiOauthToken = "oauth_token";
		//		if (!headers.Exists(keyValuePair => keyValuePair.Key == xMagentoApiOauthToken))
		//			headers.Add(xMagentoApiOauthToken, "0e83e9e55366a1919f0cc84ec80df31c");

		//		const string xMagentoApiOauthSignature = "oauth_signature";
		//		if (!headers.Exists(keyValuePair => keyValuePair.Key == xMagentoApiOauthSignature))
		//			headers.Add(xMagentoApiOauthSignature, "sEZwwmQXMHkRdVP1NLIK5fyi%2FDw%3D");

		//		//const string xEbayApiAppName = "X-EBAY-API-APP-NAME";
		//		//if (!headers.Exists(keyValuePair => keyValuePair.Key == xEbayApiAppName))
		//		//	headers.Add(xEbayApiAppName, this._ebayDevCredentials.DevName);

		//		//const string xEbayApiSiteid = "X-EBAY-API-SITEID";
		//		//if (!headers.Exists(keyValuePair => keyValuePair.Key == xEbayApiSiteid))
		//		//	headers.Add(xEbayApiSiteid, "0");

		//		return await this._webRequestServices.CreateServiceGetRequestAsync(url, body, headers).ConfigureAwait(false);
		//	}
		//	catch (WebException exc)
		//	{
		//		// todo: log some exceptions
		//		throw;
		//	}
		//}
		
		//public async Task< IEnumerable< Item > > GetItemsAsync()
		//{
		//	var items = new List<Item>();
		//	var body = this.CreateGetItemsRequestBody();

		//	var headers = CreateGetItemsRequestHeadersWithApiCallName();

		//		await ActionPolicies.GetAsync.Do( async() =>
		//		{


		//			var webRequest = await this.CreateEbayStandartGetRequestAsync(this._endPoint, headers, body).ConfigureAwait(false);

		//			using (var memStream = await this._webRequestServices.GetResponseStreamAsync(webRequest).ConfigureAwait(false))
		//			{
		//				//var pagination = new EbayPaginationResultResponseParser().Parse(memStream);
		//				//if (pagination != null)
		//				//	totalRecords = pagination.TotalNumberOfEntries;

		//				//var tempOrders = new EbayGetSallerListResponseParser().Parse(memStream);
		//				//if (tempOrders != null)
		//				//	items.AddRange(tempOrders.Items);
		//				var buff = new byte[memStream.Length];
		//				var res = memStream.Read(buff, 0, (int)memStream.Length);
		//			}
		//		}).ConfigureAwait(false);


		//	return items;
		//}

		public IEnumerable<Item> GetItems()
		{
			var items = new List<Item>();

			var RequestTokenUrl = "http://192.168.0.104/magento/oauth/initiate";
			var AuthorizeUrl = "http://192.168.0.104/magento/admin/oauth/authorize";
			var AccessTokenUrl = "http://192.168.0.104/magento/oauth/token";
			var ResourceUrl = "http://192.168.0.104/magento/api/rest/products";



			var service = new ServiceProviderDescription
			{
				RequestTokenEndpoint = new MessageReceivingEndpoint(RequestTokenUrl, HttpDeliveryMethods.PostRequest),
				UserAuthorizationEndpoint = new MessageReceivingEndpoint(AuthorizeUrl, HttpDeliveryMethods.GetRequest),
				AccessTokenEndpoint = new MessageReceivingEndpoint(AccessTokenUrl, HttpDeliveryMethods.PostRequest),
				TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
				ProtocolVersion = ProtocolVersion.V10a,
			};
			string accessToken = "0e83e9e55366a1919f0cc84ec80df31c";
			string accessTokenSecret = "d3470a7b252f50f066bddfd9268e9dc0";

			var tokenManager = new InMemoryTokenManager();
			tokenManager.ConsumerKey = "59704e7d6b9abd742de255b7c97421f6";
			tokenManager.ConsumerSecret = "476130ddd7cdf5709fd9a95bee24b71d";
			tokenManager.tokensAndSecrets[accessToken] = accessTokenSecret;

			var consumer = new DesktopConsumer(service, tokenManager);
			HttpDeliveryMethods resourceHttpMethod = HttpDeliveryMethods.GetRequest;
			//if (false)
			{
				//resourceHttpMethod |= HttpDeliveryMethods.AuthorizationHeaderRequest;
			}
			var resourceEndpoint = new MessageReceivingEndpoint(ResourceUrl, resourceHttpMethod);
			using (IncomingWebResponse resourceResponse = consumer.PrepareAuthorizedRequestAndSend(resourceEndpoint, accessToken))
			{
				var results = resourceResponse.GetResponseReader().ReadToEnd();
			}

			return items;
		}

	  //  public IEnumerable<Item> GetItems3()
	  //  {
	  //	  // Setup the variables necessary to create the OAuth 1.0 signature and make the request   
	  //	  string httpMethod = "{httpVerb}";
	  //	  string courseID = "{courseIdentification}";
	  //	  Uri url = new Uri(String.Format("{0}{1}", "https://{domain}/courses/", courseID));
	  //	  string appID = "{applicationId}";
	  //	  string consumerKey = "{consumerKey}";
	  //	  string body = "{requestBody}";
	  //	  MemoryStream requestBody = null;
	  //	  string signatureMethod = "CMAC-AES";
	  //	  string secret = "{consumerSecret}";
	  //	  HttpWebResponse response = null;

	  //	  // Set the Nonce and Timestamp parameters
	  //	  string nonce = getNonce();
	  //	  string timestamp = getTimestamp();

	  //	  // Set the request body if making a POST or PUT request
	  //	  if (httpMethod == "POST" || httpMethod == "PUT")
	  //	  {
	  //		  requestBody = new MemoryStream(Encoding.UTF8.GetBytes(body));
	  //	  }

	  //	  // Create the OAuth parameter name/value pair dictionary
	  //	  Dictionary<string, string> oauthParams = new Dictionary<string, string>
	  //{
	  //  { "oauth_consumer_key", consumerKey },
	  //  { "application_id", appID },
	  //  { "oauth_signature_method", signatureMethod },
	  //  { "oauth_timestamp", timestamp },
	  //  { "oauth_nonce", nonce },
	  //};


	  //	  // Get the OAuth 1.0 Signature
	  //	  string signature = generateSignature(httpMethod, url, oauthParams, requestBody, secret);
	  //	  Console.WriteLine("OAuth 1.0 Signature = " + signature + "\r\n\r\n");

	  //	  // Add the oauth_signature parameter to the set of OAuth Parameters
	  //	  IEnumerable<KeyValuePair<string, string>> allParams = oauthParams.Union(new[]
	  //{
	  //  new KeyValuePair<string, string>("oauth_signature", signature)
	  //});


	  //	  // Defines a query that produces a set of: keyname="URL-encoded(value)"
	  //	  IEnumerable<string> encodedParams = from param in allParams
	  //										  select param.Key + "=\"" + Uri.EscapeDataString(param.Value) + "\"";

	  //	  // Join all encoded parameters with a comma delimiter and convert to a string
	  //	  string stringParams = String.Join(",", encodedParams);

	  //	  // Build the X-Authorization request header
	  //	  string xauth = String.Format("X-Authorization: OAuth realm=\"{0}\",{1}", url, stringParams);
	  //	  Console.WriteLine("X-Authorization request header: \r\n" + xauth + "\r\n\r\n");
	  //	  try
	  //	  {
	  //		  // Setup the Request
	  //		  HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
	  //		  request.Method = httpMethod;
	  //		  request.Headers.Add(xauth);

	  //		  // Set the request body if making a POST or PUT request
	  //		  if (httpMethod == "POST" || httpMethod == "PUT")
	  //		  {
	  //			  byte[] dataArray = Encoding.UTF8.GetBytes(body);
	  //			  request.ContentLength = dataArray.Length;

	  //			  Stream requestStream = request.GetRequestStream();
	  //			  requestStream.Write(dataArray, 0, dataArray.Length);
	  //			  requestStream.Close();
	  //		  }

	  //		  // Send Request & Get Response
	  //		  response = (HttpWebResponse)request.GetResponse();
	  //		  using (StreamReader reader = new StreamReader(response.GetResponseStream()))
	  //		  {
	  //			  // Get the response stream and write to console
	  //			  string json = reader.ReadToEnd();
	  //			  Console.WriteLine("Successful Response: \r\n" + json);
	  //		  }
	  //	  }
	  //	  catch (Exception)
	  //	  {
	  //		  throw;
	  //	  }
	  //  }

		private Dictionary< string, string > CreateGetItemsRequestHeadersWithApiCallName()
		{
			return new Dictionary<string, string>();
		}

		private string CreateGetItemsRequestBody()
		{
			return string.Empty;
		}
	}

	public class WebRequestServices : IWebRequestServices
	{
		#region BaseRequests
		public WebRequest CreateServiceGetRequest(string serviceUrl, IEnumerable<Tuple<string, string>> rawUrlParameters)
		{
			var parametrizedServiceUrl = serviceUrl;

			if (rawUrlParameters.Any())
			{
				parametrizedServiceUrl += "?" + rawUrlParameters.Aggregate(string.Empty,
					(accum, item) => accum + "&" + string.Format("{0}={1}", item.Item1, item.Item2));
			}

			var serviceRequest = WebRequest.Create(parametrizedServiceUrl);
			serviceRequest.Method = WebRequestMethods.Http.Get;
			return serviceRequest;
		}

		public async Task<WebRequest> CreateServiceGetRequestAsync(string serviceUrl, string body, Dictionary<string, string> rawHeaders)
		{
			try
			{
				var encoding = new UTF8Encoding();
				var encodedBody = encoding.GetBytes(body);

				var serviceRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
				serviceRequest.Method = WebRequestMethods.Http.Get;
				serviceRequest.ContentType = "text/xml";
				serviceRequest.ContentLength = encodedBody.Length;
				serviceRequest.KeepAlive = true;

				foreach (var rawHeadersKey in rawHeaders.Keys)
				{
					serviceRequest.Headers.Add(rawHeadersKey, rawHeaders[rawHeadersKey]);
				}

				using (var newStream = await serviceRequest.GetRequestStreamAsync().ConfigureAwait(false))
					newStream.Write(encodedBody, 0, encodedBody.Length);

				return serviceRequest;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		#endregion

		#region ResponseHanding
		public Stream GetResponseStream(WebRequest webRequest)
		{
			//this.LogTraceStartGetResponse();
			using (var response = (HttpWebResponse)webRequest.GetResponse())
			using (var dataStream = response.GetResponseStream())
			{
				var memoryStream = new MemoryStream();
				if (dataStream != null)
					dataStream.CopyTo(memoryStream, 0x100);
				memoryStream.Position = 0;
				return memoryStream;
			}
			//this.LogTraceEndGetResponse();
		}

		public async Task<Stream> GetResponseStreamAsync(WebRequest webRequest)
		{
			try
			{
				//this.LogTraceGetResponseAsyncStarted();
				using (var response = (HttpWebResponse)await webRequest.GetResponseAsync().ConfigureAwait(false))
				using (var dataStream = await new TaskFactory<Stream>().StartNew(() => response != null ? response.GetResponseStream() : null).ConfigureAwait(false))
				{
					var memoryStream = new MemoryStream();
					await dataStream.CopyToAsync(memoryStream, 0x100).ConfigureAwait(false);
					memoryStream.Position = 0;
					//this.LogTraceGetResponseAsyncEnded();
					return memoryStream;
				}
			}
			catch (Exception)
			{
				
				throw;
			}
		}
		#endregion
	}

	public interface IWebRequestServices
	{
		Stream GetResponseStream(WebRequest webRequest);

		Task<Stream> GetResponseStreamAsync(WebRequest webRequest);

		WebRequest CreateServiceGetRequest(string serviceUrl, IEnumerable<Tuple<string, string>> rawUrlParameters);

		Task<WebRequest> CreateServiceGetRequestAsync(string serviceUrl, string body, Dictionary<string, string> rawHeaders);
	}

		internal class InMemoryTokenManager : IConsumerTokenManager {
		public Dictionary<string, string> tokensAndSecrets = new Dictionary<string, string>();

		internal InMemoryTokenManager() {
		}

		public string ConsumerKey { get; internal set; }

		public string ConsumerSecret { get; internal set; }

		#region ITokenManager Members

		public string GetConsumerSecret(string consumerKey) {
			if (consumerKey == this.ConsumerKey) {
				return this.ConsumerSecret;
			} else {
				throw new ArgumentException("Unrecognized consumer key.", "consumerKey");
			}
		}

		public string GetTokenSecret(string token) {
			return this.tokensAndSecrets[token];
		}

		public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response) {
			this.tokensAndSecrets[response.Token] = response.TokenSecret;
		}

		public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret) {
			this.tokensAndSecrets.Remove(requestToken);
			this.tokensAndSecrets[accessToken] = accessTokenSecret;
		}

		/// <summary>
		/// Classifies a token as a request token or an access token.
		/// </summary>
		/// <param name="token">The token to classify.</param>
		/// <returns>Request or Access token, or invalid if the token is not recognized.</returns>
		public TokenType GetTokenType(string token) {
			throw new NotImplementedException();
		}

		#endregion
	}
}