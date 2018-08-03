using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagentoAccess;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetProducts;
using MagentoAccessTestsIntegration.TestEnvironment;
using NUnit.Framework;

namespace MagentoAccessTestsIntegration.InterchangeabilityTests.GetProducts
{
	[ TestFixture ]
	[ Category( "ReadSmokeTests" ) ]
	[ Parallelizable ]
	internal class FillProductsDetails : BaseTest
	{
		[ Test ]
		[ TestCaseSource( typeof( InterchangeabilityTestCases ), nameof(InterchangeabilityTestCases.TestStoresCredentials) ) ]
		public void ProductsDetailsFilled( MagentoServiceCredentialsAndConfig credentialsRest, MagentoServiceCredentialsAndConfig credentialsSoap )
		{
			// ------------ Arrange
			var magentoServiceRest = this.CreateMagentoService( credentialsRest.SoapApiUser, credentialsRest.SoapApiKey, "null", "null", "null", "null", credentialsRest.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsRest.MagentoVersion, credentialsRest.GetProductsThreadsLimit, credentialsRest.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );
			var magentoServiceSoap = this.CreateMagentoService( credentialsSoap.SoapApiUser, credentialsSoap.SoapApiKey, "null", "null", "null", "null", credentialsSoap.StoreUrl, "http://w.com", "http://w.com", "http://w.com", credentialsSoap.MagentoVersion, credentialsSoap.GetProductsThreadsLimit, credentialsSoap.SessionLifeTimeMs, false, ThrowExceptionIfFailed.AllItems );

			var getProductsSoapTask = magentoServiceSoap.GetProductsAsync( new[] { 0, 1 }, includeDetails : false );
			getProductsSoapTask.Wait();
			var productsList = getProductsSoapTask.Result;

			// ------------ Act
			var swR = Stopwatch.StartNew();
			var productsListRest = productsList.Select( p => p.DeepClone() );
			var fillProductsDetailsRest = magentoServiceRest.FillProductsDetailsAsync( productsListRest );
			fillProductsDetailsRest.Wait();
			swR.Stop();

			Task.Delay( 500 ).Wait();
			var swS = Stopwatch.StartNew();
			var productsListSoap = productsList.Select( p => p.DeepClone() );
			var fillProductsDetailsSoap = magentoServiceSoap.FillProductsDetailsAsync( productsListSoap );
			fillProductsDetailsSoap.Wait();
			swS.Stop();

			// ------------ Assert
			Console.WriteLine( "rest time: " + swR.Elapsed + " soap time: " + swS.Elapsed );

			var thatWasReturnedRest = fillProductsDetailsRest.Result.OrderBy( x => x.ProductId ).ToList();
			var thatWasReturnedSoap = fillProductsDetailsSoap.Result.OrderBy( x => x.ProductId ).ToList();

			thatWasReturnedRest.ForEach( item => item.Categories = item.Categories?.OrderBy( c => c.Id ).Select( c => new Category( c, true ) ).ToArray() );
			thatWasReturnedSoap.ForEach( item => item.Categories = item.Categories?.OrderBy( c => c.Id ).Select( c => new Category( c, true ) ).ToArray() );

			thatWasReturnedRest.Should().BeEquivalentTo( thatWasReturnedSoap );
			swS.Elapsed.Should().BeGreaterThan( swR.Elapsed );
		}
	}
}