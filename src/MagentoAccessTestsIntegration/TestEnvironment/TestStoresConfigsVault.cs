using System.Collections.Generic;
using System.Linq;
using LINQtoCSV;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	public static class TestStoresConfigsVault
	{
		private const string TestCasesFile = @"\..\..\Files\Credentials_magento_TestEnvironment.csv";
		private static readonly IEnumerable< StoreConfig > _environmentRows;

		static TestStoresConfigsVault()
		{
			var cc = new CsvContext();
			_environmentRows = cc.Read< StoreConfig >( TestContext.CurrentContext.TestDirectory + TestCasesFile, new CsvFileDescription { FirstLineHasColumnNames = true } );
		}

		public static IEnumerable< StoreConfig > GetActiveConfigs
		{
			get { return _environmentRows.Where( line => line.Active == "1" ); }
		}

		public class StoreConfig
		{
			[ CsvColumn( Name = "Active", FieldIndex = 1 ) ]
			public string Active { get; set; }

			[ CsvColumn( Name = "V1", FieldIndex = 2 ) ]
			public string V1 { get; set; }

			[ CsvColumn( Name = "V2", FieldIndex = 3 ) ]
			public string V2 { get; set; }

			[ CsvColumn( Name = "Rest", FieldIndex = 4 ) ]
			public string Rest { get; set; }

			[ CsvColumn( Name = "MagentoVersion", FieldIndex = 5 ) ]
			public string MagentoVersion { get; set; }

			[ CsvColumn( Name = "ServiceVersion", FieldIndex = 6 ) ]
			public string ServiceVersion { get; set; }

			[ CsvColumn( Name = "MagentoUrl", FieldIndex = 7 ) ]
			public string MagentoUrl { get; set; }

			[ CsvColumn( Name = "MagentoLogin", FieldIndex = 8 ) ]
			public string MagentoLogin { get; set; }

			[ CsvColumn( Name = "MagentoPass", FieldIndex = 9 ) ]
			public string MagentoPass { get; set; }

			[ CsvColumn( Name = "GetProductThreadsLimit", FieldIndex = 10 ) ]
			public string GetProductThreadsLimit { get; set; }

			[ CsvColumn( Name = "GetProductDetailsThreadsLimit", FieldIndex = 11 ) ]
			public string GetProductDetailsThreadsLimit { get; set; }
		}
	}
}