using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetProducts;
using Netco.Extensions;
using Netco.Logging;

namespace MagentoAccess.Services.Soap._1_7_0_1_ce_1_9_0_1_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_from_1_7_to_1_9_CE : IMagentoServiceLowLevelSoap, IMagentoServiceLowLevelSoapGetProductsBySku, IMagentoServiceLowLevelSoapFillProductsDetails
	{
		public virtual async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, Mark mark = null )
		{
			try
			{
				Func< int, int, Func< int, string >, Task< List< SoapProduct > > > productsSelector = async ( start1, count1, selector1 ) =>
				{
					var sourceList = Enumerable.Range( start1, count1 ).Select( selector1 ).ToList();

					var productsResponses = await sourceList.ProcessInBatchAsync( this._getProductsMaxThreads, async x => await this.GetProductsAsync( productType, productTypeShouldBeExcluded, x, updatedFrom ).ConfigureAwait( false ) ).ConfigureAwait( false );
					var prods = productsResponses.SelectMany( x => x.Products ).ToList();
					return prods;
				};

				var productsMainPart = ( await productsSelector( 0, 100, x => "%" + x.ToString( "D2" ) ).ConfigureAwait( false ) ).ToList();
				productsMainPart.AddRange( await productsSelector( 0, 10, x => x.ToString( "D1" ) ).ConfigureAwait( false ) );
				var soapGetProductsResponse = new SoapGetProductsResponse { Products = productsMainPart };

				return soapGetProductsResponse;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		public virtual async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, IReadOnlyCollection< string > skus, Mark mark = null )
		{
			try
			{
				var skusList = skus.ToList();
				var skusListChunks = skusList.SplitToChunks( 10 );
				var responses = await skusListChunks.ProcessInBatchAsync( this._getProductsMaxThreads, async x => await this.GetProductsPageAsync( productType, productTypeShouldBeExcluded, x, updatedFrom ).ConfigureAwait( false ) ).ConfigureAwait( false );
				var soapGetProductsResponse = new SoapGetProductsResponse { Products = responses.SelectMany( x => x.Products ) };
				return soapGetProductsResponse;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		protected virtual async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, string productIdLike, DateTime? updatedFrom )
		{
			try
			{
				var filters = new filters { filter = new associativeEntity[ 0 ], complex_filter = new complexFilter[ 0 ] };

				if( productType != null )
					AddFilter( filters, productType, "type", productTypeShouldBeExcluded ? "neq" : "eq" );
				if( updatedFrom.HasValue )
					AddFilter( filters, updatedFrom.Value.ToSoapParameterString(), "updated_at", "from" );
				if( !string.IsNullOrWhiteSpace( productIdLike ) )
					AddFilter( filters, productIdLike, "product_id", "like" );

				var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;

				var res = new catalogProductListResponse();
				var privateClient = this._clientFactory.GetClient();

				await ActionPolicies.GetAsync.Do( async () =>
				{
					var statusChecker = new StatusChecker( maxCheckCount );
					TimerCallback tcb = statusChecker.CheckStatus;

					privateClient = this._clientFactory.RefreshClient( privateClient );
					var sessionId = await this.GetSessionId().ConfigureAwait( false );

					using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
						res = await privateClient.catalogProductListAsync( sessionId.SessionId, filters, store ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new SoapGetProductsResponse( res );
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		protected virtual async Task< SoapGetProductsResponse > GetProductsPageAsync( string productType, bool productTypeShouldBeExcluded, IEnumerable< string > skus, DateTime? updatedFrom )
		{
			var inCondition = string.Join( ",", ( skus ?? Enumerable.Empty< string >() ) );
			var filters = new filters { filter = new associativeEntity[ 0 ], complex_filter = new complexFilter[ 0 ] };

			if( productType != null )
				AddFilter( filters, productType, "type", productTypeShouldBeExcluded ? "neq" : "eq" );
			if( updatedFrom.HasValue )
				AddFilter( filters, updatedFrom.Value.ToSoapParameterString(), "updated_at", "from" );
			if( !string.IsNullOrWhiteSpace( inCondition ) )
				AddFilter( filters, inCondition, "sku", "in" );

			var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;

			return await this.GetWithAsync(
				res => new SoapGetProductsResponse( res ),
				async ( client, session ) => await client.catalogProductListAsync( session, filters, store ).ConfigureAwait( false ), 600000 ).ConfigureAwait( false );
		}
	}
}