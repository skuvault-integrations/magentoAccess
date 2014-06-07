using System.Collections.Generic;
using System.Linq;
using LINQtoCSV;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	public class TestData
	{
		private readonly FlatCsvLineConsumer _flatCsvLineConsumer;
		private readonly FlatCsvLineUrls _flatCsvLineUrls;
		private FlatCsvLineAccessToken _flatCsvLinesAccessToken;
		private readonly FlatCsvLineVerification _flatCsvLinesVerification;
		private string _accessTokenFilePath;

		public TestData( string consumerKeyFilePath, string urlsFilePath, string accessTokenFilePath, string verificationPath )
		{
			var cc = new CsvContext();
			this._accessTokenFilePath = accessTokenFilePath;
			this._flatCsvLinesVerification = Enumerable.FirstOrDefault( cc.Read< FlatCsvLineVerification >( verificationPath, new CsvFileDescription { FirstLineHasColumnNames = true } ) );
			this._flatCsvLineConsumer = Enumerable.FirstOrDefault( cc.Read< FlatCsvLineConsumer >( consumerKeyFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ) );
			this._flatCsvLineUrls = Enumerable.FirstOrDefault( cc.Read< FlatCsvLineUrls >( urlsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ) );
			this._flatCsvLinesAccessToken = Enumerable.FirstOrDefault( cc.Read< FlatCsvLineAccessToken >( accessTokenFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ) );
		}

		public void CreateAccessTokenFile( string accessToken, string accessTokenSecret )
		{
			var cc = new CsvContext();
			cc.Write< FlatCsvLineAccessToken >(
				new List< FlatCsvLineAccessToken > { new FlatCsvLineAccessToken { AccessToken = accessToken, AccessTokenSecret = accessTokenSecret } },
				this._accessTokenFilePath );
		}

		public MagentoConsumerCredentials GetMagentoConsumerCredentials()
		{
			return this._flatCsvLineConsumer == null ? null : new MagentoConsumerCredentials( this._flatCsvLineConsumer.ConsumerKey, this._flatCsvLineConsumer.ConsumerSecretKey );
		}

		public MagentoUrls GetMagentoUrls()
		{
			return this._flatCsvLineUrls == null ? null : new MagentoUrls( this._flatCsvLineUrls.MagentoBaseUrl, this._flatCsvLineUrls.RequestTokenUrl, this._flatCsvLineUrls.AuthorizeUrl, this._flatCsvLineUrls.AccessTokenUrl );
		}

		public MagentoAccessToken GetMagentoAccessToken()
		{
			return this._flatCsvLinesAccessToken == null ? null : new MagentoAccessToken( this._flatCsvLinesAccessToken.AccessToken, this._flatCsvLinesAccessToken.AccessTokenSecret );
		}

		public string TransmitVerification()
		{
			return this._flatCsvLinesVerification == null ? null : this._flatCsvLinesVerification.VerifierCode;
		}

		internal class FlatCsvLineConsumer
		{
			public FlatCsvLineConsumer()
			{
			}

			[ CsvColumn( Name = "ConsumerKey", FieldIndex = 1 ) ]
			public string ConsumerKey { get; set; }

			[ CsvColumn( Name = "ConsumerSecretKey", FieldIndex = 2 ) ]
			public string ConsumerSecretKey { get; set; }
		}

		internal class FlatCsvLineUrls
		{
			public FlatCsvLineUrls()
			{
			}

			[ CsvColumn( Name = "MagentoUrl", FieldIndex = 1 ) ]
			public string MagentoBaseUrl { get; set; }

			[ CsvColumn( Name = "RequestTokenUrl", FieldIndex = 2 ) ]
			public string RequestTokenUrl { get; set; }

			[ CsvColumn( Name = "AuthorizeUrl", FieldIndex = 3 ) ]
			public string AuthorizeUrl { get; set; }

			[ CsvColumn( Name = "AccessTokenUrl", FieldIndex = 4 ) ]
			public string AccessTokenUrl { get; set; }
		}

		internal class FlatCsvLineAccessToken
		{
			public FlatCsvLineAccessToken()
			{
			}

			[ CsvColumn( Name = "AccessToken", FieldIndex = 1 ) ]
			public string AccessToken { get; set; }

			[ CsvColumn( Name = "AccessTokenSecret", FieldIndex = 2 ) ]
			public string AccessTokenSecret { get; set; }
		}

		internal class FlatCsvLineVerification
		{
			public FlatCsvLineVerification()
			{
			}

			[ CsvColumn( Name = "VerifierCode", FieldIndex = 1 ) ]
			public string VerifierCode { get; set; }
		}
	}
}