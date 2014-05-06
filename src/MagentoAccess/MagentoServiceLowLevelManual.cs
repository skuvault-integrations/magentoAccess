//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using DotNetOpenAuth.Messaging;
//using DotNetOpenAuth.OAuth;
//using DotNetOpenAuth.OAuth.ChannelElements;
//using MagentoAccess.Models.GetProducts;

//namespace MagentoAccess
//{
//	public class MagentoServiceLowLevelManual
//	{
//		public IEnumerable< Item > GetItems()
//		{
//			var items = new List< Item >();

//			var RequestTokenUrl = "http://192.168.0.104/magento/oauth/initiate";
//			var AuthorizeUrl = "http://192.168.0.104/magento/admin/oauth/authorize";
//			var AccessTokenUrl = "http://192.168.0.104/magento/oauth/token";
//			var ResourceUrl = "http://192.168.0.104/magento/api/rest/products";

//			var service = new ServiceProviderDescription
//			{
//				RequestTokenEndpoint = new MessageReceivingEndpoint( RequestTokenUrl, HttpDeliveryMethods.PostRequest ),
//				UserAuthorizationEndpoint = new MessageReceivingEndpoint( AuthorizeUrl, HttpDeliveryMethods.GetRequest ),
//				AccessTokenEndpoint = new MessageReceivingEndpoint( AccessTokenUrl, HttpDeliveryMethods.PostRequest ),
//				TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
//				ProtocolVersion = ProtocolVersion.V10a,
//			};
//			var accessToken = "0e83e9e55366a1919f0cc84ec80df31c";
//			var accessTokenSecret = "d3470a7b252f50f066bddfd9268e9dc0";

//			var tokenManager = new InMemoryTokenManager();
//			tokenManager.ConsumerKey = "59704e7d6b9abd742de255b7c97421f6";
//			tokenManager.ConsumerSecret = "476130ddd7cdf5709fd9a95bee24b71d";
//			tokenManager.tokensAndSecrets[ accessToken ] = accessTokenSecret;

//			var consumer = new DesktopConsumer( service, tokenManager );
//			var resourceHttpMethod = HttpDeliveryMethods.GetRequest;
//			//if (false)
//			{
//				//resourceHttpMethod |= HttpDeliveryMethods.AuthorizationHeaderRequest;
//			}
//			var resourceEndpoint = new MessageReceivingEndpoint( ResourceUrl, resourceHttpMethod );
//			using( var resourceResponse = consumer.PrepareAuthorizedRequestAndSend( resourceEndpoint, accessToken ) )
//			{
//				var results = resourceResponse.GetResponseReader().ReadToEnd();
//			}

//			return items;
//		}

//		public IEnumerable< Item > GetItems3()
//		{
//			// Setup the variables necessary to create the OAuth 1.0 signature and make the request   
//			var httpMethod = "GET";
//			var url = new Uri( String.Format( "{0}", "http://192.168.0.104/magento/api/rest/products" ) );
//			//string appID = "{applicationId}";
//			var consumerKey = "59704e7d6b9abd742de255b7c97421f6";
//			var body = "";
//			MemoryStream requestBody = null;
//			var signatureMethod = "HMAC-SHA1";
//			var secret = "476130ddd7cdf5709fd9a95bee24b71d";
//			HttpWebResponse response = null;

//			// Set the Nonce and Timestamp parameters
//			var nonce = getNonce();
//			var timestamp = getTimestamp();

//			// Set the request body if making a POST or PUT request
//			if( httpMethod == "POST" || httpMethod == "PUT" )
//				requestBody = new MemoryStream( Encoding.UTF8.GetBytes( body ) );

//			// Create the OAuth parameter name/value pair dictionary
//			var oauthParams = new Dictionary< string, string >
//			{
//				{ "oauth_consumer_key", consumerKey },
//				//{ "application_id", appID },
//				{ "oauth_signature_method", signatureMethod },
//				{ "oauth_timestamp", timestamp },
//				{ "oauth_nonce", nonce },
//			};

