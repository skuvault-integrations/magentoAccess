using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference_v_1_14_1_EE;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetProducts;
using Netco.Extensions;
using Netco.Logging;

namespace MagentoAccess.Services.Soap._1_14_1_0_ee
{
	internal partial class MagentoServiceLowLevelSoap_v_1_14_1_0_EE : IMagentoServiceLowLevelSoap, IMagentoServiceLowLevelSoapGetProductsBySku, IMagentoServiceLowLevelSoapFillProductsDetails
	{
		public virtual async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, CancellationToken cancellationToken, Mark mark = null )
		{
			try
			{
				Func< int, int, Func< int, string >, Task< List< SoapProduct > > > productsSelector = async ( start1, count1, selector1 ) =>
				{
					var sourceList = Enumerable.Range( start1, count1 ).Select( selector1 ).ToList();
					var productsResponses = await sourceList.ProcessInBatchAsync( this._getProductsMaxThreads, async x => await this.GetProductsAsync( productType, productTypeShouldBeExcluded, x, updatedFrom, cancellationToken ).ConfigureAwait( false ) ).ConfigureAwait( false );
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

		public virtual async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, IReadOnlyCollection< string > skus, CancellationToken cancellationToken, Mark mark = null )
		{
			try
			{
				var skusList = skus.ToList();
				var skusListChunks = skusList.SplitToChunks( 10 );
				var responses = await skusListChunks.ProcessInBatchAsync( this._getProductsMaxThreads, async x => await this.GetProductsPageAsync( productType, productTypeShouldBeExcluded, x, updatedFrom, cancellationToken ).ConfigureAwait( false ) ).ConfigureAwait( false );
				var soapGetProductsResponse = new SoapGetProductsResponse { Products = responses.SelectMany( x => x.Products ) };
				return soapGetProductsResponse;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		protected virtual async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, string productIdLike, DateTime? updatedFrom, CancellationToken cancellationToken )

		{
			Func< bool, Task< catalogProductListResponse > > call = async ( keepAlive ) =>
			{
				var filtersTemp = new filters();

				if( productType != null )
					AddFilter( filtersTemp, productType, "type", productTypeShouldBeExcluded ? "neq" : "eq" );
				if( updatedFrom.HasValue )
					AddFilter( filtersTemp, updatedFrom.Value.ToSoapParameterString(), "updated_at", "from" );
				if( !string.IsNullOrWhiteSpace( productIdLike ) )
					AddFilter( filtersTemp, productIdLike, "product_id", "like" );

				var filters = filtersTemp;
				//var filters = new MagentoSoapServiceReference_v_1_14_1_EE.filters { filter = new MagentoSoapServiceReference_v_1_14_1_EE.associativeEntity[1]{associativeEntity} };
				var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;
				var res = new catalogProductListResponse();

				const int maxCheckCount = 2;
				const int delayBeforeCheck = 1800000;
				var privateClient = this._clientFactory.GetClient( keepAlive );
				var statusChecker = new StatusChecker( maxCheckCount );
				TimerCallback tcb = statusChecker.CheckStatus;

				privateClient = this._clientFactory.RefreshClient( privateClient, keepAlive );

				var sessionId = await this.GetSessionId( cancellationToken ).ConfigureAwait( false );

				using( var stateTimer = new Timer( tcb, privateClient, 1000, delayBeforeCheck ) )
					res = await privateClient.catalogProductListAsync( sessionId.SessionId, filters, store ).ConfigureAwait( false );

				return res;
			};

			try
			{
				// keep alive is a crutch for 1 client, which has server that sloses connection after few minutes.
				var keepAlive = false;
				var res = new catalogProductListResponse();
				await ActionPolicies.GetAsync.Do( async () =>
				{
					try
					{
						res = await call( keepAlive ).ConfigureAwait( false );
						return;
					}
					catch( CommunicationException )
					{
						keepAlive = !keepAlive;
					}
					res = await call( keepAlive ).ConfigureAwait( false );
				} ).ConfigureAwait( false );

				return new SoapGetProductsResponse( res );
			}
			catch( Exception exc )
			{
				if( exc is CommunicationException ) //crunch for fbeauty
				{
					var r = exc as CommunicationException;
					if( r.InnerException.Message.Contains( "403" ) )
					{
						if( productIdLike.Contains( "00" ) )
						{
							return null;
						}
					}
				}
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		protected virtual async Task< SoapGetProductsResponse > GetProductsPageAsync( string productType, bool productTypeShouldBeExcluded, IEnumerable< string > skus, DateTime? updatedFrom, CancellationToken cancellationToken )
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
				async ( client, session ) => await client.catalogProductListAsync( session, filters, store ).ConfigureAwait( false ), 600000, cancellationToken ).ConfigureAwait( false );
		}

		public Task< SoapGetProductsResponse > GetProductsBySkusAsync(  IEnumerable< string > skus, CancellationToken cancellationToken, Mark mark = null )
		{
			throw new NotImplementedException();
		}
	}
}