﻿using System.Collections.Generic;
using System.Linq;
using LINQtoCSV;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.TestEnvironment
{
	public static class Environment
	{
		private const string TestCasesFile = @"\..\..\Files\Credentials_magento_TestEnvironment.csv";
		private static readonly IEnumerable< EnvironmentCredentialRow > _environmentRows;

		static Environment()
		{
			var cc = new CsvContext();
			_environmentRows = cc.Read<EnvironmentCredentialRow>( TestContext.CurrentContext.TestDirectory + TestCasesFile, new CsvFileDescription { FirstLineHasColumnNames = true } );
		}
		
		public static IEnumerable<EnvironmentCredentialRow> ActiveEnvironmentRows
		{
			get
			{
				return _environmentRows.Where( line => line.Active == "1" );
			}
		}

		public class EnvironmentCredentialRow
		{
			[ CsvColumn( Name = "Active", FieldIndex = 1 ) ]
			public string Active { get; set; }

			[ CsvColumn( Name = "V1", FieldIndex = 2 ) ]
			public string V1 { get; set; }

			[ CsvColumn( Name = "V2", FieldIndex = 3 ) ]
			public string V2 { get; set; }

			[ CsvColumn( Name = "Rest", FieldIndex = 4 ) ]
			public string Rest { get; set; }

			[ CsvColumn( Name = "Version", FieldIndex = 5 ) ]
			public string Version { get; set; }

			[ CsvColumn( Name = "MagentoUrl", FieldIndex = 6 ) ]
			public string MagentoUrl { get; set; }

			[ CsvColumn( Name = "MagentoLogin", FieldIndex = 7 ) ]
			public string MagentoLogin { get; set; }

			[ CsvColumn( Name = "MagentoPass", FieldIndex = 8 ) ]
			public string MagentoPass { get; set; }
		}
	}
}