//			// Get the OAuth 1.0 Signature
//			var signature = generateSignature( httpMethod, url, oauthParams, requestBody, secret );
//			Console.WriteLine( "OAuth 1.0 Signature = " + signature + "\r\n\r\n" );

//			// Add the oauth_signature parameter to the set of OAuth Parameters
//			var allParams = oauthParams.Union( new[]
//			{
//				new KeyValuePair< string, string >( "oauth_signature", signature )
//			} );

//			// Defines a query that produces a set of: keyname="URL-encoded(value)"
//			var encodedParams = from param in allParams
//				select param.Key + "=\"" + Uri.EscapeDataString( param.Value ) + "\"";

//			// Join all encoded parameters with a comma delimiter and convert to a string
//			var stringParams = String.Join( ",", encodedParams );

//			// Build the X-Authorization request header
//			var xauth = String.Format( "X-Authorization: OAuth realm=\"{0}\",{1}", url, stringParams );
//			Console.WriteLine( "X-Authorization request header: \r\n" + xauth + "\r\n\r\n" );
//			try
//			{
//				// Setup the Request
//				var request = ( HttpWebRequest )WebRequest.Create( url );
//				request.Method = httpMethod;
//				request.Headers.Add( xauth );

//				// Set the request body if making a POST or PUT request
//				if( httpMethod == "POST" || httpMethod == "PUT" )
//				{
//					var dataArray = Encoding.UTF8.GetBytes( body );
//					request.ContentLength = dataArray.Length;

//					var requestStream = request.GetRequestStream();
//					requestStream.Write( dataArray, 0, dataArray.Length );
//					requestStream.Close();
//				}

//				// Send Request & Get Response
//				response = ( HttpWebResponse )request.GetResponse();
//				using( var reader = new StreamReader( response.GetResponseStream() ) )
//				{
//					// Get the response stream and write to console
//					var json = reader.ReadToEnd();
//					Console.WriteLine( "Successful Response: \r\n" + json );
//				}
//			}
//			catch( Exception )
//			{
//				throw;
//			}
//		}

//		/// <summary>
//		/// Generates an OAuth 1.0 signature.
//		/// </summary>
//		/// <param name="httpMethod">The HTTP method of the request.</param>
//		/// <param name="url">The URI of the request.</param>
//		/// <param name="oauthParams">The associative set of signable oauth parameters.</param>
//		/// <param name="requestBody">A stream containing the serialized message body.</param>
//		/// <param name="secret">Alphanumeric string used to validate the identity of the education partner (Private Key).</param>
//		/// <returns>A string containing the BASE64-encoded signature digest.</returns>
//		private static string generateSignature(
//			string httpMethod,
//			Uri url,
//			IDictionary< string, string > oauthParams,
//			Stream requestBody,
//			string secret
//			)
//		{
//			// Ensure the HTTP Method is upper-cased
//			httpMethod = httpMethod.ToUpper();

//			// Construct the URL-encoded OAuth parameter portion of the signature base string
//			var encodedParams = normalizeParams( httpMethod, url, oauthParams, requestBody );

//			// URL-encode the relative URL
//			var encodedUri = Uri.EscapeDataString( url.AbsolutePath );

//			// Build the signature base string to be signed with the Consumer Secret
//			var baseString = String.Format( "{0}&{1}&{2}", httpMethod, encodedUri, encodedParams );

//			return generateCmac( secret, baseString );
//		}

//		/// <summary>
//		/// Generates a BASE64-encoded CMAC-AES digest.
//		/// </summary>
//		/// <param name="key">The secret key used to sign the data.</param>
//		/// <param name="msg">The data to be signed.</param>
//		/// <returns>A CMAC-AES digest.</returns>
//		private static string generateCmac( string key, string msg )
//		{
//			var keyBytes = Encoding.UTF8.GetBytes( key );
//			var msgBytes = Encoding.UTF8.GetBytes( msg );

//			var macProvider = new CMac( new AesFastEngine() );
//			macProvider.Init( new KeyParameter( keyBytes ) );
//			macProvider.Reset();

//			macProvider.BlockUpdate( msgBytes, 0, msgBytes.Length );
//			var output = new byte[ macProvider.GetMacSize() ];
//			macProvider.DoFinal( output, 0 );

//			return Convert.ToBase64String( output );
//		}

//		/// <summary>
//		/// Normalizes all oauth signable parameters and url query parameters according to OAuth 1.0.
//		/// </summary>
//		/// <param name="httpMethod">The upper-cased HTTP method.</param>
//		/// <param name="url">The request URL.</param>
//		/// <param name="oauthParams">The associative set of signable oauth parameters.</param>
//		/// <param name="requestBody">A stream containing the serialized message body.</param>
//		/// <returns>A string containing normalized and encoded OAuth parameters.</returns>
//		private static string normalizeParams(
//			string httpMethod,
//			Uri url,
//			IEnumerable< KeyValuePair< string, string > > oauthParams,
//			Stream requestBody
//			)
//		{
//			var kvpParams = oauthParams;

//			// Place any Query String parameters into a key value pair using equals ("=") to mark
//			// the key/value relationship and join each paramter with an ampersand ("&")
//			if( !String.IsNullOrWhiteSpace( url.Query ) )
//			{
//				var queryParams =
//					from p in url.Query.Substring( 1 ).Split( '&' ).AsEnumerable()
//					let key = Uri.EscapeDataString( p.Substring( 0, p.IndexOf( "=" ) ) )
//					let value = Uri.EscapeDataString( p.Substring( p.IndexOf( "=" ) + 1 ) )
//					select new KeyValuePair< string, string >( key, value );

//				kvpParams = kvpParams.Union( queryParams );
//			}

//			// Include the body parameter if dealing with a POST or PUT request
//			if( httpMethod == "POST" || httpMethod == "PUT" )
//			{
//				var ms = new MemoryStream();
//				requestBody.CopyTo( ms );
//				var bodyBytes = ms.ToArray();

//				var body = Convert.ToBase64String( bodyBytes, Base64FormattingOptions.None );
//				body = Uri.EscapeDataString( body );

//				kvpParams = kvpParams.Union( new[]
//				{
//					new KeyValuePair< string, string >( "body", Uri.EscapeDataString( body ) )
//				} );
//			}

//			// Sort the parameters in lexicographical order, 1st by Key then by Value; separate with ("=")
//			var sortedParams =
//				from p in kvpParams
//				orderby p.Key ascending, p.Value ascending
//				select p.Key + "=" + p.Value;

//			// Add the ampersand delimiter and then URL-encode the equals ("%3D") and ampersand ("%26")
//			var stringParams = String.Join( "&", sortedParams );
//			var encodedParams = Uri.EscapeDataString( stringParams );

//			return encodedParams;
//		}

//		/// <summary>
//		/// Generates a random nonce.
//		/// </summary>
//		/// <returns>A unique identifier for the request.</returns>
//		private static string getNonce()
//		{
//			var rtn = Path.GetRandomFileName() + Path.GetRandomFileName() + Path.GetRandomFileName();
//			rtn = rtn.Replace( ".", "" );
//			if( rtn.Length > 32 )
//				return rtn.Substring( 0, 32 );
//			else
//				return rtn;
//		}

//		/// <summary>
//		/// Generates an integer representing the number of seconds since the unix epoch using the 
//		/// UTC date/time of the request.
//		/// </summary>
//		/// <returns>A timestamp for the request.</returns>
//		private static string getTimestamp()
//		{
//			return ( ( int )( DateTime.UtcNow - new DateTime( 1970, 1, 1 ) ).TotalSeconds ).ToString();
//		}
//	}
//}